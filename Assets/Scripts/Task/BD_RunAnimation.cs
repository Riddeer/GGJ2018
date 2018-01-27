using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using Spine.Unity;
using Spine;
public enum AnimationName
{
    NULL = -1,
    Idle = 0,
    Move = 1,
    Attack = 2,
    RollingAttack = 3,
    SpinAttack = 4,
    RollingAttackEnd = 5,
    SpinAttackEnd = 6,
    JustRolling = 7,
    JustSpin = 8,
    Death = 9,
    SIZE
}
public class BD_RunAnimation : Action
{
    private RoleAnimation m_RoleAnimation;
    private SkeletonAnimation m_SkeletonAnimation;
    public string m_AniName_PostRollingAttack = "PostRollingAttack_01_Front";
    public string m_AniName_PreRollingAttack = "PreRollingAttack_01_Front";
    public string m_AniName_RollingAttack = "RollingAttack_01_Front";
    public string m_AniName_PostSpinAttack = "PostSpinAttack_01_Front";
    public string m_AniName_PreSpinAttack = "PreSpinAttack_01_Front";
    public string m_AniName_SpinAttack = "SpinAttack_01_Front";
    public string m_AniName_Attack = "Attack_01_Side";
    public AnimationName animationName = AnimationName.NULL;
    private bool running = false;
    public override void OnAwake()
    {
        m_RoleAnimation = gameObject.GetComponentInChildren<RoleAnimation>();
        m_SkeletonAnimation = gameObject.GetComponentInChildren<RoleAnimation>().m_SkeAnimation;

      
    }
    public override void OnStart()
    {
         m_SkeletonAnimation.state.Complete += HandleComplete;
        m_SkeletonAnimation.ClearState();
        switch (animationName)
        {
            case AnimationName.Idle:
                m_RoleAnimation.Idle();
                break;
            case AnimationName.Move:
                m_RoleAnimation.Move();
                break;
            case AnimationName.Attack:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Attack, false);
                break;
            case AnimationName.RollingAttack:

                // m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PostRollingAttack, false);
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PreRollingAttack, false);
                m_SkeletonAnimation.state.AddAnimation(0, m_AniName_RollingAttack, true, 0f);

                running = true;
                break;
            case AnimationName.SpinAttack:
                // m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PostSpinAttack, false);
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PreSpinAttack, false);
                m_SkeletonAnimation.state.AddAnimation(0, m_AniName_SpinAttack, true, 0f);

                running = true;
                break;
            case AnimationName.SpinAttackEnd:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PostSpinAttack, false);

                running = true;
                break;
            case AnimationName.RollingAttackEnd:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PostRollingAttack, false);

                running = true;
                break;
                case AnimationName.JustRolling:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_RollingAttack, true);

                running = false;
                break;
                 case AnimationName.JustSpin:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_SpinAttack, true);

                running = false;
                break;
            case AnimationName.Death:
                m_RoleAnimation.Die();
                break;
        }


    }
    public override TaskStatus OnUpdate()
    {
        if (running)
            return TaskStatus.Running;
        else
            return TaskStatus.Success;

    }
    public override void OnEnd()
    {

    }
    private void HandleComplete(TrackEntry trackEntry)
    {

        if (trackEntry.ToString() == m_AniName_PreRollingAttack
        || trackEntry.ToString() == m_AniName_PreSpinAttack
        || trackEntry.ToString() == m_AniName_PostSpinAttack
        || trackEntry.ToString() == m_AniName_PostRollingAttack
       )
        {
             running = false;

        }
        // else
        // {
        //     ReturnSuccess();
        // }
        // switch (trackEntry.ToString())
        // {
        //     case "PostRollingAttack_01_Front":
        //     case "PreRollingAttack_01_Front":
        //     case "PostSpinAttack_01_Front":
        //     // case "PreSpinAttack_01_Front":
        //           ReturnRunning();
        //         break;
        //     default:
        //       
        //  ReturnSuccess();
        //         break;
        // }
    }
    private void ReturnRunning()
    {
        running = true;
    }
    private void ReturnSuccess()
    {
        running = false;
    }
}
