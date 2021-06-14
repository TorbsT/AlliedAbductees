using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Run
{
    private static float cameraInfluence = 0.2f;
    private GM gm;
    [SerializeField] public List<Chain> chains;
    public static float speed = 10f;
    public float x;
    public int HP;

    public List<Heart> hearts;
    public List<GameObject> companionRoster;
    public List<Companion> companions;
    private Section newestSection;
    private int nextLoc;
    public int score;

    private bool restartTimerGo;
    private float restartTimer;


    public Run(GM gm)
    {
        this.gm = gm;
        HP = gm.startHP;

        hearts = new List<Heart>();
        for (int i = 0; i < HP; i++)
        {
            Heart h = Object.Instantiate(gm.heartPrefab).GetComponent<Heart>();
            if (h == null) Debug.LogWarning("Illegal");
            h.id = i;
            hearts.Add(h);
        }

        restartTimer = gm.restartTimerDefault;
        GameObject g = Object.Instantiate(gm.startCompanion);
        Companion c = g.GetComponent<Companion>();
        c.manualStart(new Vector2Int(0, 2));
        c.join();
        companionRoster = new List<GameObject>();
        companions = new List<Companion>() { c };
        chains = new List<Chain>();
        foreach (GameObject go in gm.companionPrefabs)
        {
            companionRoster.Add(go);
        }
    }
    public void update()
    {
        if (restartTimerGo)
        {
            restartTimer -= Time.deltaTime;
            if (restartTimer <= 0f)
            {
                gm.restart();
                return;
            }
        }
        foreach (Companion c in companions)
        {
            c.manualUpdate();
        }
    }
    private float avgXDistFromCam()
    {
        float dist = 0f;
        int count = 0;
        foreach (Companion c in companions)
        {
            if (c.joined)
            {
                dist += c.transform.position.x - x;
                count++;
            }
        }
        if (count == 0) return 0f;
        dist /= count;
        return dist;
    }



    private void setScore()
    {
        score = Mathf.FloorToInt(x/8);
        gm.scoreTxt.text = "Score: "+score;
    }
    public void fixedUpdate()
    {
        gm.bgufo.manualFixedUpdate();
        setScore();
        float camRadiusX = 10f;
        x += speed * Time.fixedDeltaTime*(1+avgXDistFromCam()*cameraInfluence/camRadiusX);
        setCamPos(x);
        gm.ufo.setPos(x);
        while (newSectionTime()) setNextSection();
        foreach (Heart h in hearts)
        {
            h.manualUpdate();
        }
        foreach (Chain c in chains)
        {
            c.manualUpdate();
        }
        for (int i = companions.Count-1; i >= 0; i--)
        {
            Companion c = companions[i];
            c.manualFixedUpdate();
        }
    }
    public void checkPrematureRestart()
    {
        foreach (Companion c in companions)
        {
            if (c.joined) return;
        }
        while (HP > 0) loseHP();
    }
    private void setNextSection()
    {
        Section s = getNextSection();
        s.use(nextLoc);
        nextLoc += s.size;
        newestSection = s;
        spawnCompanion();
    }
    public void loseHP()
    {
        if (HP <= 0) return;
        HP--;
        hearts[HP].destroy();
        hearts.RemoveAt(HP);
        if (HP == 0)
        {
            GM.getAudioManager().play("OuttaLives");
            restartTimerGo = true;
        }
    }
    public void killAll()
    {
        foreach (Heart h in hearts)
        {
            h.destroy();
        }
        foreach (Chain c in chains)
        {
            c.killAll();
        }
        for (int i = companions.Count - 1; i >= 0; i--)
        {
            Companion c = companions[i];
            c.begone();
        }
        foreach (Section s in gm.sections)
        {
            s.unuse();
        }
    }
    public void removeAllChainsAttachedTo(Companion companion)
    {
        
        if (chains.Count == 0) return;
        for (int i = chains.Count-1; i >= 0; i--)
        {
            Chain c = chains[i];
            if (c.has(companion))
            {
                c.killAll();
                chains.RemoveAt(i);
            }
        }
    }
    private bool newSectionTime()
    {
        if (newestSection == null) return true;
        if (nextLoc - getLoc() < 16) return true;
        return false;
    }
    private Section getNextSection()
    {
        int min = 0;
        int max = gm.sections.Length;
        for (int i = 0; i < 100; i++)
        {
            int choose = Random.Range(min, max);
            Section s = gm.sections[choose];
            if (!s.used) return s;
        }
        Debug.LogWarning("Assign sections");
        return null;
    }
    private void spawnCompanion()
    {
        if (!newestSection.hasSpawn) return;
        GameObject go = getInactiveCompanionPrefab();
        if (go == null) return;

        Companion c1 = Object.Instantiate(go, gm.sectionWrapper.transform).GetComponent<Companion>();
        Vector2Int localSpawn = newestSection.localSpawn;
        Vector2Int loc2 = new Vector2Int(nextLoc - newestSection.size + localSpawn.x, localSpawn.y);
        c1.manualStart(loc2);
        companions.Add(c1);
    }
    private GameObject getInactiveCompanionPrefab()
    {
        bool exists = false;
        foreach (GameObject go in companionRoster)
        {
            Companion c = go.GetComponent<Companion>();
            if (!companionIsActive(c)) { exists = true; break; }
        }
        if (!exists) return null;
        int min = 0;
        int max = companionRoster.Count;
        for (int i = 0; i < 10000; i++)
        {
            int choose = Random.Range(min, max);
            GameObject prefab = companionRoster[choose];
            Companion c = prefab.GetComponent<Companion>();
            if (!companionIsActive(c)) { return prefab; }
        }
        return null;

    }
    private bool companionIsActive(Companion c)
    {
        foreach (Companion c1 in companions)
        {
            if (c1.id == c.id) return true;
        }
        return false;
    }

    public Companion getCompanionInActivationRange(Vector2 pos)
    {
        float range = gm.activationRange * gm.activationRange;
        foreach (Companion c in companions)
        {
            Vector2 p = c.transform.position;
            if ((p - pos).sqrMagnitude < range) return c;
        }
        return null;
    }
    public void addChain(Chain c) { chains.Add(c); }
    public int getLoc() { return Mathf.FloorToInt(x / 8); }
    public void scroll() { }
    public void setCamPos(float x) { gm.cam.transform.position = new Vector3(x, 20, -100f); }
}
