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
    private Vector3 LEFT;
    private Vector3 RIGHT;
    private Vector3 BOTTOM;
    private Vector3 BOTTOM_LEFT;
    private Vector3 BOTTOM_RIGHT;
    private Vector3 BOTTOM_LEFT_CORNER;
    private Vector3 BOTTOM_RIGHT_CORNER;

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
    [SerializeField]
    public GameObject ufo;
    [SerializeField]
    public GameObject chomper;
    [SerializeField]
    public GameObject juggler;

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
    private MusicManager musicManager;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        TOP = new Vector3(0, GameSystem.Y_ACTION_BOUNDARY, 0);
        TOP_LEFT = new Vector3(-0.53f, GameSystem.Y_ACTION_BOUNDARY, 0);
        TOP_RIGHT = new Vector3(0.53f, GameSystem.Y_ACTION_BOUNDARY, 0);
        TOP_LEFT_CORNER = new Vector3(-1.06f, GameSystem.Y_ACTION_BOUNDARY, 0);
        TOP_RIGHT_CORNER = new Vector3(1.06f, GameSystem.Y_ACTION_BOUNDARY, 0);
        LEFT = new Vector3(GameSystem.X_ACTION_BOUNDARY * -1, 0, 0);
        RIGHT = new Vector3(GameSystem.X_ACTION_BOUNDARY, 0, 0);
        BOTTOM = new Vector3(0, GameSystem.Y_ACTION_BOUNDARY * -1, 0);
        BOTTOM_LEFT = new Vector3(-0.53f, GameSystem.Y_ACTION_BOUNDARY * -1, 0);
        BOTTOM_RIGHT = new Vector3(0.53f, GameSystem.Y_ACTION_BOUNDARY * -1, 0);
        BOTTOM_LEFT_CORNER = new Vector3(GameSystem.X_ACTION_BOUNDARY * -1, GameSystem.Y_ACTION_BOUNDARY * -1, 0);
        BOTTOM_RIGHT_CORNER = new Vector3(GameSystem.X_ACTION_BOUNDARY, GameSystem.Y_ACTION_BOUNDARY * -1, 0);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cutsceneManager = gameManager.GetCutsceneManager();
        musicManager = gameManager.GetMusicManager();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Build correct level
        level = BuildLevel(gameManager.GetLevel());
    }

    // Update is called once per frame
    void Update()
    {
        // Execute the level
        if (playerController.HasControl() && levelQueue.Count != 0) {
            // Spawn entities from the list as long as a wavetimer is not present
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

                // Missiles
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 5));

                // Intro to shufflers
                result.AddEntity(new LevelEntity(shuffler, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, TOP_RIGHT));
                result.AddEntity(new LevelEntity(shuffler, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1.5f));
                result.AddEntity(new LevelEntity(shuffler, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1.5f));
                result.AddEntity(new LevelEntity(shuffler, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1.5f));
                result.AddEntity(new LevelEntity(shuffler, TOP_LEFT));

                // Weavers and shufflers
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaverLv2, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaver, TOP - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(weaver, TOP + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(shuffler, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(shuffler, TOP_RIGHT));

                // Blaster 3
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(blaster, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 3));

                result.AddEntity(new LevelEntity(weaverLv2, TOP_LEFT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(weaverLv2, TOP_RIGHT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(shuffler, TOP));
                

                // Intro to kamikaze
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 2));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));

                // Shufflers and kamikaze
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(shuffler, TOP_LEFT));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));
                result.AddEntity(new LevelEntity(shuffler, TOP_RIGHT));
                
                // Bombs!!!
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(bomb, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(bomb, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(bomb, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 5));
                

                // Intro to dragonflies
                result.AddEntity(new LevelEntity(dragonfly, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 3));
                result.AddEntity(new LevelEntity(dragonfly, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(dragonfly, TOP_LEFT));

                // Blaster 4
                result.AddEntity(new LevelEntity(blaster, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));

                // Kamikaze and dragonflies
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(bomb, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));

                // Weavers and dragonflies
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaverLv2, TOP));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 2));
                result.AddEntity(new LevelEntity(weaverLv2, TOP_LEFT));
                result.AddEntity(new LevelEntity(weaverLv2, TOP_RIGHT));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(dragonfly, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(bomb, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 3));

                // Mix of everything
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, TOP_LEFT));
                result.AddEntity(new LevelEntity(missile, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));

                // Blaster 5
                result.AddEntity(new LevelEntity(blaster, TOP));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(shuffler, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(dragonfly, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(weaver, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(weaver, TOP_LEFT));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(kamikaze, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(bomb, GameSystem.RandomPoint(TOP_LEFT_CORNER, TOP_RIGHT_CORNER)));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(kamikaze, TOP_LEFT + new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(kamikaze, TOP_RIGHT - new Vector3(HALF, 0, 0)));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0.5f));
                result.AddEntity(new LevelEntity(kamikaze, TOP));

                // Intro to stalkers
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 1));
                result.AddEntity(new LevelEntity(stalker, TOP));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(stalker, TOP));

                // Boss
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 5));
                break;

            case 2: // LEVEL 2 --------------------------------------------------------------------------------

                // Dragonflies and bombs

                // Kamikazes

                // Blaster 2

                // Introduction to UFOs

                // Weavers and UFO

                // Lightning

                // Dragonflies and UFO

                // Blaster 3

                // Stalker and UFO

                // Introduction to Chompers

                // Chompers and Shufflers

                // Shield

                // Chompers and Dragonflies

                // Blaster 4

                // Introduction to Jugglers

                // Jugglers and Kamikazes

                // Jugglers and Shufflers

                // Blaster 5

                // Chompers and UFO

                // Two Stalkers

                // Boss


                result.AddEntity(new LevelEntity(waveTimer, transform.position, 3));
                result.AddEntity(new LevelEntity(chomper, BOTTOM));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(juggler, GameSystem.RandomPoint(LEFT, TOP_LEFT_CORNER - new Vector3(0, 0.64f, 0))));

                result.AddEntity(new LevelEntity(waveTimer, transform.position, 0));
                result.AddEntity(new LevelEntity(waveTimer, transform.position, 5));
                break;
        }

        levelQueue = result.GetQueue();
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

        // If last item in queue, fade out music
        if (levelQueue.Count == 0) {
            StartCoroutine(musicManager.FadeOut());
        }
    }
}
