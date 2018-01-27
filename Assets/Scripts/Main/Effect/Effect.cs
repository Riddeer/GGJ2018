using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum EffectPlayType
{
    NULL = -1,

    Bezier = 0,
    Boomerang = 1,
    Hold = 2,
    Charge = 3,
    Stomp = 4,
    DontDestroyOnTime = 5,
    Trap = 6,
    Delay = 7,

    SIZE
};

public class Effect : MonoBehaviour
{
    public EffectPlayType m_PlayType = EffectPlayType.NULL;

    public float m_DestroyDelay = 0f;
    public float m_MoveVec = 1f;
    public float m_BezierCtrlHight = 0.3f;
    public GameObject m_BezierEndParticle;
    public List<ParticleSystem> m_StartRotationPars = new List<ParticleSystem>();

    [HideInInspector]
    public RoleBase m_Creater;

    [HideInInspector]
    public Vector3 m_TarVec = Vector3.zero;

    private Vector3 m_StartPos = Vector3.zero;
    private Vector3 m_TarPos = Vector3.zero;
    private Vector3 m_EndPos = Vector3.zero;
    private float m_Time = 0f;
    private Vector3 m_CurPos = Vector3.zero;
    private bool temp_BoomerangTurnTarget = false;
    private bool temp_Move = true;
    private Hero m_Player;
    private ParticleSystem m_ParticleSystem;

    public void Init(RoleBase creater, Vector3 tarVec, RoleBase tarRole)
    {
        m_Creater = creater;

        m_TarVec = tarVec;

        if (InputManager.instance.m_CurOpaMode == OperateMode.AutoFindEnemies || InputManager.instance.m_CurOpaMode == OperateMode.AutoFindEnemies_RollTarget_JS)
        {
            if (tarRole != null)
            {
                m_TarPos = tarRole.transform.position;
                // if (m_Creater is Hero)
                //     Debug.Log("M_TarRole: " + tarRole.name);
            }
            else
            {
                float dis = creater.m_AtkRange;
                m_TarPos = transform.position + dis * m_TarVec.normalized;
            }

        }
        else if (InputManager.instance.m_CurOpaMode == OperateMode.ManualAtk || InputManager.instance.m_CurOpaMode == OperateMode.ManualAtk_AuxAim)
        {
            m_TarPos = (Vector3)m_Creater.GetCurFaceVec() + m_Creater.GetMidPos() + new Vector3(0, 0.4f, 0);
        }
        m_Player = Global.instance.m_Hero_01;
        m_StartPos = transform.position;
        m_EndPos = m_StartPos;
        m_Time = 0f;

    }

    void Awake()
    {
        m_ParticleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    void Start()
    {

        switch (m_PlayType)
        {
            case EffectPlayType.Bezier:
                {
                    Damage dmg = gameObject.GetComponent<Damage>();
                    if (dmg != null)
                    {
                        dmg.enabled = false;
                    }
                }
                break;
            case EffectPlayType.Boomerang:
                {
                    Damage dmg = gameObject.GetComponent<Damage>();
                    m_ParticleSystem = gameObject.GetComponent<ParticleSystem>();
                    if (dmg != null)
                    {
                        dmg.enabled = false;
                    }
                }
                break;
            case EffectPlayType.Stomp:
                {
                    m_StartPos = m_Player.GetFootPos();
                    transform.position = m_StartPos;
                }
                break;

            case EffectPlayType.Hold:
            case EffectPlayType.Charge:
                {

                }
                break;
            case EffectPlayType.DontDestroyOnTime:
                {

                }
                break;
            case EffectPlayType.Trap:
                {
                    Invoke("DestroySelf", m_DestroyDelay);
                }
                break;
            case EffectPlayType.Delay:
                {
                    GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            default:
                {

                    this.UpdateRotation(m_TarVec);
                    Invoke("DestroySelf", m_DestroyDelay);
                }
                break;
        }
    }

    public void Play()
    {
        if (m_ParticleSystem == null) return;

        if (!m_ParticleSystem.isPlaying)
        {
            m_ParticleSystem.Play();

            switch (m_PlayType)
            {
                case EffectPlayType.Charge:
                    {
                        if (!IsInvoking("DestroySelf"))
                        {
                            Invoke("DestroySelf", m_DestroyDelay);
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public void UpdateRotation(Vector3 tarVec)
    {
        // is have particle system
        if (GetComponent<ParticleSystem>() == null)
            return;

        // keep rotating
        float angle = Vector2.Angle(tarVec, Vector2.right);
        if (tarVec.y > 0) angle = -angle;

        transform.localEulerAngles = new Vector3(angle, 90f, 0f);

        foreach (ParticleSystem oneP in m_StartRotationPars)
        {
            oneP.startRotation = angle * Mathf.PI / 180f;
        }
    }

    public void SetChargedValue(float val)// 0 1
    {

    }

    void FixedUpdate()
    {
        m_Time += Time.deltaTime;

        switch (m_PlayType)
        {
            case EffectPlayType.Bezier:
                {
                    if (Vector3.Distance(m_TarPos, transform.position)
                        < m_MoveVec * Time.deltaTime)
                    {
                        if (!IsInvoking("DestroySelf"))
                        {
                            Invoke("DestroySelf", m_DestroyDelay);

                            // enable damage
                            Damage dmg = gameObject.GetComponent<Damage>();
                            if (dmg != null)
                            {
                                dmg.enabled = true;
                            }
                            // play the particle of explosion
                            if (m_BezierEndParticle != null)
                            {
                                ParticleManager.instance.CreateParticle(
                                    m_BezierEndParticle,
                                    m_TarPos, Vector3.zero
                                );
                                m_ParticleSystem.Stop();
                                AudioManager.instance.Play("Explosion_FireBall_01");
                            }
                        }
                    }

                    // bezier
                    Bezier myBezier = new Bezier(m_StartPos,
                        new Vector3(0f, m_BezierCtrlHight, 0f),
                        new Vector3(0f, m_BezierCtrlHight, 0f), m_TarPos);
                    float allTime = Vector3.Distance(m_StartPos, m_TarPos) / m_MoveVec;
                    float temp = Mathf.Clamp(m_Time / allTime, 0f, 1f);

                    // pow 2
                    // temp = m_Time / (Mathf.Pow(allTime, 2));
                    // temp = Mathf.Pow(temp, 3);

                    m_CurPos = myBezier.GetPointAtTime(temp);
                    transform.position = m_CurPos;

                    // rotate
                    // transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime);
                }
                break;
            case EffectPlayType.Boomerang:
                {
                    if (!IsInvoking("DestroySelf"))
                    {
                        Invoke("DestroySelf", m_DestroyDelay);

                        // enable damage
                        Damage dmg = gameObject.GetComponent<Damage>();
                        if (dmg != null)
                        {
                            dmg.enabled = true;
                        }
                    }
                    if (Vector3.Distance(m_TarPos, transform.position)
                        < m_MoveVec * Time.deltaTime)
                    {
                        temp_BoomerangTurnTarget = true;
                    }
                    if (Vector3.Distance(m_StartPos, transform.position)
                            < Vector3.Distance(m_TarPos, transform.position) * 1.3 && temp_BoomerangTurnTarget == true)
                    {
                        if (m_Creater != null)
                        {
                            m_EndPos = m_Creater.GetFootPos();

                        }
                        else
                        {
                            m_EndPos = m_StartPos;
                        }
                        temp_Move = false;

                    }
                    if (Vector3.Distance(m_EndPos, transform.position)
                            < m_MoveVec * Time.deltaTime * 2 && temp_BoomerangTurnTarget == true)
                    {
                        Destroy(gameObject);
                    }
                    if (temp_Move)
                    { BoomerangBezier(m_TarPos, m_EndPos); }
                    if (!temp_Move)
                        ComeBakeMoveBack();
                }
                break;

            case EffectPlayType.Hold:
            case EffectPlayType.Charge:
                {
                    // keep with creater
                    if (m_Creater.m_CurStatus == RoleStatus.Die)
                    {
                        Destroy(gameObject);
                        return;
                    }

                    transform.position = m_Creater.GetMidPos();
                }
                break;

            default:
                break;
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    private Vector3[] CalcBezierControlPoint(Vector3 tarPos)
    {
        float dis = Vector3.Distance(m_StartPos, tarPos);
        float standardControlPointHigh = dis / 2f / 0.375f;
        float width = dis * 1.25f;
        Vector3[] controlPoints = new[] {new Vector3(-width,standardControlPointHigh,0f)
        ,new Vector3 (width,standardControlPointHigh,0f)};

        Vector3 dir = tarPos - m_StartPos;
        dir = dir.normalized;
        float angle = Vector3.Angle(Vector3.up, dir);
        if (tarPos.x - m_StartPos.x < 0f)
            angle = 360f - angle;
        float deg = (angle * Mathf.PI) / 180f;
        Vector3 temp = Vector3.zero;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            temp.y = (controlPoints[i].y - Vector3.zero.y) * Mathf.Cos(deg) - (controlPoints[i].x - Vector3.zero.x) * Mathf.Sin(deg) + Vector3.zero.y;
            temp.x = (controlPoints[i].y - Vector3.zero.y) * Mathf.Sin(deg) + (controlPoints[i].x - Vector3.zero.x) * Mathf.Cos(deg) + Vector3.zero.x;
            controlPoints[i] = temp;
        }
        return controlPoints;
    }
    private void BoomerangBezier(Vector3 target, Vector3 endPos)
    {
        // bezier
        Bezier myBezier = new Bezier(m_StartPos,
            CalcBezierControlPoint(target)[0],
            CalcBezierControlPoint(target)[1], endPos);
        float allTime = Vector3.Distance(m_StartPos, target) / m_MoveVec;
        float temp = Mathf.Clamp(m_Time / allTime, 0f, 1f);
        Vector3 curPos = myBezier.GetPointAtTime(temp);
        // Debug.DrawLine(transform.position, curPos, Color.red, 2f);
        transform.position = curPos;
    }
    private void ComeBakeMoveBack()
    {
        Vector3 dir = m_EndPos - transform.position;
        dir = dir.normalized;
        // Debug.DrawLine(transform.position, m_Player.GetFootPos(), Color.red, 2f);
        transform.Translate(dir * Time.deltaTime * m_MoveVec * 2.2f);
    }


}
