using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script executes a given level
public class LevelManager : MonoBehaviour
{
    // Constants for spawning
    private Vector3 TOP;
    private Vector3 TOP_LEFT;
    private Vector3 TOP_RIGHT;
    private Vector3 TOP_LEFT_CORNER;
    private Vector3 TOP_RIGHT_CORNER;

    // Steps between defined values
    private const float HALF = 0.26f;
    private const float QUARTER = 0.13f;

    // List of executable levels
    private Level level;
    private Queue<LevelEntity> levelQueue;

    // References to EVERY ENEMY IN THE GAME
    [SerializeField]
    public GameObject weaver;
    [SerializeField]
    public GameObject weaverLv2;
    [SerializeField]
    public GameObject shuffler;
    [SerializeField]
    public GameObject kamikaze;
    [SerializeField]
    public GameObject dragonfly;
    [SerializeField]
    public GameObject stalker;

    // References to every item
    [SerializeField]
    public GameObject smallHealth;
    [SerializeField]
    public GameObject bigHealth;
    [SerializeField]
    public GameObject bomb;
    [SerializeField]
    public GameObject lightning;
    [SerializeField]
    public GameObject missile;
    [SerializeField]
    public GameObject shield;
    [SerializeField]
    public GameObject blaster;

    // Reference to wave timer entity
    [SerializeField]
    public GameObject waveTimer;

    // Other important references
    private GameManager gameManager;
    private CutsceneManager cutsceneManager;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        TOP = new Vector3(0, GameSystem.Y_ACTION_BOUNDARY + 0.32f, 0);
        TOP_LEFT = new Vector3(-0.53f, GameSystem.Y_ACTION_BOUNDARY + 0.32f, 0);
        TOP_RIGHT = new Vector3(0.53f, GameSystem.Y_ACTION_BOUNDARY + 0.32f, 0);
        TOP_LEFT_CORNER = new Vector3(-1.06f, GameSystem.Y_ACTION_BOUNDARY + 0.32f, 0);
        TOP_RIGHT_CORNER = new Vector3(1.06f, GameSystem.Y_ACTION_BOUNDARY + 0.32f, 0);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cutsceneManager = gameManager.GetCutsceneManager();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Build correct level
        level = BuildLevel(gameManager.GetLevel());
    }

    // Update is called once per frame
    void Update()
    {
        // Execute the level
        if (playerController.HasControl() && levelQueue.Count != 0) {
            // Spawn entities from the list as long a a wavetimer is not present
            if (GameObject.FindGameObjectsWithTag("WaveTimer").Length == 0) {
                InstantiateEntity(levelQueue.Dequeue());
            }
        } // When level is over, enter boss battle
        else if (playerController.HasControl() && levelQueue.Count == 0 && GameObject.FindGameObjectsWithTag("WaveTimer").Length == 0) {
            cutsceneManager.TransitionToTurnBased();
        }
    }

    // Returns fully constructed current level
    private Level BuildLevel(int level) {
        Level result = new Level();

        switch (level) {
            case 1: // LEVEL 1
                // Intro to weavers
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 3));
                result.AddEntity(new LevelEntity(weaver, TOP));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 2));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT));

                // Intro to weaver lv2
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaverLv2, TOP));
                
                // Mixed weavers
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT));
                result.AddEntity(new LevelEntity(weaverLv2, TOP));
                
                // Blaster 2
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(blaster, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 5));
                result.AddEntity(new LevelEntity(weaverLv2, TOP_LEFT));
                result.AddEntity(new LevelEntity(weaverLv2, TOP_RIGHT));
                result.AddEntity(new LevelEntity(weaver, TOP));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(weaver, TOP + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaverLv2, TOP + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaverLv2, TOP_LEFT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.2f));
                // Intro to shufflers

                // Weavers and shufflers

                // Blaster 3

                // Intro to kamikaze

                // Shufflers and kamikaze

                // Intro to dragonflies

                // Blaster 4

                // Kamikaze and dragonflies

                // Shufflers and dragonflies

                // Mix of everything

                // Blaster 5

                // Intro to stalkers

                // Boss
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 5));
                break;
        }

        levelQueue = result.GetQueue();
        Debug.Log("bruh");
        return result;
    }

    // Instantiates a level entity
    private void InstantiateEntity(LevelEntity e) {
        GameObject obj = e.GetObject();
        Vector3 spawn = e.GetSpawn();
        Quaternion angle = Quaternion.Euler(0, 0, 0);

        GameObject temp = Instantiate(obj, spawn, angle);

        // Set up wave timer vars
        if (obj.Equals(waveTimer)) {
            if (e.GetTimer() != 0)
                temp.GetComponent<WaveTimer>().SetTime(e.GetTimer());
        }
    }
}
