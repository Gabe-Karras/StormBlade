using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

// This class will contain the programming for all in-game cutscenes.
public class CutsceneManager : MonoBehaviour
{
    // Splash text throughout level
    [SerializeField]
    private GameObject startMessage;
    [SerializeField]
    private GameObject gameOverMessage;
    [SerializeField]
    private GameObject victoryMessage;
    [SerializeField]
    private GameObject screenFade;

    // Song that plays in level intro
    [SerializeField]
    private AudioClip startSong;
    [SerializeField]
    private AudioClip winSong;

    // Constants for action UI bar positions
    private const float BAR_ON_SCREEN = 1.25f;
    private const float BAR_OFF_SCREEN = 1.75f;

    // References to other managers
    private GameManager gameManager;
    private ScrollManager scrollManager;
    private UIManager uiManager;
    private MusicManager musicManager;

    // Booleans to control update function
    private bool flyUp = false;
    private bool flyDown = false;
    private bool goToMiddle = false;

    // Other values pertaining to cutscene behavior
    private float flyUpSpeed = 8;
    private float flyDownSpeed = 3;
    private float flyNormalSpeed = 1;

    private float scrollTarget = 0;
    private float scrollChange;
    private float scrollSpeed = 0;

    private float uiFadeSpeed = 0.015f;
    private float uiTarget = 0;
    private float uiFade = 0;

    private float uiBarSpeed = 0.3f;
    private float uiBarTarget = BAR_ON_SCREEN;

    private float bossSpeed = 0.4f;
    private float bossTarget = GameSystem.BOSS_POSITION;

    // Reference to black bars in action UI
    private GameObject barrierLeft;
    private GameObject barrierRight;

    // Player reference
    private GameObject player;

    // Reference to level boss
    private GameObject boss;

    // Start is called before the first frame update
    void Start()
    {
        flyUpSpeed /= GameSystem.SPEED_DIVISOR;
        flyDownSpeed /= GameSystem.SPEED_DIVISOR;
        flyNormalSpeed /= GameSystem.SPEED_DIVISOR;
        uiBarSpeed /= GameSystem.SPEED_DIVISOR;
        bossSpeed /= GameSystem.SPEED_DIVISOR;

        // Grab references to managers
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        scrollManager = gameManager.GetScrollManager();
        uiManager = gameManager.GetUIManager();
        musicManager = gameManager.GetMusicManager();

        barrierLeft = uiManager.GetBoundaries().transform.Find("BarrierLeft").gameObject;
        barrierRight = uiManager.GetBoundaries().transform.Find("BarrierRight").gameObject;

        player = GameObject.Find("Player");

        if (gameManager.GetGameMode() == 0) {
            uiManager.SetTurnAlpha(0);
        } else {
            uiManager.SetActionAlpha(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Fly ship up off the screen
        if (flyUp) {
            player.transform.position += new Vector3(0, flyUpSpeed, 0);
            if (GameSystem.OutOfBounds(player) && player.transform.position.y > GameSystem.Y_ACTION_BOUNDARY)
                flyUp = false;
        }

        // Fly ship down to center screen
        if (flyDown) {
            player.transform.position += new Vector3(0, -1 * flyDownSpeed, 0);
            flyDownSpeed -= 0.001f;
            if (flyDownSpeed <= 0)
                flyDown = false;
        }

        // Accelerate/Decelerate scroll speed
        if (scrollSpeed != scrollTarget) {
            if (Math.Abs(scrollTarget - scrollSpeed) < scrollChange)
                scrollSpeed = scrollTarget;
            else if (scrollSpeed < scrollTarget)
                scrollSpeed += scrollChange;
            else
                scrollSpeed -= scrollChange;
        }

        scrollManager.SetScrollSpeed(scrollSpeed);

        // Fade Action UI
        if (uiFade < uiTarget) {
            uiManager.SetActionAlpha(uiManager.GetActionAlpha() + uiFadeSpeed);
        } else if (uiFade > uiTarget) {
            uiManager.SetActionAlpha(uiManager.GetActionAlpha() - uiFadeSpeed);
        }

        uiFade = uiManager.GetActionAlpha();

        // Move action bars
        if (barrierLeft.transform.position.x != uiBarTarget * -1) {
            Vector3 current = barrierLeft.transform.position;
            Vector3 destination = new Vector3(uiBarTarget * -1, current.y, 0);
            barrierLeft.transform.position += GameSystem.MoveTowardsPoint(current, destination, uiBarSpeed);
        }
        
        if (barrierRight.transform.position.x != uiBarTarget) {
            Vector3 current = barrierRight.transform.position;
            Vector3 destination = new Vector3(uiBarTarget, current.y, 0);
            barrierRight.transform.position += GameSystem.MoveTowardsPoint(current, destination, uiBarSpeed);
        }

        // Move player to turn-based position
        if (goToMiddle) {
            Vector3 current = player.transform.position;
            Vector3 destination = new Vector3(0, GameSystem.TURN_BASED_Y_POSITION, 0);
            player.transform.position += GameSystem.MoveTowardsPoint(current, destination, flyNormalSpeed);

            if (GameSystem.PointDistance(player.transform.position, destination) == 0)
                goToMiddle = false;
        }

        // Move boss to battle position once bars are gone
        if (boss != null && boss.transform.position.y != bossTarget) {
            Vector3 current = boss.transform.position;
            Vector3 target = new Vector3(0, bossTarget, 0);
            boss.transform.position += GameSystem.MoveTowardsPoint(current, target, bossSpeed);

            // Turn on the menu, start boss music, and officially switch to turn-based mode!
            if (boss.transform.position.y == bossTarget) {
                musicManager.PlayMusic(Resources.Load<AudioClip>("Music/Boss"));
                uiManager.SetTurnAlpha(1);
                gameManager.SwitchGameMode();
            }
        }
    }

    // Execute intro cutscene
    public void IntroCutscene() {
        // Position player accordingly
        player.GetComponent<PlayerController>().SetHasControl(false);
        player.transform.position = new Vector3(0, GameSystem.Y_ACTION_BOUNDARY * -1 - 0.1f, 0);

        // Start scrollmanager at 0 speed and UI at 0 alpha
        float tempScrollSpeed = scrollManager.GetScrollSpeed();
        scrollChange = tempScrollSpeed / 83;
        
        uiManager.SetActionAlpha(0);
        uiManager.SetTurnAlpha(0);

        // Fly ship by, play sound
        StartCoroutine(FlyBy());

        // Start accelerating scroll speed to 3x
        // Slowly lower ship in and even out scroll speed
        StartCoroutine(CatchShip(tempScrollSpeed));

        // Start text, fade in UI
        // Give player control
        StartCoroutine(FadeInUI());
    }

    // Execute transition cutscene between action mode and turn-based mode
    public void TransitionToTurnBased() {
        // Remove control from player and pilot them to correct position
        player.GetComponent<PlayerController>().SetHasControl(false);
        goToMiddle = true;

        // Fade out action UI and stop any music
        uiTarget = 0;
        musicManager.StopAllMusic();

        // Withdraw black bars from the side of screen
        StartCoroutine(MoveBars(BAR_OFF_SCREEN));

        // Bring down the boss!!
        gameManager.SpawnBoss();
        boss = gameManager.GetBoss().gameObject;
    }

    // Create game over text
    public void GameOverCutscene() {
        StartCoroutine(SplashText(gameOverMessage));
    }

    // Play victory music, create victory text, and fly player off screen
    public void VictoryCutscene() {
        StartCoroutine(WinMusic());
    }

    // Fly ship by, play sound, shake screen
    private IEnumerator FlyBy() {

        yield return new WaitForSeconds(0.2f);

        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Other/FlyByShort"), GetComponent<AudioSource>(), 0);
        yield return new WaitForSeconds(0.2f);
        flyUp = true;
        StartCoroutine(scrollManager.ScreenShake());
    }

    // Catch ship with quick scroll speed
    private IEnumerator CatchShip(float tempScrollSpeed) {
        yield return new WaitForSeconds(1);

        scrollTarget = tempScrollSpeed * 3;
        yield return new WaitForSeconds(2);
        // Play intro song
        GetComponent<AudioSource>().PlayOneShot(startSong);

        yield return new WaitForSeconds(3);

        scrollTarget = tempScrollSpeed;

        yield return new WaitForSeconds(1.7f);
        flyDown = true;
        
    }

    // Fade in Action UI, display start message
    private IEnumerator FadeInUI() {
        yield return new WaitForSeconds(8);
        uiTarget = 1;

        yield return new WaitForSeconds(1);
        Instantiate(startMessage, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
        musicManager.PlayMusic(Resources.Load<AudioClip>("Music/Level" + gameManager.GetLevel()));
        player.GetComponent<PlayerController>().SetHasControl(true);
    }

    // Move bars in or out of frame. Will only do it if action ui is transparent
    private IEnumerator MoveBars(float target) {
        while(uiManager.GetActionAlpha() != 0)
            yield return new WaitForSeconds(1);

        uiBarTarget = target;
    }

    // Place splash text on the screen after a few seconds, then fade to transition screen
    private IEnumerator SplashText(GameObject text) {
        yield return new WaitForSeconds(3);
        uiManager.SetActionAlpha(0);
        uiTarget = 0;
        Instantiate(text, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));

        yield return new WaitForSeconds(2);
        Instantiate(screenFade, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        yield return new WaitForSeconds(1.5f);
        // Update data manager before switching scenes
        gameManager.SaveData();

        SceneManager.LoadScene("Transition");
    }

    // Play win music, fly ship out of frame, and display victory text
    private IEnumerator WinMusic() {
        yield return new WaitForSeconds(2);
        GetComponent<AudioSource>().PlayOneShot(winSong);
        yield return new WaitForSeconds(2);
        StartCoroutine(SplashText(victoryMessage));
        yield return new WaitForSeconds(1);
        flyUp = true;
        flyUpSpeed = 3 / GameSystem.SPEED_DIVISOR;
    }
}
