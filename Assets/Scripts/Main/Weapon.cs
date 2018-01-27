using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponOperateType
{
    NULL = -1,

    OnePunch = 0,
    Hold = 1,
    Charge = 2,

    SIZE
};

public enum WeaponAniType
{
    NULL = -1,

    OneHand = 0,
    TwoHands = 1,

    SIZE
}

public enum WeaponID
{
    NULL = -1,
    Line = 0,
    Line_2 = 1,
    Laser = 2,
    Bezier = 3,
    Boomerang = 4,
    Hold = 5,
    Charge = 6,
    SIZE
}
public class Weapon : MonoBehaviour
{
    public delegate void WeaponEventProxy(Weapon weapon);
    public event WeaponEventProxy ReloadStart;
    public event WeaponEventProxy ReloadComplete;
    public string m_WeaponName = "Weapon_001";
    public WeaponID m_WeaponID ;
    public WeaponOperateType m_OpaType = WeaponOperateType.NULL;
    public WeaponAniType m_AniType = WeaponAniType.NULL;
    public GameObject m_Dmg;
    public float m_AtkInterval = 1f;
    public string m_SpineSkinName = "";
    public string m_Audio;
    public float m_ChargeTime = 0f;
    public int m_MaxBullet = 10;
    public int m_ConsumePerHit = 1;
    public float m_ReloadTime = 5f;
    public float m_AlertRange = 10f;
    public float prepareFireTime = 0f;
    public AimArrowType m_AimType;
    public int m_CurBullet = 0;
    private IEnumerator m_IE_Reload = null;
	private bool m_IsReloading = false;

    void Start()
    {
        m_CurBullet = m_MaxBullet;
        m_IE_Reload = null;
		m_IsReloading = false;
    }

    public bool ConsumeBullet(int val)
    {
        if (m_CurBullet < val)
        {
            return false;
        }

        m_CurBullet -= val;
        m_CurBullet = Mathf.Clamp(m_CurBullet, 0, m_MaxBullet);

		return true;
    }

    public bool CheckBullet(int val)
    {
        return m_CurBullet >= val;
    }

    public void SetFullBullet(ref int roleBullet)
    {
        if (roleBullet >= m_MaxBullet)
        {
            roleBullet -= m_MaxBullet;
            m_CurBullet = m_MaxBullet;
        }
        else
        {
            m_CurBullet = roleBullet;
            roleBullet = 0;
        }
    }

    public float GetBulletPercent()
    {
        return (float)m_CurBullet / (float)m_MaxBullet;
    }

}