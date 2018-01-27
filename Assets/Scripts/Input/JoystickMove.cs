using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickMove : MonoBehaviour
{
    
    public void Move(Vector2 move)
    {
        InputManager.instance.InputCommand(Global.instance.m_Hero_01, CommandType.Move, move);
    }

    public void MoveEnd()
    {
        InputManager.instance.InputCommand(Global.instance.m_Hero_01, CommandType.StopMove);
    }

    public void Attack(Vector2 move)
    {
        InputManager.instance.InputCommand(Global.instance.m_Hero_01, CommandType.Attack, move);
      
    }

    public void AttackEnd()
    {

        InputManager.instance.InputCommand(Global.instance.m_Hero_01, CommandType.StopAttack);

    }
    public void Attack()
    {
        
       InputManager.instance.InputCommand(Global.instance.m_Hero_01, CommandType.CastAttack);
    }
}
