using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime;
using GameDataEditor;
using Spine.Unity;
using System.Linq;
using DynamicLight2D;

public enum TransmissionType
{
    NULL = -1,
    Fat = 0,
    Thin = 1,

    SIZE
};

public class Hero : HeroModel
{
    public TransmissionType m_CurTransType = TransmissionType.Fat;
    public Slider m_ChargeSlider;
    public Slider m_BulletSlider;
    public Text m_BulletText;
    public GameObject m_CurTarMark;
    public GameObject[] m_WeaponsToCreate;
    public float m_SuckSpeed = 10f;

    // [HideInInspector]
    public bool m_IsReadyForTransmission = false;
    // [HideInInspector]
    public bool m_IsInDisForTransmission = false;

    private SpriteRenderer m_SkillPointing_CastDis;
    private Slider m_ShieldSlider;
    private Hero m_OtherHero;
    private const float m_MaxDisForTransmission = 5f;
    private const float m_TransmissionDis = 3f;
    private IEnumerator m_IE_SuckPool;
    private WaterPool m_CurSuckPool;

    protected override void Awake()
    {
        base.Awake();
        m_IsReadyForTransmission = false;
        m_IsInDisForTransmission = false;
        m_IE_SuckPool = null;
        m_CurSuckPool = null;
    }

    protected override void Start()
    {
        base.Start();

        this.UpdateOpaMode(InputManager.instance.m_CurOpaMode);
        this.CreateWeapons();
        if (m_PrepareChangeParticle) m_PrepareChangeParticle.SetActive(false);
        m_AniMng.SwitchWeaponTextrue(m_CurWeapon.m_SpineSkinName,
        m_CurWeapon.m_WeaponID, m_HeroID);
        if (m_ChargeSlider) m_ChargeSlider.gameObject.SetActive(false);

        m_CurTarMark.SetActive(false);

        foreach (Hero one in Global.instance.m_Hero_All)
        {
            if (one != this)
            {
                m_OtherHero = one;
                break;
            }
        }
        // test 
        // m_Invincible = true;
        this.UpdateTransmissionType();
    }
    protected override void Update()
    {
        if (!Global.instance.m_GameStart)
        {
            return;
        }
        if (m_OpaMode == OperateMode.AutoFindEnemies)
        {
            if (!GetCurAtkTargetList().Contains(m_CurAtkTar))
            {
                m_CurAtkTar = this.GetCurAtkTarget(GetCurAtkTargetList(), TargetsSortType.Distance);
            }

            this.UpdateAtkTarget();
        }
        else if (m_OpaMode == OperateMode.AutoFindEnemies_RollTarget_JS)
        {
            this.UpdateAtkTarget();
        }

        this.UpdateCurAtkTarMark(m_CurAtkTar);


        this.UpdateChargeShow();
        this.UpdateBulletShow();
        this.UpdateCheckTransmission();

        base.Update();

    }
    private void UpdateCheckTransmission()
    {
        if (m_CurStatus == RoleStatus.Transmission) return;

        float dis = Vector2.Distance(this.GetFootPos(), m_OtherHero.GetFootPos());
        if (dis <= m_MaxDisForTransmission)
        {
            // show tip
            // Debug.Log("Distance is right now!!!");

            m_IsInDisForTransmission = true;
        }
        else
        {
            m_IsReadyForTransmission = false;
            m_IsInDisForTransmission = false;
        }

        // if (m_IsReadyForTransmission)
        // {
        //     if (m_OtherHero.m_IsReadyForTransmission)
        //     {
        //         this.Transmission();
        //     }
        // }
    }

    public void EnsureTransmission()
    {
        if (!m_IsInDisForTransmission) return;

        this.Talk("COME ON", 3f);

        m_IsReadyForTransmission = true;
        if (m_OtherHero.m_IsReadyForTransmission)
        {
            this.Transmission();
            m_OtherHero.Transmission();
        }
    }

    public void Transmission()
    {
        if (m_CurStatus == RoleStatus.Transmission) return;

        m_IsReadyForTransmission = false;
        m_IsInDisForTransmission = false;

        // stop suck
        if (m_IE_SuckPool != null)
        {
            StopCoroutine(m_IE_SuckPool);
            m_IE_SuckPool = null;
            m_CurSuckPool = null;
        }

        m_CurStatus = RoleStatus.Transmission;
        m_AniMng.Transmission();

        // set face 
        bool otherIsRight = m_OtherHero.GetFootPos().x > this.GetFootPos().x;
        // this.SetFlipX(!otherIsRight);
        this.SetCurVec_Move( otherIsRight ? Vector2.right: Vector2.left);

        // set position
        if (m_CurTransType == TransmissionType.Thin)
        {
            transform.position = new Vector3(
                m_OtherHero.GetFootPos().x +
                    m_TransmissionDis * (otherIsRight ? -1f : 1f),
                m_OtherHero.GetFootPos().y,
                0
            );
        }
    }

    public override void CreateWeapons()
    {
        foreach (GameObject one in m_WeaponsToCreate)
        {
            GameObject createdGO = Instantiate(
                Resources.Load("Weapon/" + one.name),
                Vector3.zero, Quaternion.identity) as GameObject;
            createdGO.transform.SetParent(transform);
            Weapon createdWea = createdGO.GetComponent<Weapon>();
            m_CreatedWeapons.Add(createdWea);
        }
        Debug.Assert(m_CreatedWeapons.Count > 0, "Check Hero's weapons !!!");

        // set cur weapon
        m_CurWeapon = m_CreatedWeapons[0];
        // m_CurWeapon.SetFullBullet(ref m_AllBullet);
        this.UpdateFirstAtkWaiteTime();
        // init weapon ui text
        // InputManager.instance.InitWeaponUIText();
    }



    public void RollWeapon()
    {
        int idx = m_CreatedWeapons.FindIndex(a => a == m_CurWeapon);

        idx++;
        if (idx == m_CreatedWeapons.Count) idx = 0;

        m_CurWeapon = m_CreatedWeapons[idx];

        // clear data
        m_CurAtkGO = null;
        if (m_DmgEft != null)
        {
            m_DmgEft.DestroySelf();
        }
        // role weapon textrue
        // is not longer need AtkAnimationName,now we need spine weapon textrue name
        m_AniMng.SwitchWeaponTextrue(m_CurWeapon.m_SpineSkinName,
            m_CurWeapon.m_WeaponID, m_HeroID);
    }

    public override List<RoleBase> GetCurAtkTargetList()
    {
        List<RoleBase> enemies = new List<RoleBase>();
        foreach (RoleBase one in EnemyManager.instance.m_Enemies)
        {
            if (Vector3.Distance(this.GetFootPos(), one.GetFootPos()) <
            m_CurWeapon.m_AlertRange
            && one.m_CurStatus != RoleStatus.Die)
                enemies.Add(one);
        }

        // planesID
        int mineHeroPlanesID = Global.instance.m_Hero_01.m_CurPlanesID;
        for (int i = enemies.Count - 1; i >= 0; i--)
        {

            RoleBase oneRole = enemies[i];
            if (oneRole.m_CurPlanesID != Constants.PlanesID_Public &&
            m_CurPlanesID != oneRole.m_CurPlanesID)
            {
                enemies.RemoveAt(i);
            }
            else if (m_CurPlanesID == mineHeroPlanesID && m_CurPlanesID != Constants.PlanesID_Public)
            {
                m_AniMng.SetAlpha(0.3f);
            }
            else
            {
                // m_AniMng.SetAlpha(0f);
            }
        }


        // 2ddl
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            RoleBase oneRole = enemies[i];
            Vector2 dir = (oneRole.GetFootPos() -
                this.GetFootPos()).normalized;
            LayerMask tLayer = m_TarLayer & (~(
                1 << LayerMask.NameToLayer("Plane")
            ));
            // check raycast
            RaycastHit2D hit = Physics2D.Raycast(
                this.GetMidPos(), dir,
                m_CurWeapon.m_AlertRange, tLayer);
            Debug.DrawLine(this.GetMidPos(), this.GetMidPos() + (Vector3)dir * 10, Color.red);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Scene"))
                    enemies.RemoveAt(i);
            }
            //   m_TarLayer.dw
        }

        return enemies;


    }
    private void UpdateCurAtkTarMark(RoleBase tar)
    {
        if (m_CurTarMark == null) return;

        if (m_OpaMode == OperateMode.ManualAtk ||
            m_OpaMode == OperateMode.ManualAtk_AuxAim)
        {
            m_CurTarMark.SetActive(false);
            return;
        }
        if (tar == null)
        {
            m_CurTarMark.SetActive(false);
            return;
        }


        // follow
        m_CurTarMark.SetActive(true);
        m_CurTarMark.transform.position = tar.GetMidPos();

    }
    public override void Attack()
    {
        if (m_CurTransType == TransmissionType.Thin) return;

        if (m_CurStatus == RoleStatus.Transmission) return;

        base.Attack();
    }

    public override void Die()
    {
        base.Die();
        CameraEffect.instance.Shake(0.2f, 1f, 0.1f);


        SetPlayerAliveStatus(false);
    }
    public override void RemoveSelf()
    {
        GameObject.Destroy(gameObject);
    }
    public void SetPlayerAliveStatus(bool val)
    {
        var bl = GlobalVariables.Instance.GetVariable("playerAlive");
        bl.SetValue(val);
    }
    protected override IEnumerator IE_ReloadWeapon()
    {
        m_IsReloading = true;
        float endVal = m_AllBullet >= m_CurWeapon.m_MaxBullet ? 1f :
            (float)m_AllBullet / (float)m_CurWeapon.m_MaxBullet;
        m_CurWeapon.SetFullBullet(ref m_AllBullet);

        if (m_BulletSlider != null)
        {
            RectTransform fillRect = m_BulletSlider.fillRect;
            Image fillImg = fillRect.GetComponent<Image>();
            if (fillImg != null)
            {
                fillImg.color = Color.red;
            }
            m_BulletSlider.DOValue(endVal, m_CurWeapon.m_ReloadTime)
            .SetEase(Ease.Linear)
            .OnComplete(delegate ()
            {
                if (fillImg != null)
                {
                    fillImg.color = Color.white;
                }
            });
        }

        yield return new WaitForSeconds(m_CurWeapon.m_ReloadTime);

        m_IsReloading = false;
    }
    private void UpdateChargeShow()
    {
        if (m_ChargeSlider == null) return;

        m_ChargeSlider.gameObject.SetActive(m_ChargedValue > 0f);

        m_ChargeSlider.DOValue(m_ChargedValue, 0.5f);

        // m_ChargeSlider.value = m_ChargedValue;
    }

    private void UpdateBulletShow()
    {
        if (m_IsReloading) return;
        if (m_BulletText)
        {
            m_BulletText.text = m_CurWeapon.m_CurBullet + 
                " / " + m_AllBullet;
        }

        if (m_BulletSlider)
        {
            m_BulletSlider.DOValue(m_CurWeapon.GetBulletPercent(), 0.5f);
        }
    }


    public override void UpdateHPShow()
    {
        float val_HP = (float)m_CurHP / (float)m_MaxHP;
        float val_Shield = (float)m_CurShieldVal / (float)m_MAXShieldVal;

        if (m_HPSlider != null) m_HPSlider.value = val_HP;
        if (m_ShieldSlider != null) m_ShieldSlider.value = val_Shield;
    }

    public override void HandleComplete(TrackEntry trackEntry)
    {
        base.HandleComplete(trackEntry);
        if (trackEntry.ToString() == m_AniMng.m_AniName_TransMission)
        {
            if (m_CurStatus == RoleStatus.Transmission)
            {
                this.SwitchTransmissionType();

                m_CurStatus = RoleStatus.Idle;
                this.Idle();
            }

        }
    }

    public override void Idle()
    {
        if (m_CurStatus == RoleStatus.Transmission) return;

        base.Idle();
    }
    private void SwitchTransmissionType()
    {
        m_CurTransType = m_CurTransType == TransmissionType.Fat ?
            TransmissionType.Thin : TransmissionType.Fat;

        this.UpdateTransmissionType();
    }

    private void UpdateTransmissionType()
    {
        switch (m_CurTransType)
        {
            case TransmissionType.Fat:
                {
                    m_MoveVec = 1f;
                }
                break;

            case TransmissionType.Thin:
                {
                    m_MoveVec = 3f;
                }
                break;

            default:
                break;
        }

        m_AniMng.SetCurTransmissionRes(m_CurTransType);
    }

    protected override void OnTrigger(Collider2D col)
    {
        base.OnTrigger(col);

        WaterPool pool = col.gameObject.GetComponent<WaterPool>();
        if (m_CurTransType == TransmissionType.Thin &&
        m_CurStatus != RoleStatus.Transmission && pool)
        {
            if (pool != m_CurSuckPool)
            {
                if (m_IE_SuckPool != null)
                {
                    StopCoroutine(m_IE_SuckPool);
                    m_IE_SuckPool = null;
                    m_CurSuckPool = null;
                }
                m_IE_SuckPool = IE_SuckPool(pool);
                StartCoroutine(m_IE_SuckPool);
            }
        }
    }
    protected override void OnTrigger_Exit(Collider2D col)
    {
        base.OnTrigger_Exit(col);

        WaterPool pool = col.gameObject.GetComponent<WaterPool>();
        if (m_CurTransType == TransmissionType.Thin &&
        m_CurStatus != RoleStatus.Transmission && pool)
        {
            if (pool == m_CurSuckPool)
            {
                if (m_IE_SuckPool != null)
                {
                    StopCoroutine(m_IE_SuckPool);
                    m_IE_SuckPool = null;
                    m_CurSuckPool = null;
                }
            }
        }
    }
    private IEnumerator IE_SuckPool(WaterPool pool)
    {
        m_CurSuckPool = pool;

        while (true)
        {
            if (m_CurSuckPool)
            {
                m_CurSuckPool.BeSucked(this);
            }
            else
            {
                StopCoroutine(m_IE_SuckPool);
                m_IE_SuckPool = null;
            }

            yield return null;
        }
    }

    public override void SetFlipX(bool flipX)
    {
        if (m_Flip == flipX) return;
        m_Flip = flipX;

        float flipVal = flipX ? -1f : 1f;
        this.GetRes().SetLocalScale_X(
            Mathf.Abs(this.GetRes().localScale.x) * flipVal);
        Transform child1 = transform.GetChild(1);
        child1.SetLocalScale_X(
            Mathf.Abs(child1.localScale.x) * flipVal);
    }
}
