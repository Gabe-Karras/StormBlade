using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script executes a given level
public class LevelManager : MonoBehaviour
{
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
        else if (playerController.HasControl() && levelQueue.Count == 0) {
            cutsceneManager.TransitionToTurnBased();
        }
    }

    // Returns fully constructed current level
    private Level BuildLevel(int level) {
        Level result = new Level();

        switch (level) {
            case 1:
                break;
        }

        return result;
    }

    // Instantiates a level entity
    private void InstantiateEntity(LevelEntity e) {
        GameObject obj = e.GetObject();
        Vector3 spawn = e.GetSpawn();
        Quaternion angle = Quaternion.Euler(0, 0, 0);

        Instantiate(obj, spawn, angle);
    }
}
