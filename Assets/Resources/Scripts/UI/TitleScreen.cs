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

    // Ship cutscene elements
    [SerializeField]
    private GameObject smallShipPrefab;
    private GameObject smallShip;
    [SerializeField]
    private GameObject bigShipPrefab;
    private GameObject bigShip;

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
    [SerializeField]
    private AudioClip titleSong;
    private AudioSource source;

    // UI state (0 for no interaction, 1 for waiting enter, 2 for selection)
    private int state = 0;
    private bool textFlashing = false;
    // 0 = new game, 1 = quit
    private int selection = 0;

    // Start is called before the first frame update
    void Start()
    {
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
        group.alpha = 0;

        // Remove all text
        enterText.text = "";
        newGame.GetComponent<TextMeshProUGUI>().text = "";
        quit.GetComponent<TextMeshProUGUI>().text = "";

        // Set selector to 0 alpha
        Color temp = selector.GetComponent<Image>().color;
        temp.a = 0;
        selector.GetComponent<Image>().color = temp;

        // Set state
        state = 0;

        source = GetComponent<AudioSource>();

        // Initialize the game!!
        StartCoroutine(FullGameInitialization());
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow player interaction once game is officially initialized
        if (GameSystem.gameInitialized) {
            // Operate UI based on state
            if (state == 0) {
                // Skip cutscene and immediately activate canvas if player presses enter
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
                    // Stop sound effect
                    source.Stop();

                    StartCoroutine(ActivateCanvas());
                }
            } else if (state == 1) {
                // Wait for enter press to move to next state
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
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
    }

    // Fully initialize all important stats of the game on startup!
    // This gains the framerate through delta time and sets up the correct canvas-to-game ratio
    private IEnumerator FullGameInitialization() {
        if (!GameSystem.gameInitialized) {
            // Wait for a short period to avoid weird framerates on startup
            yield return new WaitForSeconds(0.2f);

            // Spend one second gathering 50 pieces of framerate data
            float totalTime = 0;
            float avgFrameRate = 0;
            while (totalTime < 1) {
                avgFrameRate += 1.0f / Time.deltaTime;

                totalTime += 0.02f; // 1 50th of a second
                yield return new WaitForSeconds(0.02f);
            }

            // Average it all out to get core frame rate
            avgFrameRate /= 50;
            GameSystem.FRAME_RATE = avgFrameRate;
            GameSystem.SPEED_DIVISOR = avgFrameRate / GameSystem.FRAME_SPEED_RATIO;
            GameSystem.ROTATION_DIVISOR = avgFrameRate / 60;

            Debug.Log("Frame rate: " + avgFrameRate + "\nSpeed divisor: " + GameSystem.SPEED_DIVISOR);

            // Get canvas size and set ratio

            GameSystem.gameInitialized = true;
        }

        // Create star background
        StartCoroutine(SpawnStars());
        // Play cutscene
        StartCoroutine(PlayTitleCutscene());

        yield break;
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

    // Play the cutscene of the ship flying by and coming around to form the title
    private IEnumerator PlayTitleCutscene() {
        // Play sound effect
        yield return new WaitForSeconds(1);
        if (state != 0)
            yield break;
        GameSystem.PlaySoundEffect(flyby, source, 0);

        // Fly small ship across screen to the right
        yield return new WaitForSeconds(1);
        if (state != 0)
            yield break;
        smallShip = Instantiate(smallShipPrefab, new Vector3(-1 * GameSystem.X_FULL_BOUNDARY - 0.5f, -0.5f, 0), Quaternion.Euler(0, 0, 0));

        // Fly big ship to the left until it aligns with canvas
        yield return new WaitForSeconds(2);
        if (state != 0)
            yield break;
        bigShip = Instantiate(bigShipPrefab, new Vector3(1 * GameSystem.X_FULL_BOUNDARY + 0.5f, 0.05f, 0), Quaternion.Euler(0, 0, 0));

        while (true) {
            if (state != 0)
            yield break;

            if (bigShip.transform.position.x <= -0.75f) {
                Destroy(bigShip);
                StartCoroutine(ActivateCanvas());
                yield break;
            }
            
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Flashes the screen, plays title song, and turns on canvas!
    private IEnumerator ActivateCanvas() {
        // If big/little ships exist, destroy them
        if (bigShip != null)
            Destroy(bigShip);
        if (smallShip != null)
            Destroy(smallShip);

        // Play music
        source.PlayOneShot(titleSong);

        // Change canvas mode and activate title screen
        group.alpha = 1;
        state = 1;

        // Flash screen
        for (int i = 0; i < 2; i ++) {
            Color temp = flash.color;
            temp.a = 1;
            flash.color = temp;

            yield return new WaitForSeconds(0.05f);
            temp = flash.color;
            temp.a = 0;
            flash.color = temp;

            yield return new WaitForSeconds(0.1f);
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
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            if (selection == 0) {
                SceneManager.LoadScene("Level");
            } else if (selection == 1) {
                Application.Quit();
            }
        }
    }
}
