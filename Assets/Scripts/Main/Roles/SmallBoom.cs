using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBoom : Enemy
{
    private bool m_IsOnGround = true;

    protected override void Awake()
    {
        base.Awake();

        m_IsOnGround = true;
    }

    public void Hide()
    {
        if (!m_IsOnGround) return;

        m_AniMng.SetTrigger("Hide");
    }

    public void Rise()
    {
        if (m_IsOnGround) return;

        m_AniMng.SetTrigger("Rise");
    }

    public override void HandleEvent(string key)
    {
        base.HandleEvent(key);

        if (key == "HideDone")
        {
            m_IsOnGround = false;
        }
    }

    public override bool GetHurt(long val, RoleBase atker)
    {
        if (!m_IsOnGround) return false;

        return base.GetHurt(val, atker);
    }

    protected override void AtkAnimationEvent()
    {
        this.Attack();

        base.AtkAnimationEvent();

        this.RemoveSelf();
    }
}
