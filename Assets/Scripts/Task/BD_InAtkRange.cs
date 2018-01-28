using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class BD_InAtkRange : Conditional
{  
    private float atkDistance = 1f;
	public SharedTransform target;
	private RoleBase m_Enemy;
	public bool isRobot = false;
    public override void OnAwake()
    {
       	m_Enemy = GetComponent<RoleBase>();
     
    }
    public override void OnStart()
    {
        if (isRobot == true)
        {
            atkDistance = m_Enemy.m_AlertRange;
        }
    }
   public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
            return TaskStatus.Failure;    
        float distance = Vector3.Distance(target.Value.position ,transform.position);
        
        if (distance < atkDistance)
        {
           return TaskStatus.Success;
        }
        return TaskStatus.Failure;
 
    }
    public override void OnEnd()
    {

    }
}
