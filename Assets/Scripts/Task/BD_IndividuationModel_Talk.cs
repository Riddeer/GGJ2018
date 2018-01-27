using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;


public class BD_IndividuationModel_Talk : Action
{
    private RoleBase m_RoleBase;
	public string content = "";
	public float duraTime = 1f;
    public override void OnAwake()
    {
        
    }
    public override void OnStart()
    {
		m_RoleBase = gameObject.GetComponentInChildren<RoleBase>();
		m_RoleBase.Talk(content, duraTime);
    }
    public override TaskStatus OnUpdate()
    {
		return TaskStatus.Success ;
    }
    public override void OnEnd()
    {

    }


}
