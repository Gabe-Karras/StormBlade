using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Oh yeah. This is the big one. (0 for action, 1 for turn-based)
    [SerializeField]
    private int gameMode;

    // Important player variables
    [SerializeField]
    private int hp = 10;
    [SerializeField]
    private int bp = 1;
    [SerializeField]
    private int smallHealthCount;
    [SerializeField]
    private int bigHealthCount;
    [SerializeField]
    private int bombCount;
    [SerializeField]
    private int lightningCount;
    [SerializeField]
    private int missileCount;
    [SerializeField]
    private int shieldCount;
    [SerializeField]
    private GameObject activeShield;

    private const int MAX_ACTION_HP = 10;
    private const int MAX_ACTION_BP = 5;
    private const int MAX_ITEM_COUNT = 99;

    private bool initializing = true;

    // Other managers
    [SerializeField]
    private GameObject uiManager;
    [SerializeField]
    private GameObject scrollManager;
    [SerializeField]
    private GameObject musicManager;

    // Player reference
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Set framerate
        Application.targetFrameRate = GameSystem.FRAME_RATE;

        // Create managers
        scrollManager = Instantiate(scrollManager);
        uiManager = Instantiate(uiManager);
        musicManager = Instantiate(musicManager);

        // Find player
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Stuff is done here to give managers time to initialize
        if (initializing) {
            // Updates to initialize appearances and settings
            UpdateHp(0);
            UpdateBp(0);
            UpdateSmallHealthCount(0);
            UpdateBigHealthCount(0);
            UpdateBombCount(0);
            UpdateLightningCount(0);
            UpdateMissileCount(0);
            UpdateShieldCount(0);

            musicManager.GetComponent<MusicManager>().PlayMusic(Resources.Load<AudioClip>("Music/Level1"));
            musicManager.GetComponent<MusicManager>().PlayMusic(Resources.Load<AudioClip>("Music/Level1"));
            initializing = false;

            // Case where ship starts at level 5
            if (bp == 5) {
                Instantiate(Resources.Load("Prefabs/Player/LaserRing"));
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
            Instantiate(Resources.Load<GameObject>("Prefabs/Items/Bomb"), player.transform.position, player.transform.rotation);
    }

    // Execute intro cutscene
    private void IntroCutscene() {
        // Start scrollmanager at 0 speed and UI at 0 alpha

        // Fly ship by, play sound, flash and shake screen

        // Start accelerating scroll speed to 3x

        // Slowly lower ship in and even out scroll speed

        // Bring ship up to middle

        // Start text, fade in UI

        // Give player control
    }

    // GETTERS AND SETTERS
    public int GetGameMode() {
        return gameMode;
    }

    // Getters and setters for hp/bp
    public void UpdateHp(int hpChange) {
        // Check if HP is being subtracted from, and only allow it if player is not invincible
        if (hpChange < 0) {
            if (player.GetComponent<PlayerController>().GetIframes())
                return;
            
            if (activeShield != null) {
                activeShield.GetComponent<Shield>().UpdateShieldState(-1);
                player.GetComponent<PlayerController>().SetHit(true);
                return;
            }
            
            // Notify player and subtract from blaster points
            if (hp + hpChange <= 0) {
                player.GetComponent<PlayerController>().SetDead(true);
                player.GetComponent<PlayerController>().PlayDeathSequence();
            }
            else
                player.GetComponent<PlayerController>().SetHit(true);
            UpdateBp(-1);
        }

        hp = KeepInBounds(hp + hpChange, 0, MAX_ACTION_HP);

        // Update display
        uiManager.GetComponent<UIManager>().UpdateHealthCells();
    }

    public void UpdateBp(int bpChange) {
        int previousBp = bp;
        bp = KeepInBounds(bp + bpChange, 1, MAX_ACTION_BP);

        // Update display
        uiManager.GetComponent<UIManager>().UpdateBlasterCells();

        // Add/remove laser ring
        if (bp == 5 && previousBp != 5) {
            Instantiate(Resources.Load("Prefabs/Player/LaserRing"));
        } else if (bp != 5 && previousBp == 5) {
            Destroy(GameObject.Find("LaserRing(Clone)"));
        }
    }


    public int GetHp() {
        return hp;
    }

    public int GetBp() {
        return bp;
    }

    // Getters and setters for items
    public int GetSmallHealthCount() {
        return smallHealthCount;
    }

    public int GetBigHealthCount() {
        return bigHealthCount;
    }

    public int GetBombCount() {
        return bombCount;
    }

    public int GetLightningCount() {
        return lightningCount;
    }

    public int GetMissileCount() {
        return missileCount;
    }

    public int GetShieldCount() {
        return shieldCount;
    }

    public void UpdateSmallHealthCount(int change) {
        smallHealthCount = KeepInBounds(smallHealthCount + change, 0, MAX_ITEM_COUNT); // Keep within bounds
        uiManager.GetComponent<UIManager>().UpdateItemImage(smallHealthCount, "SmallHealthSymbol");
    }

    public void UpdateBigHealthCount(int change) {
        bigHealthCount = KeepInBounds(bigHealthCount + change, 0, MAX_ITEM_COUNT);
        uiManager.GetComponent<UIManager>().UpdateItemImage(bigHealthCount, "BigHealthSymbol");
    }

    public void UpdateBombCount(int change) {
        bombCount = KeepInBounds(bombCount + change, 0, MAX_ITEM_COUNT);
        uiManager.GetComponent<UIManager>().UpdateItemImage(bombCount, "BombSymbol");
        uiManager.GetComponent<UIManager>().UpdateSelectorState(); // Keep selector sprite updated
    }

    public void UpdateLightningCount(int change) {
        lightningCount = KeepInBounds(lightningCount + change, 0, MAX_ITEM_COUNT);
        uiManager.GetComponent<UIManager>().UpdateItemImage(lightningCount, "LightningSymbol");
        uiManager.GetComponent<UIManager>().UpdateSelectorState();
    }

    public void UpdateMissileCount(int change) {
        missileCount = KeepInBounds(missileCount + change, 0, MAX_ITEM_COUNT);
        uiManager.GetComponent<UIManager>().UpdateItemImage(missileCount, "MissileSymbol");
        uiManager.GetComponent<UIManager>().UpdateSelectorState();
    }

    public void UpdateShieldCount(int change) {
        shieldCount = KeepInBounds(shieldCount + change, 0, MAX_ITEM_COUNT);
        uiManager.GetComponent<UIManager>().UpdateItemImage(shieldCount, "ShieldSymbol");
        uiManager.GetComponent<UIManager>().UpdateSelectorState();
    }

    public GameObject GetActiveShield() {
        return activeShield;
    }

    public void SetActiveShield(GameObject obj) {
        activeShield = obj;
    }

    // Method to keep value between given bounds
    private int KeepInBounds(int value, int lowBound, int highBound) {
        if (value > highBound)
            value = highBound;
        else if (value < lowBound)
            value = lowBound;
        
        return value;
    }

    // Getters for managers
    public UIManager GetUIManager() {
        return uiManager.GetComponent<UIManager>();
    }
}