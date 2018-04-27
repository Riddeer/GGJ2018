using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

public enum RoleStatus
{
    NULL = -1,

    Die = 0,
    Idle = 1,
    Move = 2,
    Attack = 3,
    Atked = 4,
    Transmission = 5,

    SIZE
};

public enum RoleBaseType
{
    NULL = -1,

    Castle = 0,
    Land = 1,
    Air = 2,
    OnCastle = 3,
    Skill = 4,

    SIZE
};
public enum TargetsSortType
{
    NULL = -1,
    Distance = 0,
    Angle = 1,
    SIZE
}
public abstract class RoleBase : MonoBehaviour
{

    public RoleBaseType m_BaseType = RoleBaseType.NULL;
    public float m_MoveVec = 1f;
    public long m_MaxHP = 100;
    public float m_AlertRange = 10f;
    public float m_AtkRange = 5f;
    public float m_AtkInterval = 0.5f;
    public float m_HurtProtectTime = 0.5f;
    public float m_BeAtkWidth = 0;
    public Slider m_HPSlider;
    public List<GameObject> m_Dmgs;
    public Transform m_MidPos = null;
    public Transform m_AtkStartPos = null;
    public Transform m_ParticlePos_Dust;
    public GameObject m_Particle_Dust;
    public GameObject m_PrepareChangeParticle;
    public GameObject m_Shield;
    public LayerMask m_TarLayer;
    public GameObject m_Shadow;
    public string behaviorDesignerPath = "BehaviorTree/NormalAI";
    public Text m_TalkText;

    [HideInInspector]
    public Weapon m_CurWeapon;

    [HideInInspector]
    public bool m_Invincible = false;
    [HideInInspector]
    public long m_CurHP = 0;
    // [HideInInspector]
    public RoleStatus m_CurStatus = RoleStatus.NULL;
    [HideInInspector]
    public int m_CurPlanesID = 0;
    [HideInInspector]
    public NavMeshAgent m_Agent;

    protected RoleAnimation m_AniMng;
    protected Dictionary<string, GameObject> m_DmgsDic =
        new Dictionary<string, GameObject>();
    protected IEnumerator m_IE_Attack;
    protected bool m_Flip = false;
    protected GameObject m_CurAtkGO = null;
    protected RoleBase m_CurAtkTar = null;
    protected Vector2 m_CurVec_AtkTar = Vector2.zero;
    protected Vector2 m_CurVec_Move = Vector2.zero;
    protected Canvas m_Canvas;
    protected BehaviorTree m_BehaviorTree = null;

    private IEnumerator m_IE_StopInvicible;
    readonly float m_DustPlayInterval = 0.2f;
    private float m_NextDustPlayTime = 0;
    private Vector2 tempJoyStickVec = Vector2.zero;
    private IEnumerator m_IE_MoveTo = null;
    private IEnumerator m_IE_Talk = null;
    private float m_EffectiveVal = 1f;

    public RoleBase CurAtkTar
    {
        get { return m_CurAtkTar; }
        set { m_CurAtkTar = value; }
    }
    public Vector2 M_CurVec_Move
    {
        get { return m_CurVec_Move; }
        set { m_CurVec_Move = value; }
    }
    protected virtual void Awake()
    {
        transform.SetParent(Global.instance.m_BattleGround);

        this.SetLayer();

        m_AniMng = GetComponent<RoleAnimation>();
        m_Canvas = GetComponentInChildren<Canvas>();

        m_IE_Attack = null;
        m_Invincible = false;// test
        m_IE_StopInvicible = null;
        m_CurVec_Move = Vector2.right;
        m_NextDustPlayTime = 0;
        m_IE_MoveTo = null;
        if (m_TalkText) m_TalkText.text = "";
        m_EffectiveVal = 1f;

        // create dic
        this.CreateDmgDic();
        this.InitNavNBehaviorTree();
    }

    public void SetLayer()
    {
        if (this is Hero || this is Robot)
        {
            // Neutral
            gameObject.layer = LayerMask.NameToLayer("Neutral");

            m_TarLayer = 1 << LayerMask.NameToLayer("Enemy")
                | 1 << LayerMask.NameToLayer("Alliance")
                | 1 << LayerMask.NameToLayer("Scene")
                | 1 << LayerMask.NameToLayer("Plane");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Alliance");
            // set m_TarLayer : Scene + Zhongli + Difang   enemy neutral
            m_TarLayer = 1 << LayerMask.NameToLayer("Enemy")
                | 1 << LayerMask.NameToLayer("Neutral")
                | 1 << LayerMask.NameToLayer("Scene")
                | 1 << LayerMask.NameToLayer("Plane");
        }
    }

    public void CreateDmgDic()
    {
        bool recreate = false;

        if (m_Dmgs.Count != m_DmgsDic.Count)
        {
            recreate = true;
        }
        else
        {
            foreach (GameObject oneGO in m_Dmgs)
            {
                if (!m_DmgsDic.ContainsValue(oneGO))
                {
                    recreate = true;
                    break;
                }
            }
        }

        if (recreate)
        {
            m_DmgsDic.Clear();
            foreach (GameObject one in m_Dmgs)
            {
                Damage dmg = one.GetComponent<Damage>();

                m_DmgsDic.Add(dmg.m_DmgName, one);
            }
        }

    }

    protected virtual void Start()
    {
        m_CurStatus = RoleStatus.Idle;
        m_CurHP = m_MaxHP;
        this.UpdateHPShow();
        m_CurPlanesID = Constants.PlanesID_Public;

    }

    protected virtual void Update()
    {
        // if (m_Shield != null)
        // {
        //     // Shield.transform.position 
        //     //  = this.GetMidPos() + new Vector3(0f,0.1f,-0.1f);
        // }
        this.UpdateFlip();
    }


    protected virtual void FixedUpdate()
    {

    }
    protected virtual void LateUpdate()
    {

    }

    protected virtual void UpdateAtkTarget()
    {
        if (m_CurAtkTar != null)
        {
            // m_CurVec_AtkTar = (m_CurAtkTar.GetFootPos() -
            //     this.GetFootPos()).normalized;
            this.SetCurVec_Atk(m_CurAtkTar.GetFootPos() -
                this.GetFootPos());
        }
        else
        {
            // m_CurVec_AtkTar = Vector2.zero;
            this.SetCurVec_Atk(Vector2.zero);
        }
    }

    protected virtual void UpdateFlip()
    {
        if (m_CurStatus == RoleStatus.Die) return;
        Vector2 curFaceVec = this.GetCurFaceVec();
        if (curFaceVec.x < 0)
        {
            this.SetFlipX(true);
        }
        else if (curFaceVec.x > 0)
        {
            this.SetFlipX(false);
        }
    }

    // protected virtual RoleBase GetCurAtkTarget()
    // {

    //     return null;
    // }
    public virtual List<RoleBase> GetCurAtkTargetList()
    {
        return null;
    }
    public virtual RoleBase GetCurAtkTarget(List<RoleBase> targets, TargetsSortType sortType, params object[] vals)
    {
        if (targets == null)
        {
            if (m_CurAtkTar != null)
            {
                Debug.Log(m_CurAtkTar.name);
            }
            else
            {
                Debug.LogError("M_CurTarget Is NULL! ");
            }
            return null;
        }
        if (targets.Count == 0)
        {
            return null;
        }


        switch (sortType)
        {
            case TargetsSortType.Distance:
                {
                    // sort by  distance
                    targets.Sort(
                        delegate (RoleBase x, RoleBase y)
                        {
                            if (Vector3.Distance(this.GetFootPos(), x.GetFootPos()) <
                                    Vector3.Distance(this.GetFootPos(), y.GetFootPos()))
                            {
                                return -1;
                            }
                            else if (Vector3.Distance(this.GetFootPos(), x.GetFootPos()) >
                                Vector3.Distance(this.GetFootPos(), y.GetFootPos()))
                            {
                                return 1;
                            }

                            return 0;
                        });

                    if (targets.Count == 0)
                    {
                        Debug.LogError("Targets Count Is Zero !!!");
                        return null;
                    }
                    else
                    {
                        return targets[0];
                    }


                }
            case TargetsSortType.Angle:
                {
                    if (vals == null)
                    {
                        Debug.LogError("JoyStick Mov val is NULL !!!");
                        return m_CurAtkTar;
                    }

                    Vector2 mov = (Vector2)vals[0];
                    if (Vector3.Angle(mov, tempJoyStickVec) < 15f)
                    {
                        return m_CurAtkTar;
                    }
                    RoleBase target = null;
                    float mixAngle = 361;
                    for (int i = 0; i < targets.Count; i++)
                    {

                        Vector3 autoVec = (targets[i].GetMidPos() -
                            Global.instance.m_Hero_01.GetMidPos()).normalized;
                        float angle = Vector3.Angle(autoVec, mov);
                        if (angle <= mixAngle)
                        {
                            mixAngle = angle;
                            target = targets[i];
                            tempJoyStickVec = autoVec;
                        }
                    }

                    if (targets.Count == 0)
                    {
                        Debug.LogError("Targets Count Is Zero !!!");
                        return null;
                    }
                    else
                        return target;
                }


        }

        return null;
    }
    public virtual Vector3 GetFootPos()
    {
        return new Vector3(transform.position.x,
            transform.position.y, 0);
    }

    public Vector3 GetMidPos()
    {
        Debug.Assert(m_MidPos != null, "Check MidPos !!!");
        return m_MidPos.position;
    }

    public Vector3 GetAtkStartPos()
    {
        return m_AtkStartPos != null ?
            m_AtkStartPos.position : this.GetMidPos();
    }

    protected Transform GetRes()
    {
        return transform.GetChild(0);
    }


    public bool GetFlipX()
    {
        return m_Flip;
    }
    public virtual void SetFlipX(bool flipX)
    {
        if (m_Flip == flipX) return;
        m_Flip = flipX;

        float flipVal = flipX ? -1f : 1f;
        this.GetRes().SetLocalScale_X(
            Mathf.Abs(this.GetRes().localScale.x) * flipVal);
    }

    protected virtual bool CheckAttack(RoleBase tar)
    {
        if (tar == null) return false;

        if (tar.m_CurStatus == RoleStatus.Die) return false;

        float dis2Castle = Vector3.Distance(this.GetFootPos(),
            tar.GetFootPos());


        return dis2Castle <= m_AtkRange;
    }

    // call once
    public virtual void MoveTo(Vector3 tarPos)
    {
        if (m_IE_MoveTo != null)
        {
            StopCoroutine(m_IE_MoveTo);
            m_IE_MoveTo = null;
        }

        m_IE_MoveTo = IE_MoveTo(tarPos);
        StartCoroutine(m_IE_MoveTo);
    }
    private IEnumerator IE_MoveTo(Vector3 tarPos)
    {
        while (true)
        {
            if (Vector3.Distance(this.GetFootPos(), tarPos) <
                m_MoveVec * Time.deltaTime)
            {
                if (m_IE_MoveTo != null)
                {
                    StopCoroutine(m_IE_MoveTo);
                    m_IE_MoveTo = null;
                }

                this.Idle();
            }
            else
            {
                this.Move(tarPos - this.GetFootPos());
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // update
    public virtual void MoveTowards(RoleBase tar)
    {
        Vector3 vec = tar.GetFootPos() - this.GetFootPos();

        // calculation the deviation
        if (Mathf.Abs(tar.GetFootPos().y - this.GetFootPos().y) <= tar.m_BeAtkWidth)
        {
            if (tar.GetFootPos().y - this.GetFootPos().y > 0)
            {
                vec = new Vector3(vec.x,
                vec.y - (Mathf.Abs(tar.GetFootPos().y - this.GetFootPos().y)), 0);
            }
            else
            {
                vec = new Vector3(vec.x,
                vec.y + (Mathf.Abs(tar.GetFootPos().y - this.GetFootPos().y)), 0);
            }
        }
        else
        {
            if (tar.GetFootPos().y - this.GetFootPos().y > 0)
            {
                vec = new Vector3(vec.x, 0, vec.y - tar.m_BeAtkWidth);
            }
            else
            {
                vec = new Vector3(vec.x, 0, vec.y + tar.m_BeAtkWidth);
            }
        }

        this.Move(vec);

    }

    // vec : the vector of moving rotation
    public void Move(Vector3 vec)
    {
        if (m_CurStatus == RoleStatus.Die) return;
        if (m_CurStatus == RoleStatus.Atked) return;

        if (vec == Vector3.zero) return;

        m_CurStatus = RoleStatus.Move;

        vec = vec.normalized;
        this.SetCurVec_Move(vec);
        vec *= m_MoveVec * m_EffectiveVal;

        Rigidbody rig = GetComponent<Rigidbody>();
        if (rig != null)
        {
            rig.MovePosition(transform.position + vec * Time.deltaTime);
        }
        else
        {
            transform.Translate(vec * Time.deltaTime);
        }

        // action
        if (m_AniMng != null)
        {
            m_AniMng.Move();
        }

        // particle
        if (m_Particle_Dust != null && m_ParticlePos_Dust != null)
        {
            if (m_NextDustPlayTime < Time.time)
            {
                ParticleManager.instance.CreateParticle(
                    m_Particle_Dust, m_ParticlePos_Dust.position, Vector3.zero);

                m_NextDustPlayTime = Time.time + m_DustPlayInterval;
            }
        }
    }

    public void SetEffectiveVal(float val)
    {
        m_EffectiveVal = val;
    }

    public void SetCurVec_Move(Vector2 vec)
    {
        if (m_CurVec_Move == vec) return;

        m_CurVec_Move = vec;
    }

    public Vector2 GetCurFaceVec()
    {
        if (m_CurVec_AtkTar != Vector2.zero)
        {
            return m_CurVec_AtkTar;
        }
        else
        {
            return m_CurVec_Move;
        }
    }

    public void SetCurVec_Atk(Vector2 vec)
    {
        if (m_CurVec_AtkTar == vec) return;

        m_CurVec_AtkTar = vec;
    }


    protected IEnumerator IE_Attack(RoleBase atkTar)
    {
        while (true)
        {
            if (!this.CheckAttack(atkTar))
            {
                StopCoroutine(m_IE_Attack);
                m_IE_Attack = null;
            }
            else
            {
                this.Attack(atkTar);
            }

            yield return new WaitForSeconds(m_AtkInterval);
        }
    }

    public virtual void Attack(RoleBase atkTar)
    {
        this.Attack();
    }

    public virtual void Attack()
    {
        this.Attack(Constants.DamageName_NorAtk);
    }

    // new base
    public virtual void Attack(string DmgName)
    {
        if (m_CurStatus == RoleStatus.Die) return;
        if (!m_DmgsDic.ContainsKey(DmgName)) return;
        GameObject dmgPrefab = m_DmgsDic[DmgName];

        this.Attack(dmgPrefab);
    }

    protected void Attack(GameObject dmgPrefab)
    {
        if (m_CurStatus == RoleStatus.Die) return;
        m_CurStatus = RoleStatus.Attack;

        // Debug.Log(gameObject.name + " is Attack()");

        m_CurAtkGO = (GameObject)Resources.Load("Attack/" + dmgPrefab.name);
        Damage dmgCO = m_CurAtkGO.GetComponent<Damage>();
        if (dmgCO != null)
        {
            // run action
            m_AniMng.Attack(dmgCO.m_ActName);
        }
    }
    protected virtual GameObject CreateDmg()
    {
        if (m_CurAtkGO == null) return null;
        Vector3 startPos = this.GetAtkStartPos();
        
        // Debug.Log(gameObject.name + " is CreateDmg()");

        // float vecLength = (startPos - this.GetMidPos()).magnitude;
        // startPos = vecLength * this.GetCurFaceVec().normalized;
        // startPos += this.GetMidPos();
        GameObject dmgGO = Instantiate(
            m_CurAtkGO,
            startPos, Quaternion.identity);

        Effect eftCO = dmgGO.GetComponent<Effect>();
        Damage dmgCO = dmgGO.GetComponent<Damage>();

        // init 
        if (eftCO != null)
        {
            // RoleBase tarRole = this.GetCurAtkTarget(GetCurAtkTargetList(), TargetsSortType.Distance);
            eftCO.Init(this, this.GetCurFaceVec().normalized, m_CurAtkTar);
        }
        if (dmgCO != null)
        {
            dmgCO.Init(this, null);
            dmgCO.enabled = true;

            dmgCO.SetTarLayer(m_TarLayer);
        }

        // set parent 
        dmgGO.transform.SetParent(transform.parent, true);

        m_CurAtkGO = null;

        return dmgGO;
    }

    public virtual void UpdateHPShow()
    {
        if (m_HPSlider == null) return;

        float val = (float)m_CurHP / (float)m_MaxHP;
        // m_HPSlider.value = val;
        m_HPSlider.DOValue(val, 1f);

        // if (m_CurHP == 0)
        // {
        //     m_HPSlider.gameObject.SetActive(false);
        // }
    }

    public virtual bool GetHurt(long val, RoleBase atker)
    {
        // Debug.Log(gameObject.name + "Get hurt , val = " + val);

        if (m_CurStatus == RoleStatus.Die) return false;

        if (m_Invincible) return false;

        float pitch = 0.9f + UnityEngine.Random.Range(-0.3f, 0.3f);

        // play audio
        AudioManager.instance.Play(Get_Crow_AudioName(), pitch);
        AudioManager.instance.Play(Get_Atked_01_AudioName(), pitch);
        AudioManager.instance.Play(Get_Atked_02_AudioName(), pitch);
        // m_CurHP -= val;
        // this.SetHP(m_CurHP - val);
        this.SetHP(m_CurHP - val);

        // this.UpdateHPShow();

        this.HurtProtect(m_HurtProtectTime);

        return true;
    }
    private void SetHP(long newVal)
    {
        m_CurHP = newVal;
        if (m_CurHP < 0)
        {

            m_CurHP = 0;
            this.Die();

            // min the mass
            Rigidbody2D rig = GetComponent<Rigidbody2D>();
            if (rig != null)
            {
                rig.mass /= 3f;
            }
        }
        this.UpdateHPShow();
    }

    protected virtual void HurtProtect(float t)
    {
        if (m_IE_StopInvicible != null)
        {
            StopCoroutine(m_IE_StopInvicible);
            m_IE_StopInvicible = null;
        }

        m_Invincible = true;
        m_IE_StopInvicible = StopInvicible(t);
        StartCoroutine(m_IE_StopInvicible);
    }
    protected virtual IEnumerator StopInvicible(float t)
    {
        yield return new WaitForSeconds(t);
        m_Invincible = false;
    }

    public virtual void Idle()
    {
        if (m_CurStatus == RoleStatus.Die) return;

        m_CurStatus = RoleStatus.Idle;

        if (m_AniMng != null) m_AniMng.Idle();
    }

    public virtual void Die()
    {

        m_CurStatus = RoleStatus.Die;

        // Audio play
        float pitch = 0.8f + UnityEngine.Random.Range(-0.3f, 0.3f);

        AudioManager.instance.Play(Get_Death_AudioName(), pitch);
        CameraEffect.instance.Shake(0.2f, 1f, 0.1f);

        if (m_AniMng != null)
        {
            m_AniMng.Die();
        }
        else
        {
            this.RemoveSelf();
        }
    }

    public virtual void RemoveSelf()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        this.OnTrigger(col);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        this.OnTrigger(col);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        this.OnTrigger_Exit(col);
    }

    protected virtual void OnTrigger(Collider2D col)
    {
        Damage[] colDamages = col.gameObject.GetComponents<Damage>();

        foreach (Damage oneDamage in colDamages)
        {
            if (oneDamage.enabled)
            {
                this.GetDamage(oneDamage);
            }
        }

        int layer = col.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Plane"))
        {
            SceneObject colPlanes = col.gameObject.GetComponentInParent<SceneObject>();

            if (colPlanes != null)
            {
                this.GetCol(colPlanes, true);
            }
        }


    }


    protected virtual void OnTrigger_Exit(Collider2D col)
    {
        int layer = col.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Plane"))
        {
            SceneObject colPlanes = col.gameObject.GetComponentInParent<SceneObject>();

            if (colPlanes != null)
            {
                this.GetCol(colPlanes, false);
            }
        }
    }


    public virtual void GetDamage(Damage colDmg)
    {
        if (colDmg.m_RoleBase.gameObject.layer == gameObject.layer)
        {
            return;
        }

        bool canEft = false;

        switch (colDmg.m_Type)
        {
            case DamageType.Damage:
                {
                    long dmgVal = 0;
                    bool crit = false;

                    canEft = colDmg.GetDmg2Role(this, out dmgVal, out crit);

                    if (!canEft) return;

                    // if (colDmg.m_RoleBase == null) return;

                    canEft = this.GetHurt(dmgVal, colDmg.m_RoleBase);

                    // add force
                    if (canEft)
                    {
                        Rigidbody2D rb = GetComponent<Rigidbody2D>();
                        if (rb && colDmg.m_RoleBase != null)
                        {
                            Vector3 hitPos = colDmg.m_RoleBase.GetMidPos();
                            Vector3 forceVec = this.GetMidPos() - hitPos;
                            // forceVec = new Vector3(forceVec.x, 0, 0);
                            forceVec = forceVec.normalized;
                            forceVec *= colDmg.GetTrulyThrust();
                            rb.AddForce(forceVec);
                        }
                    }
                }
                break;

            case DamageType.SlowDown:
                {
                    // float eftVal = 1f;
                    // float dura = 0f;

                    // canEft = colDmg.GetSlowDownEft2DmgBase(this, out eftVal, out dura);

                    // if (!canEft) return;

                    // SetTemporaryVector(eftVal, dura);
                }
                break;

            case DamageType.Poison:
                {
                    // long dmgVal = 0;//changed int => long by forestli
                    // int times = 1;

                    // canEft = colDmg.GetPosion2DmgBase(this, out dmgVal, out times);

                    // if (!canEft) return;

                    // if (m_IE_GetPosion != null)
                    // {
                    //     StopCoroutine(m_IE_GetPosion);
                    //     m_IE_GetPosion = null;
                    // }

                    // m_IE_GetPosion = GetPosionDmg(dmgVal, times);
                    // StartCoroutine(m_IE_GetPosion);
                }
                break;

            default:
                break;
        }


    }

    public virtual void AffectedOther(string dmgName, RoleBase other)
    {

    }

    public virtual void UpdateDataFromPhoton_HP(object data)
    {
        long _hp = (long)data;

        if (_hp != m_CurHP)
        {
            m_CurHP = _hp;
            this.UpdateHPShow();
        }
    }
    public virtual void UpdateDataFromPhoton_Status(object data)
    {
        RoleStatus _status = (RoleStatus)(int)data;

        if (_status != m_CurStatus)
        {
            m_CurStatus = _status;
        }
    }


    public virtual void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        // if (m_PhotonView != null && !m_PhotonView.isMine) return;

        if (e.Data.Name == Constants.SpineEventName_Attack ||
        e.Data.Name == Constants.SpineEventName_attack)
        {
            this.AtkAnimationEvent();
        }
        if (e.Data.Name == Constants.SpineEventName_Step || e.Data.Name == "Run")
        {
            float pitch = 0.9f + UnityEngine.Random.Range(-0.15f, 0.15f);
            // footstepAudioSource.Play();
            // AudioManager.instance.Play(Get_Step_AudioName(), pitch);
        }
        // if(e.Data.Name == "Run")
        // {
        //     float pitch = 0.9f + UnityEngine.Random.Range(-0.15f, 0.15f);
        //     // footstepAudioSource.Play();
        //     AudioManager.instance.Play("Slime_Step_01",pitch);
        // }



    }

    protected virtual void AtkAnimationEvent()
    {
        this.CreateDmg();
    }

    public virtual void HandleComplete(TrackEntry trackEntry)
    {
        if (trackEntry.ToString() == m_AniMng.m_AniName_Die)
        {
            this.RemoveSelf();
        }

        if (trackEntry.ToString() == m_AniMng.GetCurAtkName())
        {
            this.AtkAnimationComplete();
        }

        if (trackEntry.ToString() == "Attack_01")
        {
            this.AtkAnimationComplete();
        }

    }

    protected virtual void AtkAnimationComplete()
    {
        this.Idle();
    }

    protected void OnParticleCollision(GameObject other)
    {
        Damage[] colDamages = other.GetComponents<Damage>();
        foreach (Damage oneDamage in colDamages)
        {
            if (oneDamage.enabled)
            {
                this.GetDamage(oneDamage);
            }
        }
    }
    public void StopMoveCoroutine()
    {
        if (m_IE_MoveTo != null)
        {
            StopCoroutine(m_IE_MoveTo);
            m_IE_MoveTo = null;
        }
    }

    protected virtual void GetCol(SceneObject colSceneObj, bool enter)
    {
        int newID = enter ? colSceneObj.m_PlanesID : Constants.PlanesID_Public;
        this.SetPlanesID(newID);
    }

    protected virtual void SetPlanesID(int id)
    {
        // when finish walking spin's alpha will change to 1f with unknown reason
        //bool errorAlpha = false;
        //float curAlpha = m_AniMng.GetAlpha();
        //if (m_CurPlanesID != Constants.PlanesID_Public && curAlpha == 1f)
        //	errorAlpha = true;

        if (m_CurPlanesID != id)
        {
            m_CurPlanesID = id;

            // if (this is Hero)
            // {
            //     Debug.Log(gameObject.name + "<" + PhotonNetwork.player.ID + ">\n"
            //                     + "m_CurPlanesID = " + m_CurPlanesID);
            // }


            this.UpdatePlanesIDShow();


        }


    }

    public void UpdatePlanesIDShow()
    {
        List<int> allID = this.GetAllPlanesIDCanBeSeen();
        if (m_CurPlanesID == Constants.PlanesID_Public)
        {
            m_AniMng.SetAlpha(1f);
            // if (m_Shield != null) m_Shield.SetActive(true);

            if (m_Shadow != null) m_Shadow.SetActive(true);
            if (m_Canvas != null) m_Canvas.gameObject.SetActive(true);
        }
        else if (allID.Contains(m_CurPlanesID))
        {
            m_AniMng.SetAlpha(0.3f);
            // if (m_Shield != null) m_Shield.SetActive(true);
            if (m_Shadow != null) m_Shadow.SetActive(true);
            if (m_Canvas != null) m_Canvas.gameObject.SetActive(true);
        }
        else
        {
            m_AniMng.SetAlpha(0f);
            // if (m_Shield != null) m_Shield.SetActive(false);
            if (m_Shadow != null) m_Shadow.SetActive(false);
            if (m_Canvas != null) m_Canvas.gameObject.SetActive(false);
            if (m_PrepareChangeParticle != null) m_PrepareChangeParticle.SetActive(false);
        }
    }

    private List<int> GetAllPlanesIDCanBeSeen()
    {
        List<int> result = new List<int>();

        result.Add(Global.instance.m_Hero_01.m_CurPlanesID);
        result.Add(Global.instance.m_Hero_02.m_CurPlanesID);

        return result;
    }

    protected virtual string Get_Atk_01_AudioName()
    {
        return "Null";
    }
    protected virtual string Get_Atk_02_AudioName()
    {
        return "Null";
    }
    protected virtual string Get_Atked_01_AudioName()
    {
        return "Null";
    }
    protected virtual string Get_Atked_02_AudioName()
    {
        return "Null";
    }
    protected virtual string Get_Step_AudioName()
    {
        return "Null";
    }
    protected virtual string Get_Death_AudioName()
    {
        return "Null";
    }
    protected virtual string Get_Crow_AudioName()
    {
        return "Null";
    }

    public void RollAtkTarget(int targetIndex)
    {
        if (this.GetCurAtkTargetList() == null)
        {
            Debug.LogError("CurAtkTargets Is NULL !!!");
            return;
        }
        if (targetIndex > this.GetCurAtkTargetList().Count - 1)
            return;
        m_CurAtkTar = this.GetCurAtkTargetList()[targetIndex];
    }
    public void RollAtkTarget(RoleBase targetRoleBase)
    {
        // if (this.GetCurAtkTargetList() == null)
        //     return;
        // if (!this.GetCurAtkTargetList().Contains(targetRoleBase))
        //     return;
        m_CurAtkTar = targetRoleBase;
    }
    public RoleBase PublicGetCurAtkTarget()
    {

        return GetCurAtkTarget(GetCurAtkTargetList(), TargetsSortType.Distance);
    }
    protected void InitBehaviorTree()
    {
        ExternalBehaviorTree extBT = Resources.Load<ExternalBehaviorTree>(behaviorDesignerPath);
        if (extBT == null) return;
        BehaviorTree bt = gameObject.AddComponent<BehaviorTree>();
        bt.ExternalBehavior = extBT;
        bt.StartWhenEnabled = false;
        bt.RestartWhenComplete = true;
        m_BehaviorTree = bt;
        m_BehaviorTree.DisableBehavior();
        m_BehaviorTree.EnableBehavior();
    }
    protected void InitNav()
    {
        m_Agent = gameObject.GetComponentInChildren<NavMeshAgent>();
        if (m_Agent == null) return;
    }
    protected void InitNavNBehaviorTree()
    {
        InitNav();
        InitBehaviorTree();
    }
    public void Talk(string wordstring, float duatime = 5.0f)
    {
        if (m_TalkText == null) return;

        if (m_IE_Talk != null)
        {
            return;
        }
        else
        {
            m_IE_Talk = IE_Talk(wordstring, duatime);
            StartCoroutine(m_IE_Talk);
        }
    }


    IEnumerator IE_Talk(string wordstring, float duatime)
    {
        m_TalkText.transform.parent.gameObject.SetActive(true);
        m_TalkText.text = wordstring;
        yield return new WaitForSeconds(duatime);
        m_TalkText.transform.parent.gameObject.SetActive(false);
        m_IE_Talk = null;
    }
}
