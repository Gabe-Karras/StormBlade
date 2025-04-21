using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    // Reference to game manager
    private GameManager gameManager;

    // UI Boundaries
    [SerializeField]
    private GameObject boundaries;

    // Arrow to point at boss parts
    [SerializeField]
    private GameObject enemySelector;

    // Text object to use to display damage
    [SerializeField]
    private GameObject damageText;

    // Selection audio clip
    [SerializeField]
    private AudioClip select;

    // Screenfade when paused
    [SerializeField]
    private GameObject pauseFade;
    [SerializeField]
    private float fadeOpacity;

    // UI canvases
    private GameObject actionCanvas;
    private GameObject turnCanvas;

    // Elements of action canvas
    private GameObject healthCells;
    private GameObject blasterCells;
    private GameObject actionItemSelector;

    private GameObject[] actionItemSymbols;
    private GameObject[] actionItemLabels;

    // Item selection for action mode
    private int[] itemCounts = new int[6];
    private bool actionSelectorActive = false;
    private int actionSelectorPosition = 0;

    // Elements in turn-based mode
    private GameObject healthBar;
    private GameObject healthBarText;
    private GameObject menuSelector;
    private GameObject turnItemSelector;
    private int turnSelectorPosition = 0;
    private int menuSelectorPosition = 0;

    private int healthBarValue;
    private int healthBarSpeed = 2; // How fast the health bar fills up

    private GameObject attackText;
    private GameObject itemText;
    private GameObject descriptionText;

    private GameObject[] turnItemSymbols;
    private GameObject[] turnItemLabels;

    // Constants concerning the selection of components with items
    private const float BOMB_RADIUS = 0.375f;
    private const float LIGHTNING_WIDTH = 0.2f;

    // Keeps track of what menu the player is in during combat
    private int uiMode;

    // Keeps track of player move selection
    // (0 = bomb, 1 = lightning, 2 = missile, 3 = shield, 4 = small repair, 5 = big repair, 6 = normal attack)
    private int playerMove;

    // Player reference
    private GameObject player;
    private PlayerController playerController;

    // Boss components
    private List<BossComponent> bossComponents;
    private int enemySelectorPosition = 0;

    // Start is called before the first frame update
    void Awake()
    {
        // Get game manager
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Create boundaries
        boundaries = Instantiate(boundaries);

        pauseFade = Instantiate(pauseFade);

        // Find canvas and get references to UI objects
        actionCanvas = GameObject.Find("ActionCanvas");
        turnCanvas = GameObject.Find("TurnBasedCanvas");

        // Health/blaster ui
        healthCells = actionCanvas.transform.Find("HealthCells").gameObject;
        blasterCells = actionCanvas.transform.Find("BlasterCells").gameObject;
        healthBar = turnCanvas.transform.Find("HealthBar").gameObject;
        healthBarText = turnCanvas.transform.Find("HealthText").gameObject;

        healthBarValue = gameManager.GetHp();

        // Turn-based menu options
        attackText = turnCanvas.transform.Find("AttackText").gameObject;
        itemText = turnCanvas.transform.Find("ItemText").gameObject;
        descriptionText = turnCanvas.transform.Find("DescriptionText").gameObject;

        // Description text in menu
        descriptionText.GetComponent<TextMeshProUGUI>().text = "";

        // Collect item symbols/labels into arrays
        actionItemSymbols = GetSymbols(actionCanvas);
        actionItemLabels = GetLabels(actionCanvas);
        turnItemSymbols = GetSymbols(turnCanvas);
        turnItemLabels = GetLabels(turnCanvas);

        // Selectors
        actionItemSelector = actionCanvas.transform.Find("Selector").gameObject;
        turnItemSelector = turnCanvas.transform.Find("Selector").gameObject;
        menuSelector = turnCanvas.transform.Find("MenuSelector").gameObject;

        // Get player reference
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();

        SetUIMode(1);

        // Always keep healthbar updated
        StartCoroutine(UpdateHealthBar());
    }

    // Update is called once per frame
    void Update()
    {
        // In action mode, the only UI feature is the item selector
        if (gameManager.GetGameMode() == 0 && !playerController.IsDead() && playerController.HasControl())
            MoveActionSelector();

        // In turn-based mode, there are several layers to the UI functionality
        if (gameManager.GetGameMode() == 1) {
            // In level 0, the player can do nothing
            switch (uiMode) {
                case 1:
                    // In level 1, the player may select between the attack and item options
                    MoveMenuSelector();
                    break;
                case 2:
                    // In level 2, the player may select an item of their choosing
                    MoveTurnSelector();
                    break;
                case 3:
                    // In level 3, the player may select which component of the boss they want to hit
                    MoveEnemySelector();
                    break;
            }
        }
    }

    // Retrieve array of item symbols for given canvas
    private GameObject[] GetSymbols(GameObject canvas) {
        GameObject smallHealthSymbol = canvas.transform.Find("SmallHealthSymbol").gameObject;
        GameObject bigHealthSymbol = canvas.transform.Find("BigHealthSymbol").gameObject;
        GameObject bombSymbol = canvas.transform.Find("BombSymbol").gameObject;
        GameObject lightningSymbol = canvas.transform.Find("LightningSymbol").gameObject;
        GameObject missileSymbol = canvas.transform.Find("MissileSymbol").gameObject;
        GameObject shieldSymbol = canvas.transform.Find("ShieldSymbol").gameObject;

        return new GameObject[6] {bombSymbol, lightningSymbol, missileSymbol, shieldSymbol, smallHealthSymbol, bigHealthSymbol};
    }

    // Retrieve array of item quantity labels for given canvas
    private GameObject[] GetLabels(GameObject canvas) {
        GameObject smallHealthLabel = canvas.transform.Find("SmallHealthCount").gameObject;
        GameObject bigHealthLabel = canvas.transform.Find("BigHealthCount").gameObject;
        GameObject bombLabel = canvas.transform.Find("BombCount").gameObject;
        GameObject lightningLabel = canvas.transform.Find("LightningCount").gameObject;
        GameObject missileLabel = canvas.transform.Find("MissileCount").gameObject;
        GameObject shieldLabel = canvas.transform.Find("ShieldCount").gameObject;

        return new GameObject[6] {bombLabel, lightningLabel, missileLabel, shieldLabel, smallHealthLabel, bigHealthLabel};
    }

    // Change alpha and count of item GUI
    public void UpdateItemImage(int itemCount, string itemName) {
        // Find item image and label from name
        int i;
        for (i = 0; i < actionItemSymbols.Length; i ++) {
            if (itemName.Equals(actionItemSymbols[i].name)) {
                itemCounts[i] = itemCount; // Update array for seletors
                break;
            }
        }

        // Update selector state if the item it's currently on goes dark
        if (actionSelectorPosition == i && itemCount == 0)
            FindNextActionItem();

        if (turnSelectorPosition == i && itemCount == 0)
            FindNextTurnItem(0);

        Image actionItemImage = actionItemSymbols[i].GetComponent<Image>();
        TextMeshProUGUI actionItemLabel = actionItemLabels[i].GetComponent<TextMeshProUGUI>();
        Image turnItemImage = turnItemSymbols[i].GetComponent<Image>();
        TextMeshProUGUI turnItemLabel = turnItemLabels[i].GetComponent<TextMeshProUGUI>();

        // Update alpha of images
        if (itemCount == 0) {
            SetImageTransparent(actionItemImage);
            SetImageTransparent(turnItemImage);
        } else {
            SetImageVisible(actionItemImage);
            SetImageVisible(turnItemImage);
        }

        // Update count labels
        actionItemLabel.text = itemCount + "";
        turnItemLabel.text = itemCount + "";
    }

    // Update health and blaster GUI
    public void UpdateHealthCells() {
        Image img = healthCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/Action/HealthCells")[gameManager.GetHp()];
    }

    public void UpdateBlasterCells() {
        Image img = blasterCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/Action/BlasterCells")[gameManager.GetBp()];
    }

    // Keep player healthbar in turn-based mode active
    private IEnumerator UpdateHealthBar() {
        while (true) {
            healthBarText.GetComponent<TextMeshProUGUI>().text = healthBarValue + "/500";
            healthBar.transform.localScale = new Vector3(1 / 500f * healthBarValue, 1, 1);

            if (Math.Abs(healthBarValue - gameManager.GetHp()) < healthBarSpeed) {
                healthBarValue = gameManager.GetHp();
            } else if (healthBarValue < gameManager.GetHp()) {
                healthBarValue += healthBarSpeed;
            } else if (healthBarValue > gameManager.GetHp()) {
                healthBarValue -= healthBarSpeed;
            }

            // Simulate 60 fps
            yield return new WaitForSeconds(0.015f);
        }
    }

    // Move selector in accordance to action mode
    private void MoveActionSelector() {
        // Check if selection key is pressed
        if (Input.GetKeyDown(KeyCode.S)) {
            if (!actionSelectorActive)
                return; // If it is deactivated
            else {
                // Check for next available item to select (Starting at current selection)
                if (FindNextActionItem()) {
                    GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
                }
            }
        }
    }

    // Move item selector in turn-based menu
    private void MoveTurnSelector() {
        bool playSound = false;

        // Display correct item description
        DisplayItemDescription();

        // Check for key presses and move accordingly
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (FindNextTurnItem(1))
                playSound = true;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (FindNextTurnItem(-1))
                playSound = true;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (FindNextTurnItem(-3))
                playSound = true;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (FindNextTurnItem(3))
                playSound = true;
        }

        if (playSound)
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);

        // If player presses esc, go back
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) {
            SetUIMode(1);
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
            return;
        }

        // If player presses enter, move forward to component selection and set player move
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            playerMove = turnSelectorPosition;
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
            SetUIMode(3);
        }
    }

    // Based on what item is selected, display message in description box
    private void DisplayItemDescription() {
        if (uiMode == 2) {
            string message = "";
    
            switch(turnSelectorPosition) {
                case 0: // bomb
                    message = "Nova Bomb";
                    break;
                case 1: // lightning
                    message = "Hyper Bolt";
                    break;
                case 2: // missile
                    message = "Homing Missiles";
                    break;
                case 3: // shield
                    message = "Energy Shield";
                    break;
                case 4: // small health
                    message = "Small Repair";
                    break;
                case 5: // big health
                    message = "Large Repair";
                    break;
            }

            descriptionText.GetComponent<TextMeshProUGUI>().text = message;
        }
    }

    // Move menu selector for attack/item
    private void MoveMenuSelector() {
        // Use up and down arrows
        if (Input.GetKeyDown(KeyCode.DownArrow) && menuSelectorPosition == 0) {
            // Move down
            menuSelector.transform.position = new Vector3(menuSelector.transform.position.x, itemText.transform.position.y, 0);
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
            menuSelectorPosition = 1;
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && menuSelectorPosition == 1) {
            // Move up
            menuSelector.transform.position = new Vector3(menuSelector.transform.position.x, attackText.transform.position.y, 0);
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
            menuSelectorPosition = 0;
        }

        // Enter/Space to confirm
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            if (menuSelectorPosition == 0) {
                // Switch straight to mode three with intent to attack
                GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
                playerMove = 6; // Normal attack
                SetUIMode(3);
            } else {
                // Switch to item selection if there are items to be selected
                bool foundItems = false;
                for (int i = 0; i < itemCounts.Length; i ++) {
                    if (itemCounts[i] > 0) {
                        foundItems = true;
                        break;
                    }
                }

                if (foundItems) {
                    SetUIMode(2);
                    GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
                }
            }
        }
    }

    // Move flashing arrow around to different components of boss
    private void MoveEnemySelector() {
        // If none yet exist, create selectors on the selected components
        List<GameObject> selected = SelectComponents();
        if (GameObject.FindGameObjectsWithTag("EnemySelector").Length == 0) {
            for (int i = 0; i < selected.Count; i ++) {
                GameObject temp = Instantiate(enemySelector, selected[i].transform.position, Quaternion.Euler(0, 0, 0));

                // Keep main arrow solid
                if (selected[i].Equals(bossComponents[enemySelectorPosition].gameObject) || selected[i].Equals(player))
                    temp.GetComponent<Flicker>().SetFlashTime(0);
            }

            // Display description of current component unless player is selecting themselves
            if (playerMove != 3 && playerMove != 4 && playerMove != 5)
                descriptionText.GetComponent<TextMeshProUGUI>().text = bossComponents[enemySelectorPosition].GetName();
            else
                descriptionText.GetComponent<TextMeshProUGUI>().text = "StormBlade";
        }

        int movement = 0;

        // When arrow keys are pressed, move selection back and forth
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            movement = 1;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            movement = -1;
        }


        if (movement != 0) {
            DestroyEnemySelectors();
            enemySelectorPosition += movement;
            if (enemySelectorPosition >= bossComponents.Count)
                enemySelectorPosition = 0;
            else if (enemySelectorPosition < 0)
                enemySelectorPosition = bossComponents.Count - 1;
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
        }

        // Back out to previous menu if esc is pressed
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) {
            GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);

            // Back out to correct menu
            if (playerMove == 6)
                SetUIMode(1);
            else
                SetUIMode(2);
            return;
        }

        // If enter is pressed, attack!
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            // Don't create a new shield if one already exists
            if (playerMove != 3 || (playerMove == 3 && gameManager.GetActiveShield() == null)) {
                playerController.PerformMove(playerMove, bossComponents[enemySelectorPosition].gameObject, selected);
                SetUIMode(0);
                GameSystem.PlaySoundEffect(select, GetComponent<AudioSource>(), 0);
            }
        }
    }

    // Destroys all existing enemy selectors
    private void DestroyEnemySelectors() {
        GameObject[] selectors = GameObject.FindGameObjectsWithTag("EnemySelector");

        for (int i = 0; i < selectors.Length; i ++) {
            Destroy(selectors[i]);
        }
    }

    // Return list of components to select with arrow based on player move
    private List<GameObject> SelectComponents() {
        // Get currently selected component
        GameObject current = bossComponents[enemySelectorPosition].gameObject;
        List<GameObject> result = new List<GameObject>();

        // Bomb: check radius of explosion sprite
        if (playerMove == 0) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(current.transform.position.x, current.transform.position.y), BOMB_RADIUS);

            // Sort through all colliders and keep any that belong to active components
            for (int i = 0; i < colliders.Length; i ++) {
                if (bossComponents.Contains(colliders[i].gameObject.GetComponent<BossComponent>()))
                    result.Add(colliders[i].gameObject);
            }
        }
        // Lighting: check vertical rectangle
        else if (playerMove == 1) {
            // Get points for overlap rectangle
            // Top left
            Vector3 point1 = new Vector3(current.transform.position.x - LIGHTNING_WIDTH / 2, GameSystem.Y_ACTION_BOUNDARY, 0);
            // Bottom right
            Vector3 point2 = new Vector3(current.transform.position.x + LIGHTNING_WIDTH / 2, player.transform.position.y, 0);

            Collider2D[] colliders = Physics2D.OverlapAreaAll(point1, point2);

            // Sort through all colliders and keep any that belong to active components
            for (int i = 0; i < colliders.Length; i ++) {
                if (bossComponents.Contains(colliders[i].gameObject.GetComponent<BossComponent>()))
                    result.Add(colliders[i].gameObject);
            }
        }
        // Missile: select all active components
        else if (playerMove == 2) {
            for (int i = 0; i < bossComponents.Count; i ++) {
                result.Add(bossComponents[i].gameObject);
            }
        }
        // Shield and heals: select player
        else if (playerMove == 3 || playerMove == 4 || playerMove == 5) {
            result.Add(player);
        }
        // Basic attack: just select the current component
        else if (playerMove == 6) {
            result.Add(current);
        }

        return result;
    }

    // From the current selector position, find if there is a next available item index to snap to
    // If next item is not found, return false
    private bool FindNextActionItem() {
        // Check for next available item to select (Starting at current selection)
        int offset = actionSelectorPosition + 1;
        int[] actionItems = GetActionItems();

        for (int i = 0; i < actionItems.Length - 1; i ++) {
            if (actionItems[(i + offset) % actionItems.Length] != 0) {
                UpdateActionSelectorPosition((i + offset) % actionItems.Length);
                return true;
            }
        }

        return false;
    }

    // Find next item to latch to in turn-based menu. Takes parameter for where to search.
    // (1 = right, -1 = left, -3 = up, 3 = down, 0 = everywhere)
    // Returns false if no other item is found
    private bool FindNextTurnItem(int search) {
        // Set up loop variables in case of lateral search
        int loopStart = turnSelectorPosition + search;
        int loopEnd;
        int loopMovement = MathF.Sign(search);
        if (loopMovement == 0)
            loopMovement = 1;

        // Don't search if command is out of bounds
        if (loopStart >= 0 && loopStart < 6) {
            // Search to the right
            if (search == 1)
                loopEnd = turnSelectorPosition / 3 * 3 + 3;
            // Search to the left
            else if (search == -1)
                loopEnd = turnSelectorPosition / 3 * 3 - 1;
            // Search up
            else if (search == -3) {
                // First, check if immediate direction has anything
                if (itemCounts[loopStart] > 0) {
                    UpdateTurnSelectorPosition(loopStart);
                    return true;
                }

                // Then search whole top row
                loopStart = 2;
                loopEnd = -1;
            }
            else if (search == 3) {
                if (itemCounts[loopStart] > 0) {
                    UpdateTurnSelectorPosition(loopStart);
                    return true;
                }

                loopStart = 3;
                loopEnd = 6;
            }
            // Search everywhere!!
            else {
                loopStart = 0;
                loopEnd = 6;
            }

            int i;
            for (i = loopStart; i != loopEnd; i += loopMovement) {
                if (itemCounts[i] > 0) {
                    UpdateTurnSelectorPosition(i);
                    return true;
                }
            }
        }

        return false;
    }

    // Returns an array of itemcounts only relevant to action mode
    private int[] GetActionItems() {
        int[] result = new int[4];

        for (int i = 0; i < itemCounts.Length - 2; i ++) {
            result[i] = itemCounts[i];
        }

        return result;
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
        int[] actionItems = GetActionItems();

        // Search for any items
        int i;
        for (i = 0; i < actionItems.Length; i ++) {
            if (actionItems[i] > 0) {
                foundItems = true;
                break;
            }
        }

        if (foundItems) {
            // If coming from deactivated, move to active item
            if (!actionSelectorActive)
                UpdateActionSelectorPosition(i);

            actionSelectorActive = true;
            // Make visible
            Color tempColor = actionItemSelector.GetComponent<Image>().color;
            tempColor.a = 1f;
            actionItemSelector.GetComponent<Image>().color = tempColor;
        } else {
            actionSelectorActive = false;
            // Make invisible
            Color tempColor = actionItemSelector.GetComponent<Image>().color;
            tempColor.a = 0f;
            actionItemSelector.GetComponent<Image>().color = tempColor;
        }
    }

    // Move selector to position 0 - 3 (over the four items)
    private void UpdateActionSelectorPosition(int position) {
        actionItemSelector.transform.position = actionItemSymbols[position].transform.position;
        actionSelectorPosition = position;
    }

    // Move selector over correct item in menu
    private void UpdateTurnSelectorPosition(int position) {
        turnItemSelector.transform.position = turnItemSymbols[position].transform.position;
        turnSelectorPosition = position;
    }

    // Get the item index the selector is currently on. -1 if it is deactivated
    public int GetSelectorState() {
        if (gameManager.GetGameMode() == 0)
            return GetActionSelectorState();
        
        return -1;
    }

    public int GetActionSelectorState() {
        if (actionItemSelector.GetComponent<Image>().color.a != 0)
            return actionSelectorPosition;
        
        return -1;
    }

    // Set alpha value for all action canvas elements
    public void SetActionAlpha(float alpha) {
        actionCanvas.GetComponent<CanvasGroup>().alpha = alpha;
    }

    public float GetActionAlpha() {
        return actionCanvas.GetComponent<CanvasGroup>().alpha;
    }

    // Alpha value for turn-based menu
    public void SetTurnAlpha(float alpha) {
        turnCanvas.GetComponent<CanvasGroup>().alpha = alpha;
    }

    // Set pause shade correctly
    public void UpdatePauseFade() {
        Color temp = pauseFade.GetComponent<SpriteRenderer>().color;

        if (!GameSystem.IsPaused())
            temp.a = 0;
        else
            temp.a = fadeOpacity;

        pauseFade.GetComponent<SpriteRenderer>().color = temp;
    }
 
    // Changes ui mode in turn-based and updates alphas accordingly
    public void SetUIMode(int level) {

        descriptionText.GetComponent<TextMeshProUGUI>().text = "";
        DestroyEnemySelectors();

        switch (level) {
            case 0:
                // Make all elements invisible
                SetImageInvisible(menuSelector.GetComponent<Image>());
                SetImageInvisible(turnItemSelector.GetComponent<Image>());
                break;
            
            case 1:
                // Only menu selector is visible
                SetImageVisible(menuSelector.GetComponent<Image>());
                SetImageInvisible(turnItemSelector.GetComponent<Image>());
                break;
            
            case 2:
                // Only item selector is visible
                
                // On opening item menu selection bugfix
                if (turnSelectorPosition == 0 && gameManager.GetBombCount() == 0)
                    FindNextTurnItem(0);

                SetImageInvisible(menuSelector.GetComponent<Image>());
                SetImageVisible(turnItemSelector.GetComponent<Image>());
                break;

            case 3:
                // Make all elements invisible
                SetImageInvisible(menuSelector.GetComponent<Image>());
                SetImageInvisible(turnItemSelector.GetComponent<Image>());

                // Get updated list of components
                bossComponents = gameManager.GetBoss().GetActiveComponents();
                enemySelectorPosition = 0;
                break;
        }

        uiMode = level;
    }

    // Set any UI image to be invisible
    private void SetImageInvisible(Image img) {
        Color tempColor = img.color;
        tempColor.a = 0;
        img.color = tempColor;
    }

    // Set any UI image to be visible
    private void SetImageVisible(Image img) {
        Color tempColor = img.color;
        tempColor.a = 1;
        img.color = tempColor;
    }

    // Set UI image to be half transparent
    private void SetImageTransparent(Image img) {
        Color tempColor = img.color;
        tempColor.a = 0.5f;
        img.color = tempColor;
    }

    // Returns the parent UI boundaries object
    public GameObject GetBoundaries() {
        return boundaries;
    }

    // Returns turn-based canvas
    public GameObject GetTurnCanvas() {
        return turnCanvas;
    }

    // Returns damage text prefab
    public GameObject GetDamageText() {
        return damageText;
    }

    // Instantly set current healthbar value
    public void SetHealthBarValue(int value) {
        healthBarValue = value;
    }
}
