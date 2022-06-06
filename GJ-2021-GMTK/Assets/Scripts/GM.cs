using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    private static GM gm;

    [SerializeField] public List<BGUFO> bgufos;
    [SerializeField] public TMPro.TextMeshProUGUI gameOverScoreTxt;
    [SerializeField] public GameObject gameOverScreen;
    [SerializeField] public GameObject introScreen;
    [SerializeField] public TMPro.TextMeshProUGUI scoreTxt;
    [SerializeField] public GameObject heartPrefab;
    [SerializeField] public float restartTimerDefault;
    [SerializeField] public int startHP;
    [SerializeField] public AudioManager audioManager;
    [SerializeField] public float deathPit = -0.8f;
    [SerializeField] public bool noDuplicates;
    [SerializeField] public bool randomSeed;
    [SerializeField] public int seed;
    [SerializeField] public UFO ufo;
    [SerializeField] public List<GameObject> companionPrefabs;
    [SerializeField] public Camera cam;
    [SerializeField] public GameObject sectionWrapper;
    [SerializeField] public GameObject chainLinkPrefab;
    [SerializeField] public GameObject startCompanion;
    [SerializeField] public KeyCode restartKey;
    [SerializeField] public KeyCode beginKey;
    [SerializeField] public float activationRange;
    [SerializeField] public GameObject chainMadeParticles;
    [SerializeField] public GameObject chainDeathParticles;
    [System.NonSerialized] public Transform particleWrapper;


    public Section[] sections;
    private bool begun;

    [SerializeReference] private Run run;
    
    public void spawnParticles(GameObject go, Vector2 pos)
    {
        Instantiate(go, pos, Quaternion.identity, particleWrapper);
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = this;
        sections = sectionWrapper.GetComponentsInChildren<Section>();
        restart();
    }
    public void restart()
    {
        
        scoreTxt.text = "";
        gameOverScoreTxt.text = "Score: N/A";
        audioManager.restart();
        if (run != null)
        {
            int score = run.score;
            gameOverScoreTxt.text = "Score: "+score;
            gameOverScreen.SetActive(true);
            introScreen.SetActive(false);
            Destroy(particleWrapper.gameObject);
            run.killAll();
        } else
        {
            gameOverScreen.SetActive(false);
            introScreen.SetActive(true);
        }
        particleWrapper = new GameObject().transform;
        begun = false;
    }
    private void begin()
    {
        audioManager.begin();
        begun = true;
        if (randomSeed) seed = Random.Range(0, 100000);
        Random.InitState(seed);
        run = new Run(this);
    }
    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (begun)
        {
            run.update();
            if (Input.GetKeyDown(restartKey)) restart();
        } else
        {
            if (Input.GetKeyDown(beginKey))
            {
                if (gameOverScreen.activeSelf)
                {
                    gameOverScreen.SetActive(false);
                    introScreen.SetActive(true);
                    
                    audioManager.play("MenuButton0");
                }
                else
                {
                    introScreen.SetActive(false);
                    audioManager.play("MenuButton1");
                    begin();
                }
            }
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (begun) run.fixedUpdate();
    }

    public static AudioManager getAudioManager() { return getGM().audioManager; }
    public static Run getRun() { return getGM().run; }
    public static GM getGM() { return gm; }


}
