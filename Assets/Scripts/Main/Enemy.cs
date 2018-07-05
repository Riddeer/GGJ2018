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
        
        EnemyManager.instance.m_Enemies.Add(this);
    }
    public override void RemoveSelf()
    {
        EnemyManager.instance.RemoveEnemy(this);
        // base.RemoveSelf();
    }
    
    public override List<RoleBase> GetCurAtkTargetList()
    {
        List<RoleBase> enemies = new List<RoleBase>();

        foreach (RoleBase one in Global.instance.m_Hero_All)
        {
            if (one == null) continue;
            if (this == one) continue;
            if (this.gameObject.layer == one.gameObject.layer) continue;
            if (Vector3.Distance(this.GetFootPos(), one.GetFootPos()) <
            m_AlertRange
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

    public override void Die()
    {

        m_CurStatus = RoleStatus.Die;

        // Audio play
        float pitch = 0.8f + UnityEngine.Random.Range(-0.3f, 0.3f);

        AudioManager.instance.Play(Get_Death_AudioName(), pitch);
        CameraEffect.instance.Shake(0.2f, 1f, 0.1f);

        this.RemoveSelf();
    }

}
