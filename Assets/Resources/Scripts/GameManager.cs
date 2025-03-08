using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Important player variables
    [SerializeField]
    private int hp = 10;
    [SerializeField]
    private int bp = 1;
    [SerializeField]
    private int smallHealthCount = 0;
    [SerializeField]
    private int bigHealthCount = 0;
    [SerializeField]
    private int bombCount = 0;
    [SerializeField]
    private int lightningCount = 0;
    [SerializeField]
    private int missileCount = 0;
    [SerializeField]
    private int shieldCount = 0;
    [SerializeField]
    private int selectorPosition = 0;

    private const int MAX_ACTION_HP = 10;
    private const int MAX_ACTION_BP = 5;
    private const int MAX_ITEM_COUNT = 99;

    // UI elements
    private GameObject healthCells;
    private GameObject blasterCells;

    private GameObject smallHealthSymbol;
    private GameObject bigHealthSymbol;
    private GameObject bombSymbol;
    private GameObject lightningSymbol;
    private GameObject missileSymbol;
    private GameObject shieldSymbol;

    private GameObject smallHealthLabel;
    private GameObject bigHealthLabel;
    private GameObject bombLabel;
    private GameObject lightningLabel;
    private GameObject missileLabel;
    private GameObject shieldLabel;

    private GameObject itemSelector;

    // Item selection
    private int[] activeItems = new int[4];
    private bool selectorActive = false;

    // Player reference
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Set framerate
        Application.targetFrameRate = 60;

        // Get UI elements
        healthCells = GameObject.Find("HealthCells");
        blasterCells = GameObject.Find("BlasterCells");

        smallHealthSymbol = GameObject.Find("SmallHealthSymbol");
        bigHealthSymbol = GameObject.Find("BigHealthSymbol");
        bombSymbol = GameObject.Find("BombSymbol");
        lightningSymbol = GameObject.Find("LightningSymbol");
        missileSymbol = GameObject.Find("MissileSymbol");
        shieldSymbol = GameObject.Find("ShieldSymbol");

        smallHealthLabel = GameObject.Find("SmallHealthCount");
        bigHealthLabel = GameObject.Find("BigHealthCount");
        bombLabel = GameObject.Find("BombCount");
        lightningLabel = GameObject.Find("LightningCount");
        missileLabel = GameObject.Find("MissileCount");
        shieldLabel = GameObject.Find("ShieldCount");

        itemSelector = GameObject.Find("Selector");

        // Find player
        player = GameObject.Find("Player");

        // Updates to initialize appearances and settings
        UpdateHp(0);
        UpdateBp(0);
        UpdateSmallHealthCount(0);
        UpdateBigHealthCount(0);
        UpdateBombCount(0);
        UpdateLightningCount(0);
        UpdateMissileCount(0);
        UpdateShieldCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        MoveSelector();

        if (Input.GetKeyDown(KeyCode.Return))
            UpdateBp(1);
    }

    // Move selector in accordance to action mode
    private void MoveSelector() {
        // Check if selection key is pressed
        if (Input.GetKeyDown(KeyCode.S)) {
            if (!selectorActive)
                return; // If it is deactivated
            else {
                // Check for next available item to select (Starting at current selection)
                int offset = selectorPosition + 1;
                for (int i = 0; i < activeItems.Length - 1; i ++) {
                    if (activeItems[(i + offset) % activeItems.Length] != 0) {
                        UpdateSelectorPosition((i + offset) % activeItems.Length);
                        break;
                    }
                }
            }
        }
    }

    // GETTERS AND SETTERS
    // Getters and setters for hp/bp
    public void UpdateHp(int hpChange) {
        // Check if HP is being subtracted from, and only allow it if player is not invincible
        if (hpChange < 0) {
            if (player.GetComponent<PlayerController>().GetIframes())
                return;
            
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
        Image img = healthCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/HealthCells")[hp];
    }

    public void UpdateBp(int bpChange) {
        int previousBp = bp;
        bp = KeepInBounds(bp + bpChange, 1, MAX_ACTION_BP);

        // Update display
        Image img = blasterCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/BlasterCells")[bp];

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
        UpdateItemImage(smallHealthCount, smallHealthSymbol.GetComponent<Image>()); // Adjust opacity of item UI
        smallHealthLabel.GetComponent<TextMeshProUGUI>().text = smallHealthCount + ""; // Set number label
    }

    public void UpdateBigHealthCount(int change) {
        bigHealthCount = KeepInBounds(bigHealthCount + change, 0, MAX_ITEM_COUNT);
        UpdateItemImage(bigHealthCount, bigHealthSymbol.GetComponent<Image>());
        bigHealthLabel.GetComponent<TextMeshProUGUI>().text = bigHealthCount + "";
    }

    public void UpdateBombCount(int change) {
        bombCount = KeepInBounds(bombCount + change, 0, MAX_ITEM_COUNT);
        UpdateItemImage(bombCount, bombSymbol.GetComponent<Image>());
        activeItems[0] = bombCount; // Update array for seletor
        UpdateSelectorState(); // Keep selector sprite updated
        bombLabel.GetComponent<TextMeshProUGUI>().text = bombCount + "";
    }

    public void UpdateLightningCount(int change) {
        lightningCount = KeepInBounds(lightningCount + change, 0, MAX_ITEM_COUNT);
        UpdateItemImage(lightningCount, lightningSymbol.GetComponent<Image>());
        activeItems[1] = lightningCount;
        UpdateSelectorState();
        lightningLabel.GetComponent<TextMeshProUGUI>().text = lightningCount + "";
    }

    public void UpdateMissileCount(int change) {
        missileCount = KeepInBounds(missileCount + change, 0, MAX_ITEM_COUNT);
        UpdateItemImage(missileCount, missileSymbol.GetComponent<Image>());
        activeItems[2] = missileCount;
        UpdateSelectorState();
        missileLabel.GetComponent<TextMeshProUGUI>().text = missileCount + "";
    }

    public void UpdateShieldCount(int change) {
        shieldCount = KeepInBounds(shieldCount + change, 0, MAX_ITEM_COUNT);
        UpdateItemImage(shieldCount, shieldSymbol.GetComponent<Image>());
        activeItems[3] = shieldCount;
        UpdateSelectorState();
        shieldLabel.GetComponent<TextMeshProUGUI>().text = shieldCount + "";
    }

    // Method to keep value between given bounds
    private int KeepInBounds(int value, int lowBound, int highBound) {
        if (value > highBound)
            value = highBound;
        else if (value < lowBound)
            value = lowBound;
        
        return value;
    }

    // Change menu item opacity based on value
    private void UpdateItemImage(int itemCount, Image itemImage) {
        if (itemCount == 0) {
            var tempColor = itemImage.color;
            tempColor.a = 0.5f;
            itemImage.color = tempColor;
        } else {
            var tempColor = itemImage.color;
            tempColor.a = 1f;
            itemImage.color = tempColor;
        }
    }

    // Deactivate/activate selector based on items
    private void UpdateSelectorState() {
        bool foundItems = false;

        // Search for any items
        int i;
        for (i = 0; i < activeItems.Length; i ++) {
            if (activeItems[i] > 0) {
                foundItems = true;
                break;
            }
        }

        if (foundItems) {
            // If coming from deactivated, move to active item
            if (!selectorActive)
                UpdateSelectorPosition(i);

            selectorActive = true;
            // Make visible
            var tempColor = itemSelector.GetComponent<Image>().color;
            tempColor.a = 1f;
            itemSelector.GetComponent<Image>().color = tempColor;
        } else {
            selectorActive = false;
            // Make invisible
            var tempColor = itemSelector.GetComponent<Image>().color;
            tempColor.a = 0f;
            itemSelector.GetComponent<Image>().color = tempColor;
        }
    }

    // Move selector to position 0 - 3 (over the four items)
    private void UpdateSelectorPosition(int position) {
        if (position == 0)
            itemSelector.transform.position = bombSymbol.transform.position;
        else if (position == 1)
            itemSelector.transform.position = lightningSymbol.transform.position;
        else if (position == 2)
            itemSelector.transform.position = missileSymbol.transform.position;
        else if (position == 3)
            itemSelector.transform.position = shieldSymbol.transform.position;

        selectorPosition = position;
    }
}