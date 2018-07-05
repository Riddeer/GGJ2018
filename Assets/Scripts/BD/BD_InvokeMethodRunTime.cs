using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;


public class BD_InvokeMethodRunTime : Action
{
 
    [BehaviorDesigner.Runtime.Tasks.Tooltip("MethodName")]
    public SharedString methodName;
    public override void OnAwake()
    {
        
    }
    public override void OnStart()
    {
		gameObject.BroadcastMessage(methodName.Value);
    }
    public override TaskStatus OnUpdate()
    {
		
        return TaskStatus.Success ;
    }
    public override void OnEnd()
    {

    }


}
