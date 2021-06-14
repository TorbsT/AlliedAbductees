using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGUFO : MonoBehaviour
{
    public float xMin;
    public float xMax;
    public float speed = 1f;

    // Update is called once per frame
    public void manualFixedUpdate()
    {
        Vector2 prevPos = transform.localPosition;
        Vector3 pos = new Vector3(prevPos.x- speed, prevPos.y, -1f);
        if (pos.x < xMin) pos.x = xMax;
        transform.localPosition = pos;
    }
}
