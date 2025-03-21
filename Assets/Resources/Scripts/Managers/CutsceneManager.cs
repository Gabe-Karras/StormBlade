using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class will contain the programming for all in-game cutscenes.
public class CutsceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startMessage;

    // References to other managers
    private GameManager gameManager;
    private ScrollManager scrollManager;
    private UIManager uiManager;
    private MusicManager musicManager;

    // Booleans to control update function
    private bool flyUp = false;
    private bool flyDown = false;
    private bool fadeUI = false;

    // Other values pertaining to cutscene behavior
    private float flyUpSpeed = 8;
    private float flyDownSpeed = 3;
    private float scrollTarget = 0;
    private float scrollChange;
    private float scrollSpeed = 0;
    private float uiFadeSpeed = 0.015f;

    // Player reference
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        flyUpSpeed /= GameSystem.SPEED_DIVISOR;
        flyDownSpeed /= GameSystem.SPEED_DIVISOR;

        // Grab references to managers
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        scrollManager = gameManager.GetScrollManager();
        uiManager = gameManager.GetUIManager();
        musicManager = gameManager.GetMusicManager();

        player = GameObject.Find("Player");
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

        // Fade in Action UI
        if (fadeUI) {
            uiManager.SetActionAlpha(uiManager.GetActionAlpha() + uiFadeSpeed);
            if (uiManager.GetActionAlpha() == 1)
                fadeUI = false;
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

        // Fly ship by, play sound
        StartCoroutine(FlyBy());

        // Start accelerating scroll speed to 3x
        // Slowly lower ship in and even out scroll speed
        StartCoroutine(CatchShip(tempScrollSpeed));

        // Start text, fade in UI
        // Give player control
        StartCoroutine(FadeInUI());
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
        yield return new WaitForSeconds(5);

        scrollTarget = tempScrollSpeed;

        yield return new WaitForSeconds(1.7f);
        flyDown = true;
        
    }

    // Fade in Action UI, display start message
    private IEnumerator FadeInUI() {
        yield return new WaitForSeconds(8);
        fadeUI = true;

        yield return new WaitForSeconds(1);
        Instantiate(startMessage, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
        musicManager.PlayMusic(Resources.Load<AudioClip>("Music/Level" + gameManager.GetLevel()));
        player.GetComponent<PlayerController>().SetHasControl(true);
    }
}
