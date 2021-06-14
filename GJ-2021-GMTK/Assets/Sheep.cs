using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Companion
{
    public override void manualExtraStart()
    {
        GetComponent<SpriteRenderer>().color = Random.ColorHSV(0, 1, 0f, 0.5f, 0.5f, 1f, 1f, 1f);
    }
}
