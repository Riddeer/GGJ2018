using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using BehaviorDesigner.Runtime;
using GameDataEditor;
using UnityEngine.AI;


public enum Weapon_Gun
{
    NULL = -1,
    Line = 0,
    Line_02 = 1,
    Laser = 2,
    Bezier = 3,
    Boomerang = 4,
    Hold = 5,
    Charge = 6,
    SIZE
}
public class Robot : HeroModel
{
    public bool isRandomGun = false;
    public Weapon_Gun m_Gun = Weapon_Gun.Line;
    public List<int> weaponDataIDs = new List<int>();
    protected bool attacked;
    protected override void Awake()
    {
        base.Awake();
        CreateWeapons(SelectGun(0, 6));

        EnemyManager.instance.m_Enemies.Add(this);
    }
    protected override void Start()
    {
        base.Start();
        m_AniMng.StartSetWink();
        m_AniMng.SwitchWeaponTextrue(m_CurWeapon.m_SpineSkinName,
            m_CurWeapon.m_WeaponID, m_HeroID);
    }
    protected override void Update()
    {
        base.Update();
        m_CurAtkTar = this.GetCurAtkTarget(GetCurAtkTargetList(), TargetsSortType.Distance);
        this.UpdateAtkTarget();
    }
    protected void CreateWeapons(int curWeaponID)
    {
        for (int i = 0; i < weaponDataIDs.Count; i++)
        {
            GDEWeaponData oneWeaponData = GDEManager.instance.
                GetData_Weapon(weaponDataIDs[i]);
            GameObject createdGO = Instantiate(
                Resources.Load(oneWeaponData.PrefabPath),
                Vector3.zero, Quaternion.identity) as GameObject;
            createdGO.transform.SetParent(transform);
            Weapon createdWea = createdGO.GetComponent<Weapon>();
            m_CreatedWeapons.Add(createdWea);
        }
        Debug.Assert(m_CreatedWeapons.Count > 0, "Check Hero's weapons !!!");

        // set cur weapon
        m_CurWeapon = m_CreatedWeapons[curWeaponID];

        for (int j = 0; j < m_CreatedWeapons.Count; j++)
        {
            if (curWeaponID == (int)m_CreatedWeapons[j].m_WeaponID)
            {
                m_CurWeapon = m_CreatedWeapons[j];
            }
        }
    }
    private int SelectGun(int min, int max)
    {
        if (isRandomGun)
        {
            int gun = -1;
            gun = (int)Random.Range(min, max);
            return gun;
        }
        else return (int)m_Gun;
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
    protected override void OnTrigger(Collider2D col)
    {
        if (col.transform.parent == transform) return;
        base.OnTrigger(col);
    }
    public RoleBase PublicGetCurAtkTarget()
    {

        return GetCurAtkTarget(GetCurAtkTargetList(), TargetsSortType.Distance);
    }
    public override void RemoveSelf()
    {
        EnemyManager.instance.RemoveEnemy(this);
        // base.RemoveSelf();
    }
    public override List<RoleBase> GetCurAtkTargetList()// we can use parent scripts method
    {
        List<RoleBase> enemies = new List<RoleBase>();
        foreach (RoleBase one in Global.instance.m_Hero_All)
        {
            if (one == null) continue;
            if (Vector3.Distance(this.GetFootPos(), one.GetFootPos()) <
            m_CurWeapon.m_AlertRange
            && one.m_CurStatus != RoleStatus.Die)
                enemies.Add(one);
        }

        // sort by priority and distance
        enemies.Sort(
            delegate (RoleBase x, RoleBase y)
            {
                if (Vector3.Distance(this.GetFootPos(), x.GetFootPos()) <
                        Vector3.Distance(this.GetFootPos(), y.GetFootPos()))
                {
                    return -1;
                }
                else if (Vector3.Distance(this.GetFootPos(), x.GetFootPos()) >
                    Vector3.Distance(this.GetFootPos(), y.GetFootPos()))
                {
                    return 1;
                }

                return 0;
            });

        // planesID
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            RoleBase oneRole = enemies[i];
            if (oneRole.m_CurPlanesID != Constants.PlanesID_Public &&
            m_CurPlanesID != oneRole.m_CurPlanesID)
            {
                enemies.RemoveAt(i);
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
                this.GetAtkStartPos(), dir,
                m_CurWeapon.m_AlertRange, tLayer);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Scene"))
                    enemies.RemoveAt(i);
            }

        }
        return enemies;

    }
    public override void HandleComplete(TrackEntry trackEntry)
    {
        base.HandleComplete(trackEntry);

        if (trackEntry.ToString() == "Attacked_01")
        {
            // this.Idle();
            // attacked = false;
            // m_BehaviorTree.SetVariableValue("attacked",attacked);

        }
    }


}
