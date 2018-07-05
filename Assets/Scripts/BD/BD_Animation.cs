using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using Spine.Unity;
using Spine;
public enum BDAnimation
{
    NULL = -1,
    Attack = 0,
    Death = 1,
    Idle = 2,
    Move = 3,
    SIZE
}
public class BD_Animation : Action
{
    private RoleAnimation m_RoleAnimation;
    private SkeletonAnimation m_SkeletonAnimation;
    
    public BDAnimation animationName = BDAnimation.NULL;
    private bool running = false;
    public override void OnAwake()
    {
        m_RoleAnimation = gameObject.GetComponentInChildren<RoleAnimation>();
        m_SkeletonAnimation = gameObject.GetComponentInChildren<RoleAnimation>().m_SkeAnimation_0;

      
    }
    public override void OnStart()
    {
        //  m_SkeletonAnimation.state.Complete += HandleComplete;
        // m_SkeletonAnimation.ClearState();
        switch (animationName)
        {
            case BDAnimation.Idle:
                m_RoleAnimation.Idle();
                break;
            case BDAnimation.Move:
                m_RoleAnimation.Move();
                break;
            // case BDAnimation.Attack:
            //     m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Attack, false);
            //     break;
            // case BDAnimation.Death:

            //     // m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PostRollingAttack, false);
            //     m_SkeletonAnimation.state.SetAnimation(0, m_AniName_PreRollingAttack, false);
            //     m_SkeletonAnimation.state.AddAnimation(0, m_AniName_RollingAttack, true, 0f);

            //     running = true;
            //     break;
            
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
    // private void HandleComplete(TrackEntry trackEntry)
    // {

    // //     if (trackEntry.ToString() == m_AniName_PreRollingAttack
    // //     || trackEntry.ToString() == m_AniName_PreSpinAttack
    // //     || trackEntry.ToString() == m_AniName_PostSpinAttack
    // //     || trackEntry.ToString() == m_AniName_PostRollingAttack
    // //    )
    // //     {
    // //          running = false;

    // //     }

    // }
    private void ReturnRunning()
    {
        running = true;
    }
    private void ReturnSuccess()
    {
        running = false;
    }
}
