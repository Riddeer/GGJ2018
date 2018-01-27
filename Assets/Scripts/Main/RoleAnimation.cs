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


    public BoneFollower m_BoneFollower;
    public SkeletonAnimation m_SkeAnimation;

    [SpineAnimation(dataField: "m_SkeAnimation")]
    public string m_AniName_Move = "Move_01";
    [SpineAnimation(dataField: "m_SkeAnimation")]
    public string m_AniName_Idle = "Idle_01";
    [SpineAnimation(dataField: "m_SkeAnimation")]
    public string m_AniName_Die = "Death_01";
    [SpineAnimation(dataField: "m_SkeAnimation")]
    public string m_AniName_TransMission = "Transmission";
    public AttackAimType m_AtkAimType = AttackAimType.Aim;
    public Transform[] m_NormalRotate;
    public Transform[] m_LessRotate;
    public float armRotateRange = 1;
    public float bodyRotateRange = 1;
    public float m_AimTurnSpeed = 5f;

    public RotatedSpineNodes[] m_AimNodes;

    private IEnumerator m_IE_Wink;
    private bool isAttack = false;
    private RoleBase m_Base;
    private float m_DefaultTimeScale;
    private float m_CurAlpha = 1f;
    private Color m_CurColor = Color.white;
    private string m_CurAtkName = "";
    private List<Vector3> normalV3 = new List<Vector3>();
    private List<Vector3> lessV3 = new List<Vector3>();
    // private Hero m_player;
    void Awake()
    {
        m_Base = gameObject.GetComponent<RoleBase>();

        m_DefaultTimeScale = m_SkeAnimation.timeScale;
    }

    // Use this for initialization
    void Start()
    {
        m_SkeAnimation.state.Event += m_Base.HandleEvent;
        m_SkeAnimation.state.Complete += m_Base.HandleComplete;
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
    }

    public void Idle()
    {
        this.RunAnimation(m_AniName_Idle, true, 0, false);
    }

    public void Attack(string aniName)
    {
        m_CurAtkName = aniName;

        this.RunAnimation(aniName, false, 1, true);
    }
    public void Transmission()
    {
        this.RunAnimation(m_AniName_TransMission, false, 0, false);
    }
    // this attack is used by hero to switch weapon
    public void SwitchWeaponTextrue(string skinName, WeaponID weaponID, HeroID heroID)
    {
        // var weaponData = GDEManager.instance.GetData_Weapon((int)weaponID);
        // string boneName = "AtkPos_" + skinName;
        // m_BoneFollower.SetBone(boneName);
        // switch (heroID)
        // {
        //     case HeroID.SpaceKnight:
        //         {
        //             if (weaponData.AniType.ID == 0)
        //             {
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_R", "Arm_R");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_L", "Arm_L");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_Hold", null);

        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun", skinName);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun_01", null);


        //             }
        //             else if (weaponData.AniType.ID == 1)
        //             {
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_R", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_L", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_Hold", "BothHand");

        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun_01", skinName);

        //             }
        //         }
        //         break;

        //     case HeroID.IdiotKnight:
        //         {
        //             if (weaponData.AniType.ID == 0)
        //             {
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_hold", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Armor3", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_R3", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun_01", null);


        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun", skinName);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Armor", "Armor 2");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_R", "Hand_R");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_R", "Arm_R");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_L", "Hand_L");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_L", "Arm_L");

        //             }
        //             else if (weaponData.AniType.ID == 1)
        //             {
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_hold", "Hand_hold");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Armor3", "Armor");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_R3", "Arm_R");
        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun_01", skinName);


        //                 m_SkeAnimation.Skeleton.SetAttachment("Gun", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Armor", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_R", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_R", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Hand_L", null);
        //                 m_SkeAnimation.Skeleton.SetAttachment("Arm_L", null);

        //             }
        //         }
        //         break;

        //     default:
        //         break;
        // }
    }

    public void SetSkin(string skinName)
    {
        m_SkeAnimation.skeleton.SetSkin(skinName);
    }
    public void Stop(int trackIndex)
    {
        // TrackEntry track = m_SkeAnimation.state.GetCurrent(trackIndex);
        m_SkeAnimation.state.ClearTrack(trackIndex);
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
    public void Wink()
    {
        this.RunAnimation("Face_Wink", false, 3, false);
        var empty = m_SkeAnimation.state.AddEmptyAnimation(3, 0.5f, 0.1f);
        empty.AttachmentThreshold = 1f;

    }
    public void DefaultFace()
    {
        this.RunAnimation("Face_Default", true, 3, false);
    }
    public void AngryFace()
    {
        this.RunAnimation("Face_Attack", false, 4, false);
        var empty = m_SkeAnimation.state.AddEmptyAnimation(4, 0.5f, 1f);
        empty.AttachmentThreshold = 1f;
    }
    public void StartSetWink()
    {
        if (m_IE_Wink != null)
        {
            StopCoroutine(m_IE_Wink);
            m_IE_Wink = null;
        }
        m_IE_Wink = SetWink();
        StartCoroutine(m_IE_Wink);
    }
    public void StopWink()
    {
        if (m_IE_Wink != null)
        {
            StopCoroutine(m_IE_Wink);
            m_IE_Wink = null;
        }
    }
    IEnumerator SetWink()
    {
        while (true)
        {
            Wink();
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        }
    }
    public void Die()
    {
        // PauseAnimation();
        // m_SkeAnimation.state.ClearTracks();
        m_SkeAnimation.state.ClearTracks();
        this.RunAnimation(m_AniName_Die, false, 5, false);

        // falling action
        // Soldier soldier = m_Base as Soldier;
        // if (soldier != null && soldier.m_Height > 0.5f)
        // {
        // 	float deadActTime = this.GetCurAniTime();
        // 	transform.DOMoveY(0.5f, deadActTime).SetEase(Ease.Linear)
        // 		.OnUpdate(delegate ()
        // 		{
        // 			soldier.m_Shadow.transform.SetPosition_Y(0);
        // 		});
        // }
    }

    public void PauseAnimation()
    {
        m_SkeAnimation.timeScale = 0;
    }

    public float GetCurAniTime()
    {
        Spine.TrackEntry curEntry = m_SkeAnimation.state.GetCurrent(0);
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
        bool isPlayingTheSame = false;
        if (m_SkeAnimation && m_SkeAnimation.state.GetCurrent(0) != null)
            isPlayingTheSame = m_SkeAnimation.state.GetCurrent(0).Animation.Name == aniName;

        isPlayingTheSame = false;
        if (m_SkeAnimation && m_SkeAnimation.state.GetCurrent(0) != null)
            isPlayingTheSame = m_SkeAnimation.state.GetCurrent(0).Animation.Name == aniName;

        if (overrideSameName)
        {
            m_SkeAnimation.state.SetAnimation(index, aniName, loop);
        }
        else
        {
            if (!isPlayingTheSame)
            {

                m_SkeAnimation.state.SetAnimation(index, aniName, loop);
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
        m_SkeAnimation.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);

    }

    private void SetTimeScale(float s)
    {
        m_SkeAnimation.timeScale = s;
    }


    public float GetAlpha()
    {
        Material material = m_SkeAnimation.GetComponent<Renderer>().material;
        // return material.GetColor("_Color").a;
        return material.GetFloat("_PlaneAlpha");
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