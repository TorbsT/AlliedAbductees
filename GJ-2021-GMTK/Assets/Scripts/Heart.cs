using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    private static Vector2 startPos = new Vector2(30, 15);
    private static float diffX = 8;

    public int id;
    public void manualUpdate()
    {
        Vector2 camPos = GM.getGM().cam.transform.position;
        Vector2 firstPos = camPos + startPos;
        float thisDiffX = diffX * id;
        Vector2 pos = new Vector3(firstPos.x - thisDiffX, firstPos.y, -10);
        transform.localPosition = pos;
        Debug.Log(transform.position);
        Debug.Log(transform.localPosition);
    }
    public void destroy()
    {
        Destroy(gameObject);
    }

}
