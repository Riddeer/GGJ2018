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


public enum HeroID
{
    NULL = -1,
    SpaceKnight = 0,
    IdiotKnight = 1,
    SIZE
}
public enum Skin_IdiotKnight
{
    NULL,
    IdiotKnight,
    StupidKnight,
    Skin_01,
    Skin_02,
    SIZE
}
public class HeroModel : RoleBase
{
    public Skin_IdiotKnight m_Skin = Skin_IdiotKnight.StupidKnight;
    public long m_MAXShieldVal = 100;
    public long m_ShieldRestoreVec = 10;
    public float m_ShieldRestoreInterval = 1f;
    public float m_ShieldRestoreBreak = 3f;
    public HeroID m_HeroID;
    public int m_AllBullet = 10;

    private float m_NextShootTime;
    private int m_LeftNorAtkTimes = 0;
    readonly int m_MAXNumOfLeftNorAtkTimes = 1;
    private IEnumerator m_IE_CheckLeftNorAtkTimes = null;
    protected Effect m_DmgEft = null;
    private IEnumerator m_IE_AutoReleaseHold = null;
    private IEnumerator m_IE_ConsumeBullet = null;
    private readonly float AutoReleaseHoldDelay = 0.5f;
    protected float m_ChargedValue = 0f;
    private float m_ChargStartTime = 0f;
    private IEnumerator m_IE_AutoReleaseCharge = null;
    private readonly float AutoReleaseChargeDelay = 0.5f;
    protected List<Weapon> m_CreatedWeapons = new List<Weapon>();
    protected bool m_IsReloading = false;
    protected long m_CurShieldVal = 0;
    private IEnumerator m_IE_ShieldAutoRestore = null;
    protected OperateMode m_OpaMode = OperateMode.NULL;
    private float m_FirstAtkWaiteTime = 0;
    private IEnumerator m_IE_ShortestInterAtk = null;


    protected override void Awake()
    {
        base.Awake();

        m_LeftNorAtkTimes = 0;
        m_NextShootTime = Time.time;
        m_IE_CheckLeftNorAtkTimes = null;
        m_IE_AutoReleaseHold = null;
        m_IE_ConsumeBullet = null;
        m_IE_AutoReleaseCharge = null;
        m_IsReloading = false;
        m_CurShieldVal = m_MAXShieldVal;
        m_IE_ShieldAutoRestore = null;
        m_IE_ShortestInterAtk = null;
    }

    protected override void Start()
    {
        base.Start();
        m_AniMng.StartSetWink();
        m_AniMng.SetSkin(m_Skin.ToString());
        // m_Shield.SetActive(true);

    }




    public virtual void CreateWeapons()
    {

    }

    public void UpdateFirstAtkWaiteTime(bool toZero = false)
    {
        if (m_OpaMode == OperateMode.AutoFindEnemies ||
            m_OpaMode == OperateMode.AutoFindEnemies_RollTarget_JS)
        {
            m_FirstAtkWaiteTime = 0;
        }
        else
        {
            if (toZero)
            {
                m_FirstAtkWaiteTime = 0;
            }
            else if (m_CurWeapon != null)
            {
                m_FirstAtkWaiteTime = m_CurWeapon.prepareFireTime;
            }

        }
    }

    protected override void Update()
    {
        this.UpdateFlip();
        m_AniMng.Aim(this.GetCurFaceVec());


    }
    protected override string Get_Atk_01_AudioName()
    {
        return Constants.Lancer_Attack_01;
    }
    protected override string Get_Step_AudioName()
    {
        return Constants.Lancer_Step_01;
    }
    protected override string Get_Death_AudioName()
    {
        return Constants.Lancer_Death_01;
    }
    protected override string Get_Crow_AudioName()
    {
        return Constants.Lancer_Crow_01;
    }

    public override void Die()
    {
        m_CurStatus = RoleStatus.Die;

        // Audio play
        float pitch = 0.8f + UnityEngine.Random.Range(-0.3f, 0.3f);

        AudioManager.instance.Play(Get_Death_AudioName(), pitch);
        m_PrepareChangeParticle.SetActive(false);//hero
        if (m_AniMng != null)
        {
            m_AniMng.StopWink();
            m_AniMng.Die();
        }
        else
        {
            this.RemoveSelf();
        }
    }
    public void ReloadWeapon()
    {
        StartCoroutine(IE_ReloadWeapon());
    }
    protected virtual IEnumerator IE_ReloadWeapon()
    {
        m_IsReloading = true;
        m_CurWeapon.SetFullBullet(ref m_AllBullet);

        yield return new WaitForSeconds(m_CurWeapon.m_ReloadTime);

        m_IsReloading = false;
    }
    public override void Attack()
    {
        if (m_IsReloading) return;
        m_AniMng.AngryFace();
        GameObject dmgPrefab = m_CurWeapon.m_Dmg;

        switch (m_CurWeapon.m_OpaType)
        {
            case WeaponOperateType.OnePunch:
                {
                    if (m_FirstAtkWaiteTime > 0)//hero
                    {
                        m_PrepareChangeParticle.SetActive(true);//hero
                        m_NextShootTime = Time.time + m_FirstAtkWaiteTime;
                        m_FirstAtkWaiteTime = 0;
                        this.AddLeftNorAtk();
                        return;
                    }
                    else if (Time.time < m_NextShootTime)
                    {
                        this.AddLeftNorAtk();
                        return;
                    }
                    else
                    {
                        m_PrepareChangeParticle.SetActive(false);//hero
                        // Time.time >= m_NextShootTime
                        m_NextShootTime = Time.time + m_CurWeapon.m_AtkInterval;

                        // consume bullet
                        if (!m_CurWeapon.ConsumeBullet(
                            m_CurWeapon.m_ConsumePerHit))
                        {
                            this.Talk("NO AMMO", 1f);
                            return;
                        }

                        // to make sure the atk is continuse
                        if (m_IE_ShortestInterAtk != null)//hero
                        {
                            StopCoroutine(m_IE_ShortestInterAtk);
                            m_IE_ShortestInterAtk = null;
                        }
                        m_IE_ShortestInterAtk = this.IE_ShortestInterAtk();//hero
                        StartCoroutine(m_IE_ShortestInterAtk);//hero

                        this.Attack(dmgPrefab);

                        if (!m_CurWeapon.CheckBullet(m_CurWeapon.m_ConsumePerHit))
                            this.ReloadWeapon();
                    }
                }
                break;

            case WeaponOperateType.Hold:
                {
                    if (m_CurAtkGO == null && m_DmgEft == null)
                    {
                        this.Attack(dmgPrefab);

                        // consum bullet
                        if (m_IE_ConsumeBullet != null)
                        {
                            StopCoroutine(m_IE_ConsumeBullet);
                            m_IE_ConsumeBullet = null;
                        }

                        m_IE_ConsumeBullet = IE_ConsumeBullet();
                        StartCoroutine(m_IE_ConsumeBullet);
                    }

                    if (m_DmgEft != null)
                    {
                        m_DmgEft.UpdateRotation(this.GetCurFaceVec().normalized);
                    }

                    if (m_IE_AutoReleaseHold != null)
                    {
                        StopCoroutine(m_IE_AutoReleaseHold);
                        m_IE_AutoReleaseHold = null;
                    }

                    m_IE_AutoReleaseHold = IE_AutoReleaseHold();
                    StartCoroutine(m_IE_AutoReleaseHold);

                }
                break;

            case WeaponOperateType.Charge:
                {
                    if (m_DmgEft == null)
                    {
                        m_PrepareChangeParticle.SetActive(true);//hero

                        // consume bullet
                        if (!m_CurWeapon.ConsumeBullet(
                            m_CurWeapon.m_ConsumePerHit)) return;

                        this.Attack(dmgPrefab);
                        GameObject dmgGo = this.CreateDmg();
                        m_DmgEft = dmgGo.GetComponent<Effect>();

                        m_ChargStartTime = Time.time;
                        m_ChargedValue = 0f;
                        Damage dmg = m_DmgEft.GetComponent<Damage>();
                        dmg.SetChargedValue(m_ChargedValue);


                    }
                    else
                    {

                        float chargedTime = Time.time - m_ChargStartTime;
                        float allChargeTime = m_CurWeapon.m_ChargeTime;
                        m_ChargedValue = chargedTime / allChargeTime;
                        m_ChargedValue = Mathf.Clamp(m_ChargedValue,
                            0f, 1f);

                        m_DmgEft.SetChargedValue(m_ChargedValue);
                        Damage dmg = m_DmgEft.GetComponent<Damage>();
                        dmg.SetChargedValue(m_ChargedValue);
                    }

                    if (m_IE_AutoReleaseCharge != null)
                    {
                        StopCoroutine(m_IE_AutoReleaseCharge);
                        m_IE_AutoReleaseCharge = null;
                    }

                    m_IE_AutoReleaseCharge = IE_AutoReleaseCharge();
                    StartCoroutine(m_IE_AutoReleaseCharge);
                }
                break;

            default:
                {

                }
                break;
        }
    }

    public void StopAttack()//hero
    {
        this.UpdateFirstAtkWaiteTime();
        // m_InteChoisePointing.gameObject.SetActive(false);//hero
        // m_AimArrow.SetActive(false);//hero
        switch (m_CurWeapon.m_OpaType)
        {
            case WeaponOperateType.OnePunch:
                {
                    this.ClearLeftNorAtk();

                    if (m_IE_ShortestInterAtk != null)
                    {
                        StopCoroutine(m_IE_ShortestInterAtk);
                        m_IE_ShortestInterAtk = null;
                    }
                }
                break;

            case WeaponOperateType.Hold:
                {
                    this.ReleaseHold();
                }
                break;

            case WeaponOperateType.Charge:
                {
                    this.ReleaseCharge();
                }
                break;

            default:
                break;
        }
    }
    private IEnumerator IE_ShortestInterAtk()//hero
    {
        yield return new WaitForSeconds(m_CurWeapon.m_AtkInterval);

        this.Attack();
    }
    protected void AddLeftNorAtk()
    {
        m_LeftNorAtkTimes++;
        m_LeftNorAtkTimes = Mathf.Clamp(m_LeftNorAtkTimes, 0, m_MAXNumOfLeftNorAtkTimes);

        if (m_IE_CheckLeftNorAtkTimes == null)
        {
            m_IE_CheckLeftNorAtkTimes =
                CheckLeftNorAtkTimes(m_NextShootTime - Time.time);
            StartCoroutine(m_IE_CheckLeftNorAtkTimes);
        }

    }
    private IEnumerator CheckLeftNorAtkTimes(float waiteTime)
    {
        float t = waiteTime;

        while (m_LeftNorAtkTimes > 0)
        {
            yield return new WaitForSeconds(t);
            m_LeftNorAtkTimes--;

            this.Attack();


            t = m_CurWeapon.m_AtkInterval;
        }

        m_IE_CheckLeftNorAtkTimes = null;
    }
    private void ClearLeftNorAtk()
    {
        m_LeftNorAtkTimes = 0;
        if (m_IE_CheckLeftNorAtkTimes != null)
        {
            StopCoroutine(m_IE_CheckLeftNorAtkTimes);
            m_IE_CheckLeftNorAtkTimes = null;
        }
    }
    private void ReleaseHold()
    {
        if (m_IE_AutoReleaseHold != null)
        {
            StopCoroutine(m_IE_AutoReleaseHold);
            m_IE_AutoReleaseHold = null;
        }
        if (m_IE_ConsumeBullet != null)
        {
            StopCoroutine(m_IE_ConsumeBullet);
            m_IE_ConsumeBullet = null;
        }

        m_CurAtkGO = null;

        if (m_DmgEft != null)
        {
            m_DmgEft.DestroySelf();
            m_DmgEft = null;
        }

        // resume animation
        this.Idle();
    }
    private void ReleaseCharge()
    {
        if (m_IE_AutoReleaseCharge != null)
        {
            StopCoroutine(m_IE_AutoReleaseCharge);
            m_IE_AutoReleaseCharge = null;
        }

        m_CurAtkGO = null;

        // shoot
        if (m_DmgEft != null)
        {
            m_DmgEft.UpdateRotation(this.GetCurFaceVec().normalized);
            m_DmgEft.Play();
            m_DmgEft = null;

            if (!m_CurWeapon.CheckBullet(m_CurWeapon.m_ConsumePerHit))
                this.ReloadWeapon();
        }

        m_ChargStartTime = Time.time;
        m_ChargedValue = 0f;

        m_PrepareChangeParticle.SetActive(false);//hero

        // resume animation
        this.Idle();
    }
    private IEnumerator IE_AutoReleaseHold()
    {
        yield return new WaitForSeconds(AutoReleaseHoldDelay);

        this.ReleaseHold();
    }
    private IEnumerator IE_AutoReleaseCharge()
    {
        yield return new WaitForSeconds(AutoReleaseChargeDelay);

        this.ReleaseCharge();
    }
    IEnumerator IE_ConsumeBullet()
    {
        while (true)
        {
            if (!m_CurWeapon.ConsumeBullet(
                m_CurWeapon.m_ConsumePerHit)) this.StopAttack();
            if (!m_CurWeapon.CheckBullet(m_CurWeapon.m_ConsumePerHit))
                this.ReloadWeapon();

            yield return new WaitForSeconds(m_CurWeapon.m_AtkInterval);
        }
    }
    public override void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == Constants.SpineEventName_Attack)
        {
            this.AtkAnimationEvent();
            AudioManager.instance.Play(m_CurWeapon.m_Audio);
        }
        if (e.Data.Name == Constants.SpineEventName_Step ||
            e.Data.Name == m_AniMng.m_AniName_Move)
        {
            float pitch = 0.9f + UnityEngine.Random.Range(-0.15f, 0.15f);
            // footstepAudioSource.Play();
            AudioManager.instance.Play(Get_Step_AudioName(), pitch);
        }
    }

    protected override void AtkAnimationEvent()
    {
        switch (m_CurWeapon.m_OpaType)
        {
            case WeaponOperateType.OnePunch:
                {
                    this.CreateDmg();
                }
                break;

            case WeaponOperateType.Hold:
                {
                    GameObject go = this.CreateDmg();
                    if (go == null) return;

                    m_DmgEft = go.GetComponent<Effect>();

                    // play
                    m_DmgEft.Play();

                    // pause animation

                }
                break;

            case WeaponOperateType.Charge:
                {

                    // pause animation

                }
                break;

            default:
                break;
        }

    }
    public override bool GetHurt(long val, RoleBase atker)//hero
    {
        // Debug.Log(gameObject.name + "Get hurt , val = " + val);

        if (m_CurStatus == RoleStatus.Die) return false;

        if (m_Invincible) return false;

        if (m_CurShieldVal > 0)
        {
            // m_Shield.SetActive(true);
            if (m_CurShieldVal >= val)
            {
                m_CurShieldVal -= val;
                val = 0;
            }
            else
            {
                val -= m_CurShieldVal;
                m_CurShieldVal = 0;
                // m_Shield.SetActive(false);
            }
        }

        bool result = base.GetHurt(val, atker);

        if (result && m_CurStatus != RoleStatus.Die)
        {
            // break shiled_autorestore
            if (m_IE_ShieldAutoRestore != null)
            {
                StopCoroutine(m_IE_ShieldAutoRestore);
                m_IE_ShieldAutoRestore = null;
            }

            m_IE_ShieldAutoRestore = this.IE_ShieldAutoRestore();
            StartCoroutine(m_IE_ShieldAutoRestore);
        }

        return result;
    }
    protected override void AtkAnimationComplete()
    {
        switch (m_CurWeapon.m_OpaType)
        {
            case WeaponOperateType.OnePunch:
                {
                    // this.Idle();
                }
                break;

            case WeaponOperateType.Hold:
                {


                }
                break;

            case WeaponOperateType.Charge:
                {

                }
                break;

            default:
                break;
        }
    }
    private IEnumerator IE_ShieldAutoRestore()
    {
        yield return new WaitForSeconds(m_ShieldRestoreBreak);

        while (true)
        {
            this.ShieldRestore(m_ShieldRestoreVec);

            yield return new WaitForSeconds(m_ShieldRestoreInterval);
        }

    }

    private void ShieldRestore(long val)
    {
        if (m_CurStatus == RoleStatus.Die) return;
        if (m_CurShieldVal == m_MAXShieldVal) return;

        m_CurShieldVal += val;
        if (m_CurShieldVal > m_MAXShieldVal) m_CurShieldVal = m_MAXShieldVal;

        this.UpdateHPShow();
    }
    public void UpdateOpaMode(OperateMode opaMode)
    {
        this.SetCanAutoFindEnemies(opaMode);
    }
    private void SetCanAutoFindEnemies(OperateMode val)
    {
        if (m_OpaMode == val) return;

        m_OpaMode = val;

        this.UpdateFirstAtkWaiteTime();
    }
}
