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
            InputManager.instance.InputCommand(m_Role, CommandType.Move, vec);
        }
        else
        {
            InputManager.instance.InputCommand(m_Role, CommandType.StopMove);
        }

        // Right stick movement
        float axisRX = XCI.GetAxis(XboxAxis.RightStickX, m_Ctrler);
        float axisRY = XCI.GetAxis(XboxAxis.RightStickY, m_Ctrler);
        vec.x = axisRX;
        vec.y = axisRY;
        if (vec.magnitude > m_MinStickDragDis)
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Attack, vec);
        }
        else
        {
            InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
            InputManager.instance.InputCommand(m_Role, CommandType.CastAttack);
        }

        // D-Pad
        if (XCI.GetDPad(XboxDPad.Up, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.up);
        }
        if (XCI.GetDPad(XboxDPad.Down, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.down);
        }
        if (XCI.GetDPad(XboxDPad.Left, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.left);
        }
        if (XCI.GetDPad(XboxDPad.Right, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.right);
        }
        if (XCI.GetDPadUp(XboxDPad.Up, m_Ctrler) || XCI.GetDPadUp(XboxDPad.Down, m_Ctrler) ||
        XCI.GetDPadUp(XboxDPad.Left, m_Ctrler) || XCI.GetDPadUp(XboxDPad.Right, m_Ctrler))
        {
            InputManager.instance.InputCommand(m_Role, CommandType.StopMove);
        }

        // A,B,X,Y
        if (XCI.GetButton(XboxButton.A, m_Ctrler))
        {
            // InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.down);
            m_Role.EnsureTransmission();
        }
        // if (XCI.GetButton(XboxButton.B, m_Ctrler))
        // {
        //     InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.right);
        // }
        // if (XCI.GetButton(XboxButton.X, m_Ctrler))
        // {
        //     InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.left);
        // }
        // if (XCI.GetButton(XboxButton.Y, m_Ctrler))
        // {
        //     InputManager.instance.InputCommand(m_Role, CommandType.Attack, Vector2.up);
        // }
        // if (XCI.GetButtonUp(XboxButton.A, m_Ctrler) || XCI.GetButtonUp(XboxButton.B, m_Ctrler) ||
        // XCI.GetButtonUp(XboxButton.X, m_Ctrler) || XCI.GetButtonUp(XboxButton.Y, m_Ctrler))
        // {
        //     InputManager.instance.InputCommand(m_Role, CommandType.StopAttack);
        //     InputManager.instance.InputCommand(m_Role, CommandType.CastAttack);
        // }

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
    }
    private void UpdateCtrl_KeyBoard()
    {
        // player 1
        if (m_Ctrler == XboxController.First)
        {
            // move
            if (Input.GetKey(KeyCode.W))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.up);
            }
            if (Input.GetKey(KeyCode.A))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.left);
            }
            if (Input.GetKey(KeyCode.S))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.down);
            }
            if (Input.GetKey(KeyCode.D))
            {
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
                InputManager.instance.InputCommand(m_Role, CommandType.CastAttack);
            }
            // transmission
            if (Input.GetKey(KeyCode.H))
            {
                m_Role.EnsureTransmission();
            }
        }
        // player 2
        if (m_Ctrler == XboxController.Second)
        {
            // move
            if (Input.GetKey(KeyCode.UpArrow))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.up);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.left);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                InputManager.instance.InputCommand(m_Role, CommandType.Move, Vector2.down);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
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
                InputManager.instance.InputCommand(m_Role, CommandType.CastAttack);
            }
            // transmission
            if (Input.GetKey(KeyCode.Keypad0))
            {
                m_Role.EnsureTransmission();
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
