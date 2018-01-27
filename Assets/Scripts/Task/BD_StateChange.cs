using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

public class BD_StateChange : Action
{
    private RoleBase m_Enemy;
	public float m_MoveVec;
	
    public override void OnAwake()
    {
        m_Enemy = gameObject.GetComponentInChildren<RoleBase>();
        if (m_Enemy is Enemy || m_Enemy is Robot)
        {
          //do noting
        }
        else
        {
            Debug.LogError("This Role Is Not A Enemy Or Robot");
            return;
        }
    }
    public override void OnStart()
    {
        m_Enemy.m_MoveVec = m_MoveVec;
    }
    public override TaskStatus OnUpdate()
    {
        
        return TaskStatus.Success;
    }
    public override void OnEnd()
    {

    }


}
