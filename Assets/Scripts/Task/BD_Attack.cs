using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BD_Attack : Action
{
    private RoleBase m_Enemy;
    private float m_AtkInterval;
    private float temp;
    public bool isRobot = false;
    public override void OnAwake()
    {
        m_Enemy = gameObject.GetComponentInChildren<RoleBase>();
      
    }
    public override void OnStart()
    {
        m_AtkInterval = m_Enemy.m_AtkInterval;
        temp = -m_AtkInterval;
    }
    public override TaskStatus OnUpdate()
    {
        if (isRobot)
        {
           m_Enemy.Attack();
        }
        else if (Time.time > (temp + m_AtkInterval))
        {
            m_Enemy.Attack();

            temp = Time.time;
        }

        return TaskStatus.Running;
    }
    public override void OnEnd()
    {

    }


}
