using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public int size;
    public Vector2Int localSpawn;
    public int difficulty;

    public bool hasSpawn;
    public int loc;
    public bool used;


    public void Awake()
    {
        unuse();
    }
    public void use(int loc)
    {
        if (used) Debug.LogWarning("NO");
        this.loc = loc;
        transform.localPosition = new Vector2(loc * 8, 0);
        used = true;
    }
    public void unuse()
    {
        // ignores whether it was used
        used = false;
        transform.localPosition = new Vector2(0f, 50f*8f);
    }

    private void Update()
    {
        if (!used) return;
        if (GM.getRun().getLoc() > loc+size+8*7)
        {
            used = false;
        }
    }


}
