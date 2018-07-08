using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBoom : Enemy
{
    protected override void AtkAnimationEvent()
    {
        this.Attack();

        base.AtkAnimationEvent();

        this.RemoveSelf();
    }
}
