using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
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
        // Divide for vector math
        speed = speed / 50;

        x = transform.position.x;
        previousX = x;

        playerAnimator = GetComponent<Animator>();

        playerCollider = GetComponent<Collider2D>();

        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        // Action controls
        if (!dead) {
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
                Invincibility();
                hit = false;
            }
        }
    }

    // Move player with arrow keys
    public void MovePlayer() {
        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += MoveWithCollision(new Vector3(speed, 0, 0));
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position += MoveWithCollision(new Vector3(-1 * speed, 0, 0));
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += MoveWithCollision(new Vector3(0, speed, 0));
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position += MoveWithCollision(new Vector3(0, -1 * speed, 0));
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
            if (gameManager.GetComponent<GameManager>().GetBP() == 1)
                SpawnLasers(laser0);
            else if (gameManager.GetComponent<GameManager>().GetBP() == 2)
                SpawnLasers(laser1);
            else if (gameManager.GetComponent<GameManager>().GetBP() == 3) {
                SpawnLasers(laser1);
                SpawnLasersWave(laser2);
            }
            else if (gameManager.GetComponent<GameManager>().GetBP() == 4 || gameManager.GetComponent<GameManager>().GetBP() == 5) {
                SpawnLasers(laser1);
                SpawnLasersSpreadshot(laser0);
                SpawnLasersWave(laser2);
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
    private void Invincibility() {
        // Set iframes to true
        iframes = true;

        // Flash sprite for length of time
        StartCoroutine(FlashSprite(IFRAME_SECONDS));
    }

    // Flash sprite alpha for given seconds
    IEnumerator FlashSprite(float seconds) {
        float total = 0f;
        float flashTime = 0.05f; // 20th of a second
        Color temp = GetComponent<SpriteRenderer>().color;

        // Loop for iframe time
        while (total < seconds) {
            // Flash alpha color
            if (temp.a == 1)
                temp.a = 0f;
            else 
                temp.a = 1f;

            GetComponent<SpriteRenderer>().color = temp;

            // Wait for a 20th of a second
            yield return new WaitForSeconds(flashTime);

            // Add time to total
            total += flashTime;
        }

        // If alpha is zero after loop, set to 1
        if (temp.a == 0) {
            temp.a = 1;
            GetComponent<SpriteRenderer>().color = temp;
        }

        // Set iframes to false
        iframes = false;
    }

    // Play death sequence
    public void PlayDeathSequence() {
        // Switch to death animation
        playerAnimator.Play("PlayerDeath");

        // Start flashing!
        StartCoroutine(FlashSprite(10)); // Long enough to go until end of animation

        // Put a bunch of random explosions around player
        StartCoroutine(SpawnExplosions());

        // Shoot out shrapnel every time the death animation progresses (animation event)

        // When animation ends, destroy object, create big explosion
        // and shoot out a few pieces of shrapnel (animation event)
    }

    // Spawn a random explosion somewhere on the player
    private void RandomExplosion() {
        System.Random rand = new System.Random();
        float explosionX = ((float) rand.Next(-15, 16)) / 100;
        float explosionY = ((float) rand.Next(-12, 13)) / 100;

        Instantiate(smallExplosion, transform.position + new Vector3(explosionX, explosionY), transform.rotation);
    }

    // Put a bunch of random explosions around player
    IEnumerator SpawnExplosions() {
        float explodeTime = 0.2f; // 5th of a second

        while (true) {
            RandomExplosion();
            yield return new WaitForSeconds(explodeTime);
        }
    }

    // Shoot out shrapnel in accordance to animation
    public void AnimationHandler(string message) {
        // Shoot shrapnel in proper directions
        if (message.Equals("DeathFrame1")) {
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(45);
        } else if (message.Equals("DeathFrame2")) {
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(180);
        } else if (message.Equals("DeathFrame3")) {
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(285);
        } else if (message.Equals("DeathAnimationEnded")) {
            // Create big explosion and destroy player
            Instantiate(bigExplosion, transform.position, transform.rotation);
            GameObject temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(0);
            temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(115);
            temp = Instantiate(shrapnel, transform.position, transform.rotation);
            temp.GetComponent<Shrapnel>().SetAngle(270);
            Destroy(gameObject);
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
