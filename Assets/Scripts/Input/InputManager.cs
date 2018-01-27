using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void DelUIButton(Hero player);

public enum PlatformType
{
    EditorAndWIN = 0,
    Android = 1,
    iOS = 2
};

public enum OperateMode
{
    NULL = -1,

    AutoFindEnemies = 0,
    ManualAtk = 1,
    ManualAtk_AuxAim = 2,
    AutoFindEnemies_RollTarget_JS = 3,
    SIZE
};

public enum InputType
{
    NULL = -1,

    Touch_Move = 0,
    Touch_Up,
    SelCard,

    SIZE
};

public enum CommandType
{
    NULL = -1,

    Move = 0,
    StopMove = 1,
    Attack = 2,
    StopAttack = 3,
    RollWeapon = 4,
    CastAttack = 5,
    RollAtkTarget = 6,

    SIZE
};

public class InputManager : MonoBehaviour
{
    public event DelUIButton OnButtonDown;
    public event DelUIButton OnButtonLeft;
    [HideInInspector]
    public static InputManager instance { get; private set; }
    public OperateMode m_CurOpaMode = OperateMode.NULL;
    public float fixAngle = 20f;
    public Canvas m_Canvas;

    public InputType m_CurInputType = InputType.NULL;
    public float m_MinInputInterval = 0.2f;

    [HideInInspector]
    public List<RoleControl> m_AllCtrls = new List<RoleControl>();
    [HideInInspector]
    public PlatformType m_PlatformType = PlatformType.EditorAndWIN;

    private Dictionary<CommandType, float> m_CommandTime =
        new Dictionary<CommandType, float>();
    private IEnumerator m_IE_AtkBtnDown;
    private int temp_TargetIndex = 0;
    void Awake()
    {
        m_IE_AtkBtnDown = null;
    }

    protected void OnEnable()
    {
        // DontDestroyOnLoad(gameObject);

        if (InputManager.instance == null)
        {
            InputManager.instance = this;
        }


#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        m_PlatformType = PlatformType.EditorAndWIN;
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        m_PlatformType = PlatformType.Android;
#endif

#if !UNITY_EDITOR && UNITY_IPHONE
        m_PlatformType = PlatformType.iOS;
#endif

    }

    void Start()
    {
        // this.SetAtkBtnCallbacks();
        m_AllCtrls.Clear();
        foreach (Hero one in Global.instance.m_Hero_All)
        {
            RoleControl ctrl = one.GetComponent<RoleControl>();
            m_AllCtrls.Add(ctrl);
        }
    }

    public void InitWeaponUIText()
    {
        Transform weaponTextTransfrom = m_Canvas.transform.Find("Node_RB/Btn_RollWeapon/Text");
        Text weaponText = weaponTextTransfrom.GetComponent<Text>();
        Weapon curWeapon = Global.instance.m_Hero_01.m_CurWeapon;
        weaponText.text = curWeapon.m_WeaponName;
    }
    public void SetType(InputType _type)
    {
        if (m_CurInputType != _type)
        {
            m_CurInputType = _type;
        }
    }

    public bool CheckType(InputType _type)
    {
        return m_CurInputType == _type;
    }

    public void ChangeOperation(Int32 val)
    {
        OperateMode modeVal = (OperateMode)val;
        m_CurOpaMode = modeVal;

        if (Global.instance.m_Hero_01 != null)
        {
            Global.instance.m_Hero_01.UpdateOpaMode(m_CurOpaMode);
        }
    }

    void Update()
    {
        this.UpdateOpaMode();
        foreach (RoleControl one in m_AllCtrls)
        {   
            one.UpdateCtrl();
        }
    }

    public void UpdateOpaMode()
    {
        // switch (m_CurOpaMode)
        // {
        //     case OperateMode.AutoFindEnemies:
        //         {
        //             m_JoyGO_Atk.SetActive(false);
        //             m_Btn_Atk.SetActive(true);
        //             m_Btn_RollTarget.SetActive(true);

        //         }
        //         break;

        //     case OperateMode.ManualAtk:
        //         {
        //             m_JoyGO_Atk.SetActive(true);
        //             m_Btn_Atk.SetActive(false);
        //             m_Btn_RollTarget.SetActive(false);

        //         }
        //         break;
        //     case OperateMode.ManualAtk_AuxAim:
        //         {
        //             m_JoyGO_Atk.SetActive(true);
        //             m_Btn_Atk.SetActive(false);
        //             m_Btn_RollTarget.SetActive(false);

        //         }
        //         break;
        //     case OperateMode.AutoFindEnemies_RollTarget_JS:
        //         {
        //             m_JoyGO_Atk.SetActive(true);
        //             m_Btn_Atk.SetActive(false);
        //             m_Btn_RollTarget.SetActive(false);

        //         }
        //         break;

        //     default:
        //         {
        //             m_JoyGO_Atk.SetActive(false);
        //             m_Btn_Atk.SetActive(false);
        //         }
        //         break;
        // }
    }

    private void SetCommandTime(CommandType cType)
    {
        if (m_CommandTime.ContainsKey(cType))
        {
            m_CommandTime[cType] = Time.time;
        }
        else
        {
            m_CommandTime.Add(cType, Time.time);
        }
    }
    private bool CheckCommandTime(CommandType cType)
    {
        if (m_CommandTime.ContainsKey(cType))
        {
            return Time.time > m_CommandTime[cType] + m_MinInputInterval;
        }
        else
        {
            return true;
        }
    }

    public void InputCommand(Hero player, CommandType cType, params object[] vals)
    {
        if (!Global.instance.m_GameStart) return;

        // Hero player = Global.instance.m_Hero_01;
        if (player == null) return;
        if (player.m_CurStatus == RoleStatus.Die) return;

        // check time
        // if (this.CheckCommandTime(cType))
        // {
        //     this.SetCommandTime(cType);
        // }
        // else
        // {
        //     return;
        // }

        switch (cType)
        {
            case CommandType.Move:
                {
                    Debug.Assert(vals.Length > 0, "Check vals !!!");
                    Vector2 moveVal = (Vector2)vals[0];
                    player.Move(moveVal);
                }
                break;

            case CommandType.StopMove:
                {
                    player.Idle();
                }
                break;

            case CommandType.Attack:
                {
                    if (vals.Length > 0)
                    {
                        Vector2 moveVal = (Vector2)vals[0];
                        if (m_CurOpaMode == OperateMode.ManualAtk_AuxAim)
                            player.SetCurVec_Atk(AtkVecFix_M_AutoAim(moveVal) *
                                player.m_CurWeapon.m_AlertRange);
                        if (m_CurOpaMode == OperateMode.ManualAtk)
                            player.SetCurVec_Atk(moveVal *
                                player.m_CurWeapon.m_AlertRange);
                        if (m_CurOpaMode == OperateMode.AutoFindEnemies_RollTarget_JS)
                        {      
                            player.RollAtkTarget(player.GetCurAtkTarget(player.GetCurAtkTargetList(), TargetsSortType.Angle, moveVal));
                        }
                    }



                    if (m_CurOpaMode == OperateMode.ManualAtk && player.m_CurWeapon.m_AimType == AimArrowType.ArrowNCircle
                    || m_CurOpaMode == OperateMode.ManualAtk_AuxAim && player.m_CurWeapon.m_AimType == AimArrowType.ArrowNCircle)
                    {
                        player.m_PrepareChangeParticle.SetActive(true);
                    }
                    else
                    {
                        player.Attack();
                    }
                    if (m_CurOpaMode != OperateMode.AutoFindEnemies)
                        OnButtonDown(player);

                }
                break;

            case CommandType.StopAttack:
                {
                    player.StopAttack();
                }
                break;

            case CommandType.RollWeapon:
                {
                    player.RollWeapon();
                }
                break;
            case CommandType.CastAttack:
                {
                    player.m_PrepareChangeParticle.SetActive(false);
                    // AimArrow.instance.gameObject.SetActive(false);
                    OnButtonLeft(player);
                    if (player.m_CurWeapon.m_AimType == AimArrowType.ArrowNCircle)
                    {
                        if (m_CurOpaMode == OperateMode.ManualAtk_AuxAim ||
                            m_CurOpaMode == OperateMode.ManualAtk)
                        {
                            player.Attack();
                        }

                    }

                }
                break;
            case CommandType.RollAtkTarget:
                {

                    if (player.GetCurAtkTargetList() == null)
                    {
                        Debug.LogError("CurAtkTargets Is NULL !!!");
                        return;
                    }
                    if (player.GetCurAtkTargetList().Count == 0)
                    {
                        Debug.LogError("CurAtkTargets Count Is Zero !!!");
                        return;
                    }
                    if (temp_TargetIndex >= player.GetCurAtkTargetList().Count)
                    {
                        temp_TargetIndex = 0;
                    }
                    if (player.CurAtkTar == player.GetCurAtkTargetList()[temp_TargetIndex])
                    {
                        temp_TargetIndex++;

                    }
                    player.RollAtkTarget(temp_TargetIndex);

                }
                break;

            default:
                break;
        }
    }

    private void SetAtkBtnCallbacks()
    {
        // Button atkBtn = m_Btn_Atk.GetComponent<Button>();
        // UIEventListener btnListener = atkBtn.gameObject.AddComponent<UIEventListener>();

        // btnListener.OnDown += delegate (GameObject gb)
        // {
        //     if (m_IE_AtkBtnDown == null)
        //     {
        //         m_IE_AtkBtnDown = IE_AtkBtnDown();
        //         StartCoroutine(m_IE_AtkBtnDown);
        //     }
        // };

        // btnListener.OnUp += delegate (GameObject gb)
        // {
        //     if (m_IE_AtkBtnDown != null)
        //     {
        //         StopCoroutine(m_IE_AtkBtnDown);
        //         m_IE_AtkBtnDown = null;

        //         // stop atk
        //         this.InputCommand(Global.instance.m_Hero_01,
        //         CommandType.StopAttack);
        //     }
        // };
    }

    private IEnumerator IE_AtkBtnDown()
    {
        while (true)
        {
            this.InputCommand(Global.instance.m_Hero_01,
                CommandType.Attack);

            yield return new WaitForSeconds(m_MinInputInterval);
        }
    }

    public void RollWeapon(Text weaponText)
    {
        this.InputCommand(Global.instance.m_Hero_01,
                CommandType.RollWeapon);

        if (Global.instance.m_Hero_01 == null) return;
        Weapon curWeapon = Global.instance.m_Hero_01.m_CurWeapon;
        weaponText.text = curWeapon.m_WeaponName;
    }
    public void RollTarget()
    {
        this.InputCommand(Global.instance.m_Hero_01,
                CommandType.RollAtkTarget);
    }

    public Vector2 AtkVecFix_M_AutoAim(Vector2 moveVal)
    {
        List<RoleBase> curTars = Global.instance.m_Hero_01.GetCurAtkTargetList();
        Vector2 result = moveVal;
        if (curTars != null)
        {
            float mixAngle = 360;
            // float targetdis = 0;
            for (int i = 0; i < curTars.Count; i++)
            {
                Vector3 autoVec = (curTars[i].GetMidPos() -
                    Global.instance.m_Hero_01.GetMidPos()).normalized;

                float angle = Vector3.Angle(autoVec, moveVal);
                if (angle <= fixAngle && angle <= mixAngle)
                {
                    result = autoVec;
                    mixAngle = angle;
                }
            }

        }
        return result;
    }
}
