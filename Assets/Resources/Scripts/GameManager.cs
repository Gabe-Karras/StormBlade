using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    // Oh yeah. This is the big one. (0 for action, 1 for turn-based)
    [SerializeField]
    private int gameMode;

    // This one's a pretty big deal too!
    [SerializeField]
    private int level;

    // Important player variables
    [SerializeField]
    private int hp = 10;
    [SerializeField]
    private int bp;
    [SerializeField]
    private float shieldDefense = 2;
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
    [SerializeField]
    private GameObject shieldPrefab;

    // Whether game is paused or not
    private bool paused = false;

    private const int MAX_ACTION_HP = 10;
    private const int MAX_TURN_HP = 500;
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
    [SerializeField]
    private GameObject cutsceneManager;
    [SerializeField]
    private GameObject levelManager;

    // Player reference
    private GameObject player;

    // Boss of the level
    private GameObject boss;
    private bool bossTurn = false;
    private bool levelEnded = true;

    // Start is called before the first frame update
    void Start()
    {
        // Load data from data manager
        LoadData();

        // Create managers
        scrollManager = Instantiate(scrollManager, transform.position, transform.rotation);
        uiManager = Instantiate(uiManager, transform.position, transform.rotation);
        musicManager = Instantiate(musicManager, transform.position, transform.rotation);
        cutsceneManager = Instantiate(cutsceneManager, transform.position, transform.rotation);
        levelManager = Instantiate(levelManager, transform.position, transform.rotation);

        // Find player
        player = GameObject.Find("Player");

        // Get boss prefab
        boss = Resources.Load<GameObject>("Prefabs/Enemies/Bosses/Boss" + level);
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
            // This is called once more for initializing the turn-based menu
            UpdateBombCount(0);

            // Play level intro
            cutsceneManager.GetComponent<CutsceneManager>().IntroCutscene();
            initializing = false;

            // Case where ship starts at level 5
            if (bp == 5) {
                Instantiate(Resources.Load("Prefabs/Player/LaserRing"));
            }
        }

        // End level when boss is defeated
        if (boss == null && !levelEnded) {
            cutsceneManager.GetComponent<CutsceneManager>().VictoryCutscene();
            level ++;
            levelEnded = true;
        }
    }

    // Load data from data manager
    public void LoadData() {
        int[] data = DataManager.Instance.GetData();
        level = data[0];
        bp = data[1];
        bombCount = data[2];
        lightningCount = data[3];
        missileCount = data[4];
        shieldCount = data[5];
        smallHealthCount = data[6];
        bigHealthCount = data[7];
    }

    // Send current data to data manager
    public void SaveData() {
        int[] data = {level, bp, bombCount, lightningCount, missileCount, shieldCount, smallHealthCount, bigHealthCount};
        DataManager.Instance.SetData(data);
    }

    // Set up player to take a turn in turn-based mode
    private void TakeTurn() {
        // First, check if player is dead
        if (hp <= 0) {
            player.GetComponent<PlayerController>().SetDead(true);
            player.GetComponent<PlayerController>().PlayDeathSequence();
            if (activeShield != null)
                Destroy(activeShield);

            return;
        }

        // If not, activate menu!
        uiManager.GetComponent<UIManager>().SetUIMode(1);
    }

    // Spawn in the boss above camera
    public void SpawnBoss() {
        boss = Instantiate(boss, new Vector3(0, GameSystem.Y_ACTION_BOUNDARY * 2, 0), Quaternion.Euler(0, 0, 0));
    }

    // GETTERS AND SETTERS
    public int GetGameMode() {
        return gameMode;
    }

    // Don't call this unless you know what you're doing
    public void SwitchGameMode() {
        if (gameMode == 0) {
            gameMode = 1;

            // Convert hp to turn-based bounds
            hp *= 50;

            // Instantly update healthbar
            uiManager.GetComponent<UIManager>().SetHealthBarValue(hp);

            // If action shield exists, replace with turn-based shield
            if (activeShield != null) {
                Destroy(activeShield);
                activeShield = Instantiate(shieldPrefab);
            }
        } else
            gameMode = 0;
    }

    public int GetLevel() {
        return level;
    }

    // Get reference to boss instance
    public Boss GetBoss() {
        return boss.GetComponent<Boss>();
    }

    public bool IsPaused() {
        return paused;
    }

    public void SetPaused(bool paused) {
        this.paused = paused;
    }

    // Getters and setters for hp/bp
    public void UpdateHp(int hpChange) {
        // In action mode, getting hit reduces your blaster points and death is immediate
        if (gameMode == 0) {
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
        // In turn based mode, modifying the hp results in a graphic, and death is determined at the
        // beginning of the player's turn
        else {
            // Text color in animation
            Color textColor = new Color(1, 1, 1);

            // If change is negative, apply shield defense
            if (hpChange < 0) {
                if (activeShield != null) {
                    hpChange = (int) (hpChange / shieldDefense);
                    activeShield.GetComponent<Shield>().UpdateShieldState(-1);
                }
            }
            // If change is positive, make it a health color
            else {
                // Same color as player healthbar and particle animation
                textColor = new Color(0.93f, 0.38f, 0.36f);
            }

            // Play animation
            float textX = player.transform.position.x * GameSystem.CANVAS_RATIO * GameSystem.PIXELS_PER_UNIT;
            float textY = player.transform.position.y * GameSystem.CANVAS_RATIO * GameSystem.PIXELS_PER_UNIT;

            GameObject damageText = Instantiate(uiManager.GetComponent<UIManager>().GetDamageText());
            damageText.GetComponent<TextMeshProUGUI>().text = Math.Abs(hpChange) + "";
            damageText.GetComponent<TextMeshProUGUI>().color = textColor;

            damageText.transform.SetParent(uiManager.GetComponent<UIManager>().GetTurnCanvas().transform);
            damageText.GetComponent<RectTransform>().anchoredPosition = new Vector2(textX, textY);

            hp = KeepInBounds(hp + hpChange, 0, MAX_TURN_HP);
        }
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

    public bool GetBossTurn() {
        return bossTurn;
    }

    public void SetBossTurn(bool turn) {
        bossTurn = turn;

        // Take player turn if set to false
        if (turn == false)
            TakeTurn();
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

    public ScrollManager GetScrollManager() {
        return scrollManager.GetComponent<ScrollManager>();
    }

    public MusicManager GetMusicManager() {
        return musicManager.GetComponent<MusicManager>();
    }

    public CutsceneManager GetCutsceneManager() {
        return cutsceneManager.GetComponent<CutsceneManager>();
    }

    public LevelManager GetLevelManager() {
        return levelManager.GetComponent<LevelManager>();
    }
}