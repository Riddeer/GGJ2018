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

public class Hero : HeroModel
{
    public Slider m_ChargeSlider;
    public Slider m_BulletSlider;
    public GameObject m_CurTarMark;
    public GameObject[] m_WeaponsToCreate;

    private SpriteRenderer m_SkillPointing_CastDis;
    private Slider m_ShieldSlider;

    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();

        this.UpdateOpaMode(InputManager.instance.m_CurOpaMode);
        this.CreateWeapons();
        m_PrepareChangeParticle.SetActive(false);
        m_AniMng.SwitchWeaponTextrue(m_CurWeapon.m_SpineSkinName,
        m_CurWeapon.m_WeaponID, m_HeroID);

        m_CurTarMark.SetActive(false);

        // test 
        // m_Invincible = true;
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
        base.Update();

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
        m_CurWeapon.SetFullBullet();

        if (m_BulletSlider != null)
        {
            RectTransform fillRect = m_BulletSlider.fillRect;
            Image fillImg = fillRect.GetComponent<Image>();
            if (fillImg != null)
            {
                fillImg.color = Color.red;
            }
            m_BulletSlider.DOValue(1f, m_CurWeapon.m_ReloadTime)
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
        if (m_BulletSlider == null) return;

        m_BulletSlider.DOValue(m_CurWeapon.GetBulletPercent(), 0.5f);

    }


    public override void UpdateHPShow()
    {
        float val_HP = (float)m_CurHP / (float)m_MaxHP;
        float val_Shield = (float)m_CurShieldVal / (float)m_MAXShieldVal;

        if (m_HPSlider != null) m_HPSlider.value = val_HP;
        if (m_ShieldSlider != null) m_ShieldSlider.value = val_Shield;
    }


}
