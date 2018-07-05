using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class BD_SightPos : Action
{
    // public Transform[] targets; 
    public SharedFloat viewDistance = 7f;
    // public SharedFloat fieldOfViewAngle = 90;
	// public Transform target;
	public SharedTransform target;
    public SharedTransformList targetList;
    public bool usedEnemyAlertRange = false;
    private BehaviorTree m_BehaviorTree;
    private RoleBase m_Enemy;
    public override void OnAwake()
    {
        
        m_Enemy = GetComponent<RoleBase>();
        m_BehaviorTree = GetComponent<BehaviorTree>();

    }
    public override void OnStart()
    {
    //    if (usedEnemyAlertRange)
    //    {
    //        if (m_Enemy != null)
    //        {
    //            viewDistance.Value = m_Enemy.m_AlertRange;
    //        }
    //    }
        // targetList.Value[0] = Global.instance.m_Player_Mine.transform;
    }
   public override TaskStatus OnUpdate()
    {
        //  if (Global.instance.m_Hero_01 != null && m_Enemy.PublicGetCurAtkTarget() != null)
        //  {
        //      float distance = Vector3.Distance(m_Enemy.PublicGetCurAtkTarget().transform.position ,transform.position);
        //      if ( distance < viewDistance.Value)
        //      {
        //         //  m_BehaviorTree.SetVariableValue("target",targetList.Value[0]);
        //         m_BehaviorTree.SetVariableValue("target",m_Enemy.PublicGetCurAtkTarget().transform);
        //          return TaskStatus.Success;
        //      }
        //  }
        // return TaskStatus.Failure;
		m_BehaviorTree.SetVariableValue("staticPoint",m_Enemy.gameObject.transform.position);
		return TaskStatus.Success;
    }
    public override void OnEnd()
    {

    }
}
