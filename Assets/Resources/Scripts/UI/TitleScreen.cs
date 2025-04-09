using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// This class executes everything related to the title screen
public class TitleScreen : MonoBehaviour
{
    // How long to wait between spawning stars
    [SerializeField]
    private int initialStars;
    [SerializeField]
    private float starTime;
    [SerializeField]
    private GameObject star;

    // Canvas elements
    [SerializeField]
    private GameObject titleCanvas;
    private TextMeshProUGUI enterText;
    private GameObject newGame;
    private GameObject quit;
    private GameObject selector;
    private Image flash;
    private CanvasGroup group;

    // Sounds
    [SerializeField]
    private AudioClip select;
    [SerializeField]
    private AudioClip flyby;
    private AudioSource source;

    // UI state (0 for no interaction, 1 for waiting enter, 2 for selection)
    private int state = 0;
    private bool textFlashing = false;
    // 0 = new game, 1 = quit
    private int selection = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Create star background
        StartCoroutine(SpawnStars());

        // Instantiate title screen canvas
        titleCanvas = Instantiate(titleCanvas);

        // Get canvas components
        enterText = titleCanvas.transform.Find("PressEnter").gameObject.GetComponent<TextMeshProUGUI>();
        newGame = titleCanvas.transform.Find("NewGame").gameObject;
        quit = titleCanvas.transform.Find("Quit").gameObject;
        selector = titleCanvas.transform.Find("Selector").gameObject;
        flash = titleCanvas.transform.Find("TitleFlash").gameObject.GetComponent<Image>();
        group = titleCanvas.GetComponent<CanvasGroup>();

        // Set canvas alpha to zero
        //group.alpha = 0;

        // Remove all text
        enterText.text = "";
        newGame.GetComponent<TextMeshProUGUI>().text = "";
        quit.GetComponent<TextMeshProUGUI>().text = "";

        // Set selector to 0 alpha
        Color temp = selector.GetComponent<Image>().color;
        temp.a = 0;
        selector.GetComponent<Image>().color = temp;

        // Set state
        state = 1;

        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Operate UI based on state
        if (state == 1) {
            // Wait for enter press to move to next state
            if (Input.GetKeyDown(KeyCode.Return)) {
                state = 2;
                // Set enter text to blank
                enterText.text = "";
                // Bring forward options text
                newGame.GetComponent<TextMeshProUGUI>().text = "New Game";
                quit.GetComponent<TextMeshProUGUI>().text = "Quit";
                // Make selector visible
                Color temp = selector.GetComponent<Image>().color;
                temp.a = 1;
                selector.GetComponent<Image>().color = temp;
                return;
            }

            // Flash text
            if (!textFlashing)
                StartCoroutine(flashText());
        } else if (state == 2) {
            // Allow player to select options
            MoveSelector();

            // Give selector correct position
            if (selection == 0)
                selector.transform.position = new Vector3(selector.transform.position.x, newGame.transform.position.y, 0);
            else if (selection == 1)
                selector.transform.position = new Vector3(selector.transform.position.x, quit.transform.position.y, 0);
        }
    }

    // Spawn scrolling stars on right side of screen
    private IEnumerator SpawnStars() {
        // Spawn initial wave of stars on screen
        for (int i = 0; i < initialStars; i ++) {
            Vector3 point1 = new Vector3(GameSystem.X_FULL_BOUNDARY, GameSystem.Y_ACTION_BOUNDARY, 0);
            Vector3 point2 = new Vector3(-1 * GameSystem.X_FULL_BOUNDARY, -1 * GameSystem.Y_ACTION_BOUNDARY, 0);
            Vector3 randomPoint = GameSystem.RandomPoint(point1, point2);
            Instantiate(star, randomPoint, Quaternion.Euler(0, 0, 0));
        }

        while (true) {
            // Generate random y position
            Vector3 point1 = new Vector3(GameSystem.X_FULL_BOUNDARY, GameSystem.Y_ACTION_BOUNDARY, 0);
            Vector3 point2 = new Vector3(GameSystem.X_FULL_BOUNDARY, -1 * GameSystem.Y_ACTION_BOUNDARY, 0);
            Vector3 randomPoint = GameSystem.RandomPoint(point1, point2);
            Instantiate(star, randomPoint, Quaternion.Euler(0, 0, 0));

            yield return new WaitForSeconds(starTime);
        }
    }

    // Flash 'press enter' text
    private IEnumerator flashText() {
        textFlashing = true;
        if (enterText.text.Equals(""))
            enterText.text = "Press Enter";
        else
            enterText.text = "";

        yield return new WaitForSeconds(0.5f);
        textFlashing = false;
    }

    // Allow player to move selector
    private void MoveSelector() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (selection == 0) {
                selection ++;
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (selection == 1) {
                selection --;
                GameSystem.PlaySoundEffect(select, source, 0);
            }
        }

        // If enter is pressed, execute the current selection
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (selection == 0) {
                SceneManager.LoadScene("SampleScene");
            } else if (selection == 1) {
                Application.Quit();
            }
        }
    }
}
