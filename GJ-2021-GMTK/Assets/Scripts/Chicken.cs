using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : Companion
{
    [SerializeField] private Vector2 flyForce;
    [SerializeField] private float flySpeed;

    public override void hold(bool charged)
    {
        
        if (grounded)
        {
            if (jumpForce.x == 0 && jumpForce.y == 0) { jump(); fly(); }
            else jump();
        }
        else fly();
        grounded = false;
    }
    public override void unhold(bool charged)
    {
        walk();
        animator.SetBool("Flying", false);
    }
    private void fly()
    {
        animator.SetBool("Flying", true);
        if (rb.velocity.y < flySpeed) applyForce(flyForce);
    }
}
