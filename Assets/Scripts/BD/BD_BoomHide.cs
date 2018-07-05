using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class BD_BoomHide : Action
{
    private SmallBoom m_Role;

    public override void OnAwake()
    {
        m_Role = gameObject.GetComponent<SmallBoom>();

    }

    public override TaskStatus OnUpdate()
    {
        m_Role.Hide();
        return TaskStatus.Running;
    }
}
