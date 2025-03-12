using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    // Allows player controls
    [SerializeField]
    private bool hasControl = false;

    // Player speed
    [SerializeField]
    private float speed = 4;

    // Player projectiles
    [SerializeField]
    private GameObject laser0;
    [SerializeField]
    private GameObject laser1;
    [SerializeField]
    private GameObject laser2;

    // Explosions in death animation
    [SerializeField]
    private GameObject smallExplosion;
    [SerializeField]
    private GameObject bigExplosion;
    [SerializeField]
    private GameObject shrapnel;

    // Audio sources
    [SerializeField]
    private AudioSource laserSource;
    [SerializeField]
    private AudioSource hitSource;

    // Audio clips
    private AudioClip laserSound;
    private AudioClip hitSound;

    // X coordinates for animation
    private float x;
    private float previousX;

    // Animation elements
    private Animator playerAnimator;
    private string currentAnimation;

    // Constants for turret distance between animations
    private const float TURRET_Y = (float) 0.03;
    private const float TURRET_X = (float) 0.1;
    private const float CLOSE_TURRET_X = (float) 0.07;
    private const float FAR_TURRET_X = (float) 0.09;

    // Collider to use collision physics
    private Collider2D playerCollider;

    private SpriteRenderer playerSprite;

    // Game manager
    private GameObject gameManager;

    // Values dealing with damage and death
    private bool hit = false;
    private bool dead = false;
    private bool iframes = false;
    private const float IFRAME_SECONDS = 1;

    // Start is called before the first frame update
    void Start()
    {
        speed = speed / GameSystem.SPEED_DIVISOR;
        x = transform.position.x;
        previousX = x;

        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        playerSprite = GetComponent<SpriteRenderer>();

        laserSound = Resources.Load<AudioClip>("SoundEffects/Projectiles/BasicLaser");
        hitSound = Resources.Load<AudioClip>("SoundEffects/Damage/Hit");

        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        // Action controls
        if (!dead && hasControl) {
            MovePlayer();

            // Update x coordinates for animate function
            x = transform.position.x;
            AnimatePlayer();
            previousX = x;

            // Get current animation
            currentAnimation = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            Shoot();

            // Check if hit and activate iframes
            if (hit) {
                Invincibility(0);
                hit = false;
            }
        }
    }

    // Move player with arrow keys
    public void MovePlayer() {
        float moveSpeed = speed;
        // Calculate speed for moving diagonal
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)) && 
            (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))) {
                moveSpeed = (float) Math.Sqrt(Math.Pow(moveSpeed, 2) / 2);
        }

        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += MoveWithCollision(new Vector3(moveSpeed, 0, 0));
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position += MoveWithCollision(new Vector3(-1 * moveSpeed, 0, 0));
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += MoveWithCollision(new Vector3(0, moveSpeed, 0));
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position += MoveWithCollision(new Vector3(0, -1 * moveSpeed, 0));
    }


    // Move with respect to collision
    private Vector3 MoveWithCollision(Vector3 direction) {
        Vector3 result = direction;

        // Check for UI object in the way of current movement
        float playerBoundX = transform.position.x + direction.x + MathF.Sign(direction.x) * playerCollider.bounds.extents.x;
        float playerBoundY = transform.position.y + direction.y + MathF.Sign(direction.y) * playerCollider.bounds.extents.y;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(playerBoundX, playerBoundY), 0.0001f);
        
        foreach (Collider2D c in colliders) {
            // Check if it is a barrier
            if (c.gameObject.CompareTag("Barrier")) {

                // If so, snap to position next to it instead of going into it
                result = new Vector3(0, 0, 0);

                if (direction.x != 0) { // Horizontal
                    int sign = MathF.Sign(direction.x);
                    float wall = c.bounds.center.x - sign * c.bounds.extents.x;     // x value of colliding wall
                    float playerWall = transform.position.x + sign * playerCollider.bounds.extents.x; // x value of hitbox wall

                    // Distance between colliders
                    float distance = Math.Abs(wall - playerWall);
                    result += new Vector3(sign * distance, 0, 0);
                }
                
                if (direction.y != 0) { // Vertical
                    int sign = MathF.Sign(direction.y);
                    float wall = c.bounds.center.y - sign * c.bounds.extents.y;     // y value of colliding wall
                    float playerWall = transform.position.y + sign * playerCollider.bounds.extents.y; // y value of hitbox wall

                    // Distance between colliders
                    float distance = Math.Abs(wall - playerWall);
                    result += new Vector3(0, sign * distance, 0);
                }
            }
        }
        

        return result;
    }


    // Tilt the ship if it's going in a certain direction
    public void AnimatePlayer() {
        if (x < previousX) {
            playerAnimator.Play("PlayerLeft");
        } else if (x > previousX) {
            playerAnimator.Play("PlayerRight");
        } else {
            playerAnimator.Play("PlayerIdle");
        }
    }

    // Shoot lasers with spacebar
    public void Shoot() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Decide how to shoot based on BP
            if (gameManager.GetComponent<GameManager>().GetBp() == 1) {
                SpawnLasers(laser0);
                laserSource.pitch = 0.75f;
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0);
            } else if (gameManager.GetComponent<GameManager>().GetBp() == 2) {
                SpawnLasers(laser1);
                laserSource.pitch = 1;
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0);
            } else if (gameManager.GetComponent<GameManager>().GetBp() == 3) {
                SpawnLasers(laser1);
                SpawnLasersWave(laser2);
                laserSource.pitch = 0.5f;
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0);
            }
            else if (gameManager.GetComponent<GameManager>().GetBp() == 4 || gameManager.GetComponent<GameManager>().GetBp() == 5) {
                SpawnLasers(laser1);
                SpawnLasersSpreadshot(laser0);
                SpawnLasersWave(laser2);
                laserSource.pitch = 0.5f;
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0);
            }
        }
    }

    // Get vectors for turret positions based on animation
    private Vector3[] GetTurretPositions() {
        Vector3 leftTurret = transform.position;
        Vector3 rightTurret = transform.position;

        // Check animation to get correct positions
        if (currentAnimation.Equals("PlayerIdle")) {
            leftTurret += new Vector3(-1 * TURRET_X, TURRET_Y);
            rightTurret += new Vector3(TURRET_X, TURRET_Y);
        } else if (currentAnimation.Equals("PlayerLeft")) {
            leftTurret += new Vector3(-1 * CLOSE_TURRET_X, TURRET_Y);
            rightTurret += new Vector3(FAR_TURRET_X, TURRET_Y);
        } else if (currentAnimation.Equals("PlayerRight")) {
            leftTurret += new Vector3(-1 * FAR_TURRET_X, TURRET_Y);
            rightTurret += new Vector3(CLOSE_TURRET_X, TURRET_Y);
        }

        Vector3[] result = {leftTurret, rightTurret};
        return result;
    }

    // Spawn lasers out of turrets
    private void SpawnLasers (GameObject laser) {
        Vector3[] positions = GetTurretPositions();
        Vector3 leftTurret = positions[0];
        Vector3 rightTurret = positions[1];

        // Spawn lasers
        Instantiate(laser, leftTurret, transform.rotation);
        Instantiate(laser, rightTurret, transform.rotation);
    }

    // Spawn lasers out of turrets (WaveLaser)
    private void SpawnLasersWave (GameObject laser) {
        Vector3[] positions = GetTurretPositions();
        Vector3 leftTurret = positions[0];
        Vector3 rightTurret = positions[1];

        // Spawn lasers
        WaveLaser temp = Instantiate(laser, leftTurret, transform.rotation).GetComponent<WaveLaser>();
        temp.SetDirection(-1);
        Instantiate(laser, rightTurret, transform.rotation);
    }

    // Spawn lasers out of turrets in spreadshot
    private void SpawnLasersSpreadshot (GameObject laser) {
        Vector3[] positions = GetTurretPositions();
        Vector3 leftTurret = positions[0];
        Vector3 rightTurret = positions[1];
        float angle1 = 30;
        float angle2 = 60;

        // Spawn lasers
        Instantiate(laser, leftTurret, Quaternion.Euler(0, 0, transform.rotation.z + angle1));
        Instantiate(laser, rightTurret, Quaternion.Euler(0, 0, transform.rotation.z - angle1));
        Instantiate(laser, leftTurret, Quaternion.Euler(0, 0, transform.rotation.z + angle2));
        Instantiate(laser, rightTurret, Quaternion.Euler(0, 0, transform.rotation.z - angle2));
    }

    // Enter invincibility frames if hit
    private void Invincibility(float seconds) {
        // Play hit sound
        GameSystem.PlaySoundEffect(hitSound, hitSource, 0);

        // Set iframes to true
        iframes = true;

        // Flash sprite for length of time
        if (seconds == 0)
            seconds = IFRAME_SECONDS;
        StartCoroutine(InvincibleFlash(seconds));
    }

    // Flash sprite alpha for given seconds, then remove invincibility
    IEnumerator InvincibleFlash(float seconds) {
        StartCoroutine(GameSystem.FlickerSprite(GetComponent<SpriteRenderer>(), seconds));
        yield return new WaitForSeconds(seconds);

        // Set iframes to false
        iframes = false;
    }

    // Play death sequence
    public void PlayDeathSequence() {
        gameManager.GetComponent<GameManager>().UpdateBp(-4);

        // Switch to death animation
        playerAnimator.Play("PlayerDeath");

        // Start flashing!
        Invincibility(10); // Long enough to go until end of animation

        // Put a bunch of random explosions around player
        StartCoroutine(GameSystem.StartExploding(playerSprite, (GameObject) Resources.Load("Prefabs/Explosions/SmallExplosion")));

        // Shoot out shrapnel every time the death animation progresses (animation event)

        // When animation ends, destroy object, create big explosion
        // and shoot out a few pieces of shrapnel (animation event)
    }

    // Shoot out shrapnel in accordance to animation
    public void AnimationHandler(string message) {
        // Shoot shrapnel in proper directions
        if (message.Equals("DeathFrame1")) {
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(315);
        } else if (message.Equals("DeathFrame2")) {
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(180);
        } else if (message.Equals("DeathFrame3")) {
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(75);
        } else if (message.Equals("DeathAnimationEnded")) {
            // Create big explosion and remove player
            Instantiate(bigExplosion, transform.position, transform.rotation);
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(0);
            temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(115);
            temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(270);

            // Teleport out of camera
            transform.position += new Vector3(0, -5, 0);
        }
    }
    

    // GETTERS AND SETTERS
    public bool GetIframes() {
        return iframes;
    }

    public void SetHit(bool hit) {
        this.hit = hit;
    }

    public void SetDead(bool dead) {
        this.dead = dead;
    }
}
