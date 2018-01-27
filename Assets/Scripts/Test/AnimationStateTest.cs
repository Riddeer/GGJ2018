using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class AnimationStateTest : MonoBehaviour
{

    SkeletonAnimation m_SK;
    void Start()
    {
        m_SK = GetComponent<SkeletonAnimation>();
        TrackTest();
        // 	 m_SK.state.Complete +=  delegate {
        //      // ... 或者选择忽略它的参数
        //      HandleEvent ();
        //   };
        // m_SK.state.AddAnimation(0,"Death_01",false,0);

    }

    private void TrackTest()
    {
        var Track = m_SK.state.SetAnimation(0, "Move_01", false);
        Track.Complete += delegate
        {
            // ... 或者选择忽略它的参数
            HandleEvent();
        };
    }
    void HandleEvent()
    {


        var state = m_SK.state.SetAnimation(0, "Death_01", false);
        state.Complete += delegate
        {
            // ... 或者选择忽略它的参数
            m_SK.state.SetAnimation(0, "Move_01", false);
        };
    }
}


