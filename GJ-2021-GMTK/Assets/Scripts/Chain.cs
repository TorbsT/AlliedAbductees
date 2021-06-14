using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain
{
    private static float stretchBoundary = (8*3)*(8*3);  // longer than this => stretching starts. Squared
    private static float stretchForce = 0.1f;
    private static int linkCount = 15;
    private Companion a;
    private Companion b;
    private List<Transform> links;  // Links in the chain (visual)

    public Chain(Companion a, Companion b)
    {
        this.a = a;
        this.b = b;
        GM.getRun().addChain(this);

        links = new List<Transform>();
        for (int i = 0; i < linkCount; i++)
        {
            Transform t = Object.Instantiate(GM.getGM().chainLinkPrefab).transform;
            links.Add(t);
        }
        manualUpdate();  // UH OH
        for (int i = 0; i < linkCount; i++)
        {
            Transform t = links[i];
            GM.getGM().spawnParticles(GM.getGM().chainMadeParticles, t.position);
        }


    }

    public void manualUpdate()
    {
        Vector2 aPos = a.transform.position;
        Vector2 bPos = b.transform.position;
        Vector2 difference = bPos - aPos;

        Vector2 normalized = difference.normalized;
        float stretchLength = difference.sqrMagnitude;
        float overflow = Mathf.Max(0, stretchLength - stretchBoundary);
        Vector2 force = normalized * stretchForce * overflow/2f;
        applyPullForce(a, force);
        applyPullForce(b, -force);


        // do visual
        for (int i = 0; i < linkCount; i++)
        {
            float length = ((float)i) / ((float)linkCount);
            Vector2 pos = aPos + length * difference;
            links[i].position = pos;
        }
    }
    public bool has(Companion c)
    {
        return a.Equals(c) || b.Equals(c);
    }
    public void killAll()
    {
        foreach (Transform t in links)
        {
            GM.getGM().spawnParticles(GM.getGM().chainDeathParticles, t.position);
            Object.Destroy(t.gameObject);
        }
    }

    private void applyPullForce(Companion c, Vector2 f)
    {
        c.applyForce(f);
    }
}
