using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This class controls the between levels transition screen
public class TransitionScreen : MonoBehaviour
{
    // Reference to canvas menu
    [SerializeField]
    private GameObject canvas;
    private GameObject mainSelector;
    private GameObject quitSelector;
    private GameObject continueText;
    private GameObject titleText;
    private GameObject yesText;
    private GameObject noText;
    private CanvasGroup mainMenu;
    private CanvasGroup quitMenu;

    // Selection noise
    [SerializeField]
    private AudioClip select;
    private AudioSource source;

    // Either game over or victory
    [SerializeField]
    private GameObject winText;
    [SerializeField]
    private GameObject loseText;

    // Which menu mode is active (0 = main, 1 = quit)
    private int menuMode = 0;

    // State of selectors
    private int mainSelection = 0;
    private int quitSelection = 0;

    // X distance from options for quit menu
    private const float quitDistance = 145;

    // Start is called before the first frame update
    void Start()
    {
        // Create splash text
        if (DataManager.Instance.Won())
            Instantiate(winText, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
        else
            Instantiate(loseText, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));

        // Instantiate canvas
        canvas = Instantiate(canvas);

        // Get canvas elements
        mainSelector = GameObject.Find("MainSelector");
        quitSelector = GameObject.Find("QuitSelector");
        continueText = GameObject.Find("Continue");
        titleText = GameObject.Find("ReturnToTitle");
        yesText = GameObject.Find("Yes");
        noText = GameObject.Find("No");
        mainMenu = GameObject.Find("MainMenu").GetComponent<CanvasGroup>();
        quitMenu = GameObject.Find("QuitMenu").GetComponent<CanvasGroup>();

        // Get reference to audio source
        source = GetComponent<AudioSource>();

        // Ensure menu always starts at mode 0
        SwitchMenuModes(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Update selector positions
        if (mainSelection == 0)
            mainSelector.transform.position = new Vector3(mainSelector.transform.position.x, continueText.transform.position.y, 0);
        else
            mainSelector.transform.position = new Vector3(mainSelector.transform.position.x, titleText.transform.position.y, 0);

        if (quitSelection == 0)
            quitSelector.transform.position = new Vector3(yesText.transform.position.x - quitDistance, quitSelector.transform.position.y, 0);
        else
            quitSelector.transform.position = new Vector3(noText.transform.position.x - quitDistance, quitSelector.transform.position.y, 0);

        // Allow player to use correct selectors
        if (menuMode == 0)
            MoveMainSelector();
        else if (menuMode == 1)
            MoveQuitSelector();
    }

    private void MoveMainSelector() {
        // Move selector up and down with arrow keys
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (mainSelection == 0) {
                mainSelection ++;
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (mainSelection == 1) {
                mainSelection --;
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            // If continue is selected, go to current level
            if (mainSelection == 0) {
                SceneManager.LoadScene("Level");
            }
            // If return to title is selected, open quit menu
            else if (mainSelection == 1) {
                SwitchMenuModes(1);
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }
    }

    private void MoveQuitSelector() {
        // Move selector left and right with arrow keys
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (quitSelection == 0) {
                quitSelection = 1;
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (quitSelection == 1) {
                quitSelection = 0;
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            // If yes is selected, destroy data and return to title screen
            if (quitSelection == 0) {
                Destroy(DataManager.Instance.gameObject);
                SceneManager.LoadScene("Title");
            }
            // If no is selected, return to main menu
            else if (quitSelection == 1) {
                SwitchMenuModes(0);
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }

        // Also return to main menu if player presses escape
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SwitchMenuModes(0);
            GameSystem.PlaySoundEffect(select, source, 0);
        }
    }

    private void SwitchMenuModes(int mode) {
        // Turn on/off correct menus
        if (mode == 0) {
            quitMenu.alpha = 0;
            mainMenu.alpha = 1;
        } else if (mode == 1) {
            quitMenu.alpha = 1;
            mainMenu.alpha = 0;
        }

        menuMode = mode;
    }
}
