using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    NULL = -1,

    Damage = 0,
    SlowDown = 1,
    Poison = 2,

    SIZE
};

public enum DamageEftType
{
    NULL = -1,

    One = 0,
    Range = 1,

    SIZE
};

public class Damage : MonoBehaviour
{
    public string m_DmgName = "";

    public string m_ActName = "Attack";

    public DamageType m_Type = DamageType.NULL;

    public DamageEftType m_EftType = DamageEftType.NULL;

    public bool m_OnlyEftOnce = true;
    public float m_EftInterval = 0.1f;

    public float m_Thrust = 0f;

    // damage
    public long m_DamageVal = 0;
    [Range(0f, 100f)]
    public float m_CritPercent = 0f;
    public float m_CritDmg = 1f;

    // slow down
    [Range(0f, 1f)]
    public float m_EffectVal = 1f;
    public float m_EffectDura = 0f;

    // posion
    public int m_EffectTimes = 1;


    [Header("Additional Damage")]
    // which can be attacked
    public long m_AddDmg_Castle = 0;
    public long m_AddDmg_Land = 0;
    public long m_AddDmg_Air = 0;
    public long m_AddDmg_OnCastle = 0;

    // must be setted
    [HideInInspector]
    public RoleBase m_RoleBase;
    [HideInInspector]
    public RoleBase m_TarRole = null;
    [HideInInspector]
    public List<RoleBase> m_EffectedBases = new List<RoleBase>();


    private long m_Add_Dmg = 0;

    private float m_Add_Crit = 0;
    private float m_Add_CritDmg = 0;
    private float m_EftMultiple = 1f;


    void Start()
    {

    }

    public void Init(RoleBase creater, RoleBase tar)
    {
        m_RoleBase = creater;

        m_TarRole = tar;

    }

    public void GetAddAtt(long add_Dmg, float add_CritPer, float add_CritDmg)
    {
        m_Add_Dmg = add_Dmg;
        m_Add_Crit = add_CritPer;
        m_Add_CritDmg = add_CritDmg;
    }

    public void SetChargedValue(float val)// 0 1
    {
        // Debug.Log("SetChargedValue = " + val);
        m_EftMultiple = val;
    }

    public float GetTrulyThrust()
    {
        return m_Thrust * m_EftMultiple;
    }

    public bool GetDmg2Role(RoleBase role, out long dmgVal, out bool crit)
    {
        dmgVal = 0;
        crit = false;
        if(role == m_RoleBase) return false;
        if (m_EftType == DamageEftType.One && m_TarRole != null &&
            m_TarRole != role) return false;

        if (this.IsRoleEffected(role)) return false;

        dmgVal = (long)((float)(m_DamageVal + m_Add_Dmg) * m_EftMultiple);

        switch (role.m_BaseType)
        {
            case RoleBaseType.Castle:
                {
                    dmgVal += m_AddDmg_Castle;
                }
                break;
            case RoleBaseType.Land:
                {
                    dmgVal += m_AddDmg_Land;
                }
                break;
            case RoleBaseType.Air:
                {
                    dmgVal += m_AddDmg_Air;
                }
                break;
            case RoleBaseType.OnCastle:
                {
                    dmgVal += m_AddDmg_OnCastle;
                }
                break;

            default:
                break;
        }

        float critPer = m_CritPercent + m_Add_Crit;
        float critDmg = m_CritDmg + m_Add_CritDmg;

        float randomCritVal = UnityEngine.Random.Range(0, 100f);

        crit = randomCritVal < critPer;
        if (crit)
        {
            dmgVal = (int)(critDmg * (float)dmgVal);
        }

        if (m_EftType == DamageEftType.One && m_TarRole != null &&
            m_TarRole == role)
        {
            Destroy(gameObject);
        }

        // regist
        this.RegistEffectedRole(role);

        // report to RoleBase
        if (m_RoleBase != null)
            m_RoleBase.AffectedOther(m_DmgName, role);


        return true;
    }


    public bool GetSlowDownEft2Role(RoleBase role, out float eftVal, out float dura)
    {
        eftVal = 1f;
        dura = 0f;

        if (this.IsRoleEffected(role)) return false;

        eftVal = m_EffectVal;
        dura = m_EffectDura;

        //regist
        this.RegistEffectedRole(role);

        return true;
    }

    public bool GetPosion2Role(RoleBase role, out long dmgVal, out int times)
    {
        dmgVal = 0;
        times = 1;

        if (this.IsRoleEffected(role)) return false;

        dmgVal = m_DamageVal + m_Add_Dmg;
        times = m_EffectTimes;

        //regist
        this.RegistEffectedRole(role);

        return true;
    }

    public bool IsRoleEffected(RoleBase role)
    {
        return m_EffectedBases.Contains(role);
    }

    public void RegistEffectedRole(RoleBase role)
    {
        if (m_EffectedBases.Contains(role)) return;

        m_EffectedBases.Add(role);

        if (!m_OnlyEftOnce)
        {
            StartCoroutine(IE_RemoveRegistedRole(role));
        }
    }

    private IEnumerator IE_RemoveRegistedRole(RoleBase role)
    {
        yield return new WaitForSeconds(m_EftInterval);

        if (m_EffectedBases.Contains(role)) m_EffectedBases.Remove(role);
    }

    public void SetTarLayer(LayerMask tLayer)
    {
        ParticleSystem[] parSys =       GetComponentsInChildren<ParticleSystem>();
        if (parSys.Length == 0)
            return;

            foreach (ParticleSystem oneP in parSys)
            {
                if (oneP.collision.enabled)
                {
                    var col = oneP.collision;
                    col.collidesWith = tLayer;
                }
            }
    }
}
