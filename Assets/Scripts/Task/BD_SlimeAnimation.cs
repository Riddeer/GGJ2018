using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;
using Spine.Unity;
using Spine;
public enum SlimeAnimationName
{
    NULL = -1,
    Attack_01_Back = 0,
    Attack_01_Front = 1,
    Attack_02_Back = 2,
	Attack_02_Front = 3,
    Die_01_Front = 4,
    Move_01_Front = 5,
    Move_01_Back = 6,

    SIZE
}
public class BD_SlimeAnimation : Action
{
    private RoleAnimation m_RoleAnimation;
    private SkeletonAnimation m_SkeletonAnimation;
    public string m_AniName_Attack_01_Back = "Attack_01_Back";
    public string m_AniName_Attack_01_Front = "Attack_01_Front";
    public string m_AniName_Attack_02_Back = "Attack_02_Back";
	public string m_AniName_Attack_02_Front = "Attack_02_Front";
    public string m_AniName_Die_01_Front = "Die_01_Front";
    public string m_AniName_Move_01_Front = "Move_01_Front";
    public string m_AniName_Move_01_Back = "Move_01_Back";
    public SlimeAnimationName animationName = SlimeAnimationName.NULL;
    private bool running = false;
    public override void OnAwake()
    {
        m_RoleAnimation = gameObject.GetComponentInChildren<RoleAnimation>();
        m_SkeletonAnimation = gameObject.GetComponentInChildren<RoleAnimation>().m_SkeAnimation_0;
    }
    public override void OnStart()
    {
        m_SkeletonAnimation.state.Complete += HandleComplete;
        m_SkeletonAnimation.ClearState();
        switch (animationName)
        {
            case SlimeAnimationName.Attack_01_Back:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Attack_01_Back, false);
				running = true;
                break;
            case SlimeAnimationName.Attack_01_Front:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Attack_01_Front, false);
				running = true;
                break;
            case SlimeAnimationName.Attack_02_Back:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Attack_02_Back, false);
				running = true;
                break;
			case SlimeAnimationName.Attack_02_Front:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Attack_02_Front, false);
				running = true;
                break;
            case SlimeAnimationName.Die_01_Front:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Die_01_Front, false);
				running = true;
                break;
            case SlimeAnimationName.Move_01_Front:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Move_01_Front, true);
				
                break;
            case SlimeAnimationName.Move_01_Back:
                m_SkeletonAnimation.state.SetAnimation(0, m_AniName_Move_01_Back, true);
				
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
        running = false;
    }

}
