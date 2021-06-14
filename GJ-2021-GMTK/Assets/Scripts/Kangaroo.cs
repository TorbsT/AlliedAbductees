using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kangaroo : Companion
{
    public Vector2 superJumpForce;

    public override void hold(bool charged)
    {

    }
    public override void unhold(bool charged)
    {
        if (!canJump()) walk();
        else
        {
            if (charged) superJump();
            else
            {
                if (canJump()) jump(0.5f, 1f);
                walk();
            }
        }
    }
    private void superJump()
    {
        applyForce(superJumpForce);
        justJumped(1f, 0.7f);
    }
}
