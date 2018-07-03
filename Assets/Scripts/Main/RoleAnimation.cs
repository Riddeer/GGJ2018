using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(RoleBase))]
public class RoleAnimation : MonoBehaviour
{
    public enum AttackAimType
    {
        NULL = -1,
        Aim = 1,
        NotAim = 2,
        Size
    }

    public SkeletonAnimation m_SkeAnimation_0;
    public SkeletonAnimation m_SkeAnimation_1;
    public Animator m_Animator_0;
    public Animator m_Animator_1;

    [SpineAnimation(dataField: "m_SkeAnimation_0")]
    public string m_AniName_Move = "Move_01";
    [SpineAnimation(dataField: "m_SkeAnimation_0")]
    public string m_AniName_Idle = "Idle_01";
    [SpineAnimation(dataField: "m_SkeAnimation_0")]
    public string m_AniName_Die = "Death_01";
    [SpineAnimation(dataField: "m_SkeAnimation_0")]
    public string m_AniName_TransMission = "Transmission";
    public AttackAimType m_AtkAimType = AttackAimType.Aim;
    public float m_AimTurnSpeed = 5f;

    public RotatedSpineNodes[] m_AimNodes;

    private IEnumerator m_IE_Wink;
    private bool isAttack = false;
    private RoleBase m_Base;
    private float m_DefaultTimeScale;
    private float m_CurAlpha = 1f;
    private Color m_CurColor = Color.white;
    private string m_CurAtkName = "";
    // private Hero m_player;
    void Awake()
    {
        m_Base = GetComponent<RoleBase>();
        if (m_SkeAnimation_0)
        {
            m_DefaultTimeScale = m_SkeAnimation_0.timeScale;
        }
        else
        {
            m_DefaultTimeScale = 1;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (m_SkeAnimation_0)
        {
            m_SkeAnimation_0.state.Event += m_Base.HandleEvent;
            m_SkeAnimation_0.state.Complete += m_Base.HandleComplete;
        }
        if (m_SkeAnimation_1)
        {
            m_SkeAnimation_1.state.Event += m_Base.HandleEvent;
            m_SkeAnimation_1.state.Complete += m_Base.HandleComplete;
        }
        m_CurAlpha = 1f;
        m_CurColor = Color.white;
        // m_player = Global.instance.m_Player_Mine;

        // SaveDefaultRotation(m_NormalRotate, normalV3);
        // SaveDefaultRotation(m_LessRotate, lessV3);

        this.SetAimNodesDefaultRotation();
    }

    public void Aim(Vector2 curFaceVec)
    {
        // AimTarget(m_NormalRotate, armRotateRange, normalV3, m_Base.GetCurFaceVec());
        // AimTarget(m_LessRotate, bodyRotateRange, lessV3, m_Base.GetCurFaceVec());
        if (m_Base.m_CurStatus == RoleStatus.Die) return;
        this.TurnAimNodesToTargetRotation(curFaceVec);
    }

    public void Move()
    {
        this.RunAnimation(m_AniName_Move, true, 0, false);

        if (m_Animator_0) m_Animator_0.SetTrigger("Move");
        if (m_Animator_1) m_Animator_1.SetTrigger("Move");
    }

    public void Idle()
    {
        this.RunAnimation(m_AniName_Idle, true, 0, false);

        if (m_Animator_0) m_Animator_0.SetTrigger("Idle");
        if (m_Animator_1) m_Animator_1.SetTrigger("Idle");
    }

    public void Attack(string aniName)
    {
        m_CurAtkName = aniName;

        this.RunAnimation(aniName, false, 1, true);

        if (m_Animator_0) m_Animator_0.SetTrigger("Attack");
        if (m_Animator_1) m_Animator_1.SetTrigger("Attack");
    }
    public void Transmission()
    {
        this.RunAnimation(m_AniName_TransMission, false, 0, false);
    }

    public void Stop(int trackIndex)
    {
        // TrackEntry track = m_SkeAnimation.state.GetCurrent(trackIndex);
        if (m_SkeAnimation_0) m_SkeAnimation_0.state.ClearTrack(trackIndex);
        if (m_SkeAnimation_1) m_SkeAnimation_0.state.ClearTrack(trackIndex);
    }

    // this attack is used by slime
    public void EnemyAttack(string aniName)
    {
        m_CurAtkName = aniName;
        this.RunAnimation(aniName, false, 0, false);

    }

    public string GetCurAtkName()
    {
        return m_CurAtkName;
    }
    public void StopWink()
    {
        if (m_IE_Wink != null)
        {
            StopCoroutine(m_IE_Wink);
            m_IE_Wink = null;
        }
    }
    public void Die()
    {
        if (m_SkeAnimation_0) m_SkeAnimation_0.state.ClearTracks();
        if (m_SkeAnimation_1) m_SkeAnimation_1.state.ClearTracks();
        this.RunAnimation(m_AniName_Die, false, 5, false);
    }

    public void PauseAnimation()
    {
        if (m_SkeAnimation_0) m_SkeAnimation_0.timeScale = 0;
        if (m_SkeAnimation_1) m_SkeAnimation_1.timeScale = 0;
    }

    public float GetCurAniTime()
    {
        if (!m_SkeAnimation_0) return 0f;

        Spine.TrackEntry curEntry = m_SkeAnimation_0.state.GetCurrent(0);
        if (curEntry != null)
        {
            // return curEntry.AnimationTime;
            return curEntry.Animation.Duration;
        }

        return 0f;
    }

    public void SetEffectiveVector(float val)
    {
        this.SetTimeScale(m_DefaultTimeScale * val);
        if (val < 1f)
        {
            // this.SetColor(m_FrozenColor.r, m_FrozenColor.g, m_FrozenColor.b);
        }
        else
        {
            // this.SetColor(Color.white.r, Color.white.g, Color.white.b);
        }
    }



    // when same name , <overrideSameName> the animation ;return is the animation called playing
    public bool RunAnimation(string aniName, bool loop, int index,
        bool overrideSameName = false)
    {
        if (m_SkeAnimation_0 == null && m_SkeAnimation_1 == null) return false;

        bool isPlayingTheSame = false;
        if (m_SkeAnimation_0 && m_SkeAnimation_0.state.GetCurrent(0) != null)
            isPlayingTheSame = m_SkeAnimation_0.state.GetCurrent(0).Animation.Name == aniName;

        isPlayingTheSame = false;
        if (m_SkeAnimation_0 && m_SkeAnimation_0.state.GetCurrent(0) != null)
            isPlayingTheSame = m_SkeAnimation_0.state.GetCurrent(0).Animation.Name == aniName;

        if (overrideSameName)
        {
            m_SkeAnimation_0.state.SetAnimation(index, aniName, loop);
            if (m_SkeAnimation_1) m_SkeAnimation_1.state.SetAnimation(index, aniName, loop);
        }
        else
        {
            if (!isPlayingTheSame)
            {
                m_SkeAnimation_0.state.SetAnimation(index, aniName, loop);
                if (m_SkeAnimation_1) m_SkeAnimation_1.state.SetAnimation(index, aniName, loop);
            }
        }

        return isPlayingTheSame;
    }


    public void DoFadeAction(float alphaVal, float t)
    {
        StartCoroutine(IE_Fade(alphaVal, t));
    }

    public void DoColorAction(float r, float g, float b, float t)
    {
        this.SetColor(r, g, b, m_CurAlpha);
        StartCoroutine(RevertColor(t));
    }

    private IEnumerator IE_Fade(float aVal, float t)
    {
        float passedTime = 0f;

        while (passedTime < t)
        {
            float newVal = 1f;
            if (passedTime < 0.5f * t)
            {
                // lower
                newVal = 1f -
                    passedTime * (1f - aVal) / (0.5f * t);
            }
            else
            {
                // higher
                newVal = aVal +
                    (passedTime - 0.5f * t) * (1f - aVal) / (0.5f * t);
            }
            this.SetAlpha(newVal);

            passedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        this.SetAlpha(1f);
    }
    public void DoScaleAction(Vector3 localscale)
    {
        StartCoroutine(IE_Scale(localscale));
    }
    private IEnumerator IE_Scale(Vector3 localscale)
    {

        Vector3 temp = Vector3.zero;
        float speed = 0.5f;
        temp = transform.localScale;
        while (transform.localScale != localscale)
        {
            temp.x = Mathf.Lerp(transform.localScale.x, localscale.x, Time.deltaTime * speed);
            temp.y = Mathf.Lerp(transform.localScale.y, localscale.y, Time.deltaTime * speed);
            transform.localScale = temp;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RevertColor(float t)
    {
        yield return new WaitForSeconds(t);
        this.SetColor(1, 1, 1, 1);
    }

    // [0, 1]
    public void SetAlpha(float a)
    {
        this.SetColor(m_CurColor.r, m_CurColor.g, m_CurColor.b, a);
    }

    // r g b [0, 1]
    private void SetColor(float r, float g, float b, float a)
    {
        m_CurAlpha = a;
        m_CurColor = new Vector4(r, g, b, m_CurAlpha);

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        // mpb.SetColor("_Color", m_CurColor);
        // mpb.SetColor("_OccludedColor", m_CurColor);
        mpb.SetFloat("_PlaneAlpha", m_CurAlpha);
        if (m_SkeAnimation_0) m_SkeAnimation_0.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
        if (m_SkeAnimation_1) m_SkeAnimation_1.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
    }

    private void SetTimeScale(float s)
    {
        if (m_SkeAnimation_0) m_SkeAnimation_0.timeScale = s;
        if (m_SkeAnimation_1) m_SkeAnimation_1.timeScale = s;
    }

    private void AimTarget(Transform[] transList, float rotateRange, List<Vector3> defaultV3List, Vector2 curFaceVec)
    {
        switch (m_AtkAimType)
        {
            case AttackAimType.Aim:
                TurnToTargetRotation(transList, rotateRange, defaultV3List, curFaceVec);
                break;
            case AttackAimType.NotAim:
                if (!isAttack)
                {
                    TurnToDefaultRotation(transList, defaultV3List);
                }
                else if (isAttack)
                {
                    TurnToTargetRotation(transList, rotateRange, defaultV3List, curFaceVec);
                }
                break;
        }
    }
    private void TurnToTargetRotation(Transform[] transList, float rotateRange,
        List<Vector3> defaultV3List, Vector2 curFaceVec)
    {
        if (m_Base.GetFlipX())
        {
            curFaceVec.x = -curFaceVec.x;
            curFaceVec.y = -curFaceVec.y;
        }
        for (int i = 0; i < transList.Length; i++)
        {
            float angle = Mathf.Atan2(curFaceVec.y, curFaceVec.x) * Mathf.Rad2Deg * rotateRange;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transList[i].rotation = Quaternion.Slerp(transList[i].rotation, rotation, m_AimTurnSpeed * Time.deltaTime);
        }
    }
    private void TurnToDefaultRotation(Transform[] transList,
        List<Vector3> defaultV3List)
    {
        for (int i = 0; i < transList.Length; i++)
        {
            transList[i].rotation = Quaternion.Euler(
                Mathf.Lerp(transList[i].rotation.x, defaultV3List[i].x, Time.deltaTime * 0.5f),
                Mathf.Lerp(transList[i].rotation.y, defaultV3List[i].y, Time.deltaTime * 0.5f),
                Mathf.Lerp(transList[i].rotation.z, defaultV3List[i].z, Time.deltaTime * 0.5f));
        }
    }
    private void SaveDefaultRotation(Transform[] transList, List<Vector3> defaultV3List)
    {
        for (int i = 0; i < transList.Length; i++)
        {
            defaultV3List.Add(
                new Vector3(transList[i].rotation.x,
                transList[i].rotation.y,
                transList[i].rotation.z));
        }
    }

    private void AimTarget(Vector2 curFaceVec)
    {
        switch (m_AtkAimType)
        {
            case AttackAimType.Aim:
                this.TurnAimNodesToTargetRotation(curFaceVec);
                break;
            case AttackAimType.NotAim:
                if (!isAttack)
                {
                    this.TurnAimNodesToDefaultRotation();
                }
                else if (isAttack)
                {
                    this.TurnAimNodesToTargetRotation(curFaceVec);
                }
                break;
        }
    }
    private void SetAimNodesDefaultRotation()
    {
        foreach (RotatedSpineNodes one in m_AimNodes)
        {
            one.SetDefaultRotation();
        }
    }

    private void TurnAimNodesToDefaultRotation()
    {
        foreach (RotatedSpineNodes one in m_AimNodes)
        {
            one.TurnToDefaultRotation();
        }
    }

    private void TurnAimNodesToTargetRotation(Vector2 curFaceVec)
    {
        if (m_Base.GetFlipX())
        {
            curFaceVec.x = -curFaceVec.x;
            curFaceVec.y = -curFaceVec.y;
        }

        foreach (RotatedSpineNodes one in m_AimNodes)
        {
            one.TurnToTargetRotation(curFaceVec, m_AimTurnSpeed);
        }
    }

    public void SetCurTransmissionRes(TransmissionType transType)
    {
        switch (transType)
        {
            case TransmissionType.Fat:
                {
                    if (m_SkeAnimation_0) m_SkeAnimation_0.gameObject.SetActive(true);
                    if (m_SkeAnimation_1) m_SkeAnimation_1.gameObject.SetActive(false);
                    if (m_Animator_0) m_Animator_0.gameObject.SetActive(true);
                    if (m_Animator_1) m_Animator_1.gameObject.SetActive(false);
                }
                break;

            case TransmissionType.Thin:
                {
                    if (m_SkeAnimation_0) m_SkeAnimation_0.gameObject.SetActive(false);
                    if (m_SkeAnimation_1) m_SkeAnimation_1.gameObject.SetActive(true);
                    if (m_Animator_0) m_Animator_0.gameObject.SetActive(false);
                    if (m_Animator_1) m_Animator_1.gameObject.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

}


[System.Serializable]
public class RotatedSpineNodes
{
    public float m_RotateRange;
    public List<Transform> m_TarNodes = new List<Transform>();

    private List<Vector3> m_DefaultRotations = new List<Vector3>();

    public void SetDefaultRotation()
    {
        m_DefaultRotations.Clear();

        foreach (Transform one in m_TarNodes)
        {
            m_DefaultRotations.Add(new Vector3(
                one.rotation.x,
                one.rotation.y,
                one.rotation.z
            ));
        }
    }

    public void TurnToDefaultRotation()
    {
        Debug.Assert(m_TarNodes.Count == m_DefaultRotations.Count,
            "m_TarNodes.Count must be equle to m_DefaultRotations.Count !!!");

        for (int i = 0; i < m_TarNodes.Count; i++)
        {
            m_TarNodes[i].rotation = Quaternion.Euler(
                Mathf.Lerp(m_TarNodes[i].rotation.x, m_DefaultRotations[i].x, Time.deltaTime * 0.5f),
                Mathf.Lerp(m_TarNodes[i].rotation.y, m_DefaultRotations[i].y, Time.deltaTime * 0.5f),
                Mathf.Lerp(m_TarNodes[i].rotation.z, m_DefaultRotations[i].z, Time.deltaTime * 0.5f));

        }
    }

    public void TurnToTargetRotation(Vector2 curFaceVec, float turnSpeed)
    {
        for (int i = 0; i < m_TarNodes.Count; i++)
        {
            float angle = Mathf.Atan2(curFaceVec.y, curFaceVec.x) *
                Mathf.Rad2Deg * m_RotateRange;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            m_TarNodes[i].rotation = Quaternion.Slerp(
                m_TarNodes[i].rotation, rotation, turnSpeed * Time.deltaTime);
        }
    }
}