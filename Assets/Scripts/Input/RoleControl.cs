using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class RoleControl : MonoBehaviour
{
    public XboxController m_Ctrler;
    public float m_MinStickDragDis = 0.2f;

    private Hero m_Role;

    void Awake()
    {
        m_Role = GetComponent<Hero>();
    }

    void Start()
    {

    }

    private void UpdateCtrl_XCI()
    {
        Vector2 vec = new Vector2();
        bool moved = false;

        // Left Stick
        if (XCI.GetButtonDown(XboxButton.LeftStick, m_Ctrler))
        {

        }

        // Right Stick
        if (XCI.GetButtonDown(XboxButton.RightStick, m_Ctrler))
        {

        }

        // Left stick movement
        float axisLX = XCI.GetAxis(XboxAxis.LeftStickX, m_Ctrler);
        float axisLY = XCI.GetAxis(XboxAxis.LeftStickY, m_Ctrler);
        vec.x = axisLX;
        vec.y = axisLY;
        if (vec.magnitude > m_MinStickDragDis)
        {
            moved = true;
            InputManager.instance.InputCommand(m_Role, CommandType.Move, vec);
        }

        // Right stick movement
        float axisRX = XCI.GetAxis(XboxAxis.RightStickX, m_Ctrler);
        float axisRY = XCI.GetAxis(XboxAxis.RightStickY, m_Ctrler);
        vec.x = axisRX;
        vec.y = axisRY;
        if (vec.magnitude > m_MinStickDragDis)
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Attack, vec);
            InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
        }


        // D-Pad
        if (XCI.GetDPad(XboxDPad.Up, m_Ctrler))
        {
            moved = true;
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.up);
        }
        if (XCI.GetDPad(XboxDPad.Down, m_Ctrler))
        {
            moved = true;
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.down);
        }
        if (XCI.GetDPad(XboxDPad.Left, m_Ctrler))
        {
            moved = true;
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.left);
        }
        if (XCI.GetDPad(XboxDPad.Right, m_Ctrler))
        {
            moved = true;
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.right);
        }

        // A,B,X,Y
        if (XCI.GetButton(XboxButton.A, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.down);
            InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
        }
        if (XCI.GetButton(XboxButton.B, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.right);
            InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
        }
        if (XCI.GetButton(XboxButton.X, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.left);
            InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
        }
        if (XCI.GetButton(XboxButton.Y, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.up);
            InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
        }

        // Trigger input
        float trigL = XCI.GetAxis(XboxAxis.LeftTrigger, m_Ctrler);
        float trigR = XCI.GetAxis(XboxAxis.RightTrigger, m_Ctrler);

        // Bumper input
        if (XCI.GetButtonDown(XboxButton.LeftBumper, m_Ctrler))
        {

        }
        if (XCI.GetButtonDown(XboxButton.RightBumper, m_Ctrler))
        {

        }

        // Start and back, same as bumpers but smaller bullets
        if (XCI.GetButtonUp(XboxButton.Back, m_Ctrler))
        {

        }
        if (XCI.GetButtonUp(XboxButton.Start, m_Ctrler))
        {

        }

        // idle
        if (!moved)
        {
            InputManager.instance.InputCommand(m_Role, CommandType.StopMove);
        }
    }
    private void UpdateCtrl_KeyBoard()
    {
        bool moved = false;
        // player 1
        if (m_Ctrler == XboxController.First)
        {
            // move
            if (Input.GetKey(KeyCode.W))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.up);
            }
            if (Input.GetKey(KeyCode.A))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.left);
            }
            if (Input.GetKey(KeyCode.S))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.down);
            }
            if (Input.GetKey(KeyCode.D))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.right);
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) ||
                Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.StopMove);
            }
            // attack
            if (Input.GetKey(KeyCode.I))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.up);
            }
            if (Input.GetKey(KeyCode.J))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.left);
            }
            if (Input.GetKey(KeyCode.K))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.down);
            }
            if (Input.GetKey(KeyCode.L))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.right);
            }
            if (Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.J) ||
                Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.L))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
            }
        }
        // player 2
        if (m_Ctrler == XboxController.Second)
        {
            // move
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.up);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.left);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.down);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moved = true;
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.right);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) ||
                Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.StopMove);
            }
            // attack
            if (Input.GetKey(KeyCode.Keypad8))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.up);
            }
            if (Input.GetKey(KeyCode.Keypad4))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.left);
            }
            if (Input.GetKey(KeyCode.Keypad5))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.down);
            }
            if (Input.GetKey(KeyCode.Keypad6))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.right);
            }
            if (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Keypad4) ||
                Input.GetKeyUp(KeyCode.Keypad5) || Input.GetKeyUp(KeyCode.Keypad6))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
            }
        }

    }

    public void UpdateCtrl()
    {
        if (XCI.GetNumPluggedCtrlrs() < 2)
        {
            this.UpdateCtrl_KeyBoard();
        }
        else
        {
            this.UpdateCtrl_XCI();
        }


    }
}
