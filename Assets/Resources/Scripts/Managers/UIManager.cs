using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Reference to game manager
    private GameManager gameManager;

    // UI canvases
    [SerializeField]
    private GameObject actionCanvas;
    [SerializeField]
    private float actionAlpha;

    // UI Boundaries
    [SerializeField]
    private GameObject boundaries;

    // Elements of action canvas
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

    private GameObject[] itemSymbols;
    private GameObject[] itemLabels;

    // Item selection
    private int[] activeItems = new int[4];
    private bool selectorActive = false;
    private int selectorPosition = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Get game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Create boundaries
        boundaries = Instantiate(boundaries);

        // Create canvas and get references to UI objects
        actionCanvas = Instantiate(actionCanvas);

        healthCells = actionCanvas.transform.Find("HealthCells").gameObject;
        blasterCells = actionCanvas.transform.Find("BlasterCells").gameObject;

        smallHealthSymbol = actionCanvas.transform.Find("SmallHealthSymbol").gameObject;
        bigHealthSymbol = actionCanvas.transform.Find("BigHealthSymbol").gameObject;
        bombSymbol = actionCanvas.transform.Find("BombSymbol").gameObject;
        lightningSymbol = actionCanvas.transform.Find("LightningSymbol").gameObject;
        missileSymbol = actionCanvas.transform.Find("MissileSymbol").gameObject;
        shieldSymbol = actionCanvas.transform.Find("ShieldSymbol").gameObject;

        smallHealthLabel = actionCanvas.transform.Find("SmallHealthCount").gameObject;
        bigHealthLabel = actionCanvas.transform.Find("BigHealthCount").gameObject;
        bombLabel = actionCanvas.transform.Find("BombCount").gameObject;
        lightningLabel = actionCanvas.transform.Find("LightningCount").gameObject;
        missileLabel = actionCanvas.transform.Find("MissileCount").gameObject;
        shieldLabel = actionCanvas.transform.Find("ShieldCount").gameObject;

        itemSelector = actionCanvas.transform.Find("Selector").gameObject;

        // Set canvas alphas
        actionCanvas.GetComponent<CanvasGroup>().alpha = actionAlpha;

        // Set lists of items
        itemSymbols = new GameObject[6] {smallHealthSymbol, bigHealthSymbol, bombSymbol, lightningSymbol, missileSymbol, shieldSymbol};
        itemLabels = new GameObject[6] {smallHealthLabel, bigHealthLabel, bombLabel, lightningLabel, missileLabel, shieldLabel};
    }

    // Update is called once per frame
    void Update()
    {
        // In action mode, the only UI feature is the item selector
        if (gameManager.GetGameMode() == 0)
            MoveActionSelector();
    }

    // Change alpha and count of item GUI
    public void UpdateItemImage(int itemCount, string itemName) {
        // Find item image and label from name
        int i;
        for (i = 0; i < itemSymbols.Length; i ++) {
            if (itemName.Equals(itemSymbols[i].name)) {
                if (i > 1)
                    activeItems[i - 2] = itemCount; // Update array for seletor

                break;
            }
        }

        Image itemImage = itemSymbols[i].GetComponent<Image>();
        TextMeshProUGUI itemLabel = itemLabels[i].GetComponent<TextMeshProUGUI>();

        // Update alpha of image
        if (itemCount == 0) {
            Color tempColor = itemImage.color;
            tempColor.a = 0.5f;
            itemImage.color = tempColor;
        } else {
            Color tempColor = itemImage.color;
            tempColor.a = 1f;
            itemImage.color = tempColor;
        }

        // Update count label
        itemLabel.text = itemCount + "";
    }

    // Update health and blaster GUI
    public void UpdateHealthCells() {
        Image img = healthCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/HealthCells")[gameManager.GetHp()];
    }

    public void UpdateBlasterCells() {
        Image img = blasterCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/BlasterCells")[gameManager.GetBp()];
    }

    // Move selector in accordance to action mode
    private void MoveActionSelector() {
        // Check if selection key is pressed
        if (Input.GetKeyDown(KeyCode.S)) {
            if (!selectorActive)
                return; // If it is deactivated
            else {
                // Check for next available item to select (Starting at current selection)
                int offset = selectorPosition + 1;
                for (int i = 0; i < activeItems.Length - 1; i ++) {
                    if (activeItems[(i + offset) % activeItems.Length] != 0) {
                        UpdateActionSelectorPosition((i + offset) % activeItems.Length);
                        break;
                    }
                }
            }
        }
    }

    // Hollow method that executes the right one based on game mode
    public void UpdateSelectorState() {
        if (gameManager.GetGameMode() == 0) {
            UpdateActionSelectorState();
        }
    }

    // Deactivate/activate selector based on items
    private void UpdateActionSelectorState() {
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
                UpdateActionSelectorPosition(i);

            selectorActive = true;
            // Make visible
            Color tempColor = itemSelector.GetComponent<Image>().color;
            tempColor.a = 1f;
            itemSelector.GetComponent<Image>().color = tempColor;
        } else {
            selectorActive = false;
            // Make invisible
            Color tempColor = itemSelector.GetComponent<Image>().color;
            tempColor.a = 0f;
            itemSelector.GetComponent<Image>().color = tempColor;
        }
    }

    // Move selector to position 0 - 3 (over the four items)
    private void UpdateActionSelectorPosition(int position) {
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
