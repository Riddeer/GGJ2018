using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using Spine;

public class Enemy : RoleBase
{
    // [HideInInspector]
    // public NavMeshAgent m_Agent;

    // public string behaviorDesignerPath = "BehaviorTree/NormalAI";
    // protected BehaviorTree m_BehaviorTree = null;

    protected override void Awake()
    {

        base.Awake();
        // m_Agent = gameObject.GetComponentInChildren<NavMeshAgent>();

        // ExternalBehaviorTree extBT = Resources.Load<ExternalBehaviorTree>(behaviorDesignerPath);
        // BehaviorTree bt = gameObject.AddComponent<BehaviorTree>();
        // bt.ExternalBehavior = extBT;
        // bt.StartWhenEnabled = false;
        // bt.RestartWhenComplete = true;
        // m_BehaviorTree = bt;
        // m_BehaviorTree.DisableBehavior();
        // if (m_PhotonView == null)
        // {
        //     m_BehaviorTree.EnableBehavior();
        // }
        // else if (m_PhotonView.isMine)
        // {
        //     m_BehaviorTree.EnableBehavior();
        // }
        EnemyManager.instance.m_Enemies.Add(this);
    }
    public override void RemoveSelf()
    {
        EnemyManager.instance.RemoveEnemy(this);
        // base.RemoveSelf();
    }
    // protected override RoleBase GetCurAtkTarget()
    // {
    //     List<RoleBase> enemies = new List<RoleBase>();
    //     foreach (RoleBase one in Global.instance.m_Player_All.Values)
    //     {
    //         if (Vector3.Distance(this.GetFootPos(), one.GetFootPos()) <
    //         m_AlertRange
    //         && one.m_CurStatus != RoleStatus.Die)
    //             enemies.Add(one);
    //     }

    //     // sort by priority and distance
    //     enemies.Sort(
    //         delegate (RoleBase x, RoleBase y)
    //         {
    //             if (Vector3.Distance(this.GetFootPos(), x.GetFootPos()) <
    //                     Vector3.Distance(this.GetFootPos(), y.GetFootPos()))
    //             {
    //                 return -1;
    //             }
    //             else if (Vector3.Distance(this.GetFootPos(), x.GetFootPos()) >
    //                 Vector3.Distance(this.GetFootPos(), y.GetFootPos()))
    //             {
    //                 return 1;
    //             }

    //             return 0;
    //         });

    //     // planesID
    //     for (int i = enemies.Count - 1; i >= 0; i--)
    //     {
    //         RoleBase oneRole = enemies[i];
    //         if (oneRole.m_CurPlanesID != Constants.PlanesID_Public &&
    //         m_CurPlanesID != oneRole.m_CurPlanesID)
    //         {
    //             enemies.RemoveAt(i);
    //         }
    //     }


    //     // 2ddl
    //     for (int i = enemies.Count - 1; i >= 0; i--)
    //     {
    //         RoleBase oneRole = enemies[i];
    //         Vector2 dir = (oneRole.GetFootPos() -
    //             this.GetFootPos()).normalized;
    //         LayerMask tLayer = m_TarLayer & (~(
    //         1 << LayerMask.NameToLayer("Plane")
    //     ));
    //         // check raycast
    //         RaycastHit2D hit = Physics2D.Raycast(
    //             this.GetAtkStartPos(), dir,
    //            m_AlertRange, tLayer);
    //         if (hit.collider != null)
    //         {
    //             if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Scene"))
    //                 enemies.RemoveAt(i);
    //         }

    //     }

    //     if (enemies.Count > 0)
    //     {
    //         return enemies[0];
    //     }
    //     return null;
    // }
    public override List<RoleBase> GetCurAtkTargetList()
    {
        List<RoleBase> enemies = new List<RoleBase>();

        foreach (RoleBase one in Global.instance.m_Hero_All)
        {
            if (one == null) continue;
            if (this == one) continue;
            if (this.gameObject.layer == one.gameObject.layer) continue;
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
    protected override void Update()
    {
        base.Update();
        // Debug.Log(m_CurStatus);
    }

    protected override void OnTrigger(Collider2D col)
    {
        if (col.transform.parent == transform) return;
        base.OnTrigger(col);
    }

}
