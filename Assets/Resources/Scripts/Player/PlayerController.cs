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

    // Items
    [SerializeField]
    private GameObject bomb;
    [SerializeField]
    private GameObject lightning;
    [SerializeField]
    private GameObject missile;
    [SerializeField]
    private GameObject shield;

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
    [SerializeField]
    private AudioSource pickupSource;
    [SerializeField]
    private AudioSource healSource;

    // Audio clips
    private AudioClip laserSound;
    private AudioClip hitSound;
    private AudioClip pickupSound;
    private AudioClip healSound;
    private AudioClip pauseSound;

    // X coordinates for animation
    private float x;
    private float previousX;

    // This is the threshold distance for what to animate or not
    private const float MINIMAL_DISTANCE = 0.0001f;

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
    private GameManager gameManager;
    private MusicManager musicManager;
    private UIManager uiManager;
    private CutsceneManager cutsceneManager;

    // Player moves class
    private PlayerMoves playerMoves;

    // Values dealing with damage and death
    private bool hit = false;
    private bool dead = false;
    private bool iframes = false;
    private const float IFRAME_SECONDS = 1;

    // Broken materials bugfix
    private bool isFlashing = false;

    private bool initializing = true;

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
        pickupSound = Resources.Load<AudioClip>("SoundEffects/Items/Pickup");
        healSound = Resources.Load<AudioClip>("SoundEffects/Items/Heal");
        pauseSound = Resources.Load<AudioClip>("SoundEffects/Other/Select");
    }

    // Update is called once per frame
    void Update()
    {
        // Initialization on the first update frame to give game manager time to initialize
        if (initializing) {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            musicManager = gameManager.GetMusicManager();
            uiManager = gameManager.GetUIManager();
            cutsceneManager = gameManager.GetCutsceneManager();
            playerMoves = GetComponent<PlayerMoves>();

            initializing = false;
        }

        // Animate/control when alive
        if (!dead) {

            // Action controls
            if (hasControl) {
                PauseGame();

                if (!GameSystem.IsPaused()) {
                    MovePlayer();

                    // Get current animation
                    currentAnimation = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                    Shoot();

                    UseItem();

                    // Check if hit and activate iframes
                    if (hit) {
                        Invincibility(0);
                        hit = false;
                    }
                }
            }

            // Update x coordinates for animate function
            x = transform.position.x;
            AnimatePlayer();
            previousX = x;
        }
    }

    // Handle item pickups
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Item")) {
            // Figure out which var to add to
            string item = other.gameObject.GetComponent<Pickup>().GetItem();

            switch (item) {
                case "blaster":
                    gameManager.UpdateBp(1);
                    break;
                case "smallHealth":
                    gameManager.UpdateSmallHealthCount(1);
                    break;
                case "bigHealth":
                    gameManager.UpdateBigHealthCount(1);
                    break;
                case "bomb":
                    gameManager.UpdateBombCount(1);
                    break;
                case "lightning":
                    gameManager.UpdateLightningCount(1);
                    break;
                case "missile":
                    gameManager.UpdateMissileCount(1);
                    break;
                case "shield":
                    gameManager.UpdateShieldCount(1);
                    break;
            }

            // Flash player sprite and destroy pickup
            if (!isFlashing) {
                StartCoroutine(GameSystem.FlashSprite(playerSprite, Resources.Load<Material>("Materials/SolidWhite"), time: 0.05f));
                isFlashing = true;
                StartCoroutine(FlashTimer(0.06f));
            }
            Destroy(other.gameObject);

            // Play sound effect
            GameSystem.PlaySoundEffect(pickupSound, pickupSource, 0);
        }
    }

    // Method to reset flash variable. Call after flashing sprite
    private IEnumerator FlashTimer(float seconds) {
        yield return new WaitForSeconds(seconds);

        isFlashing = false;
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
        if (x < previousX && previousX - x > MINIMAL_DISTANCE) {
            playerAnimator.Play("PlayerLeft");
        } else if (x > previousX && x - previousX > MINIMAL_DISTANCE) {
            playerAnimator.Play("PlayerRight");
        } else {
            playerAnimator.Play("PlayerIdle");
        }
    }

    // Shoot lasers with spacebar
    public void Shoot() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Decide how to shoot based on BP
            if (gameManager.GetBp() == 1) {
                SpawnLasers(laser0);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0, pitch: 0.75f);
            } else if (gameManager.GetBp() == 2) {
                SpawnLasers(laser1);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0);
            } else if (gameManager.GetBp() == 3) {
                SpawnLasers(laser1);
                SpawnLasersWave(laser2);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0, pitch: 0.5f);
            }
            else if (gameManager.GetBp() == 4 || gameManager.GetBp() == 5) {
                SpawnLasers(laser1);
                SpawnLasersSpreadshot(laser0);
                SpawnLasersWave(laser2);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0, pitch: 0.5f);
            }
        }
    }

    // Pause game when enter is pressed
    private void PauseGame() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            // If game is running, pause
            if (!GameSystem.IsPaused()) {
                // Set timescale to 0
                Time.timeScale = 0;

                // Set pause boolean
                GameSystem.SetPaused(true);

                // Pause music
                musicManager.PauseMusic();

                // Activate pause fade
                uiManager.UpdatePauseFade();
            }
            // If game is paused, resume
            else {
                Time.timeScale = 1;
                GameSystem.SetPaused(false);
                musicManager.ResumeMusic();
                uiManager.UpdatePauseFade();
            }

            // Play pause sound effect
            GameSystem.PlaySoundEffect(pauseSound, pickupSource, 0);
        }
    }

    // Perform move from PlayerMoves class in turn-based combat
    public void PerformMove(int move, GameObject singleTarget, List<GameObject> manyTargets) {
        switch(move) {
            case 0: // Nova bomb
                StartCoroutine(playerMoves.Bomb(singleTarget, manyTargets));
                gameManager.UpdateBombCount(-1);
                break;
            case 1: // Hyper bolt
                StartCoroutine(playerMoves.Lightning(singleTarget, manyTargets));
                gameManager.UpdateLightningCount(-1);
                break;
            case 2: // Homing missiles
                StartCoroutine(playerMoves.Missile(manyTargets));
                gameManager.UpdateMissileCount(-1);
                break;
            case 3: // Energy shield
                StartCoroutine(playerMoves.Shield());
                gameManager.UpdateShieldCount(-1);
                break;
            case 4: // Small repair
                StartCoroutine(playerMoves.SmallHealth());
                gameManager.UpdateSmallHealthCount(-1);
                break;
            case 5: // Big repair
                StartCoroutine(playerMoves.BigHealth());
                gameManager.UpdateBigHealthCount(-1);
                break;
            case 6: // Normal attack
                StartCoroutine(playerMoves.Attack(singleTarget));
                break;
        }
    }

    // Get vectors for turret positions based on animation
    public Vector3[] GetTurretPositions() {
        Vector3 leftTurret = transform.position;
        Vector3 rightTurret = transform.position;

        leftTurret += new Vector3(-1 * TURRET_X, TURRET_Y);
        rightTurret += new Vector3(TURRET_X, TURRET_Y);
        
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

    // If player presses item key, deploy the item that is currently selected
    private void UseItem() {
        // Heal items have cooldown
        if (playerSprite.material != Resources.Load<Material>("Materials/SolidRed")) {
            // Small heal
            if (Input.GetKeyDown(KeyCode.E)) {
                if (gameManager.GetSmallHealthCount() > 0) {
                    gameManager.UpdateHp(2);
                    gameManager.UpdateSmallHealthCount(-1);
                    PlayHealAnimation(0.1f);
                }
            }

            // Big heal
            if (Input.GetKeyDown(KeyCode.W)) {
                if (gameManager.GetBigHealthCount() > 0) {
                    gameManager.UpdateHp(4);
                    gameManager.UpdateBigHealthCount(-1);
                    PlayHealAnimation(0.2f);
                }
            }
        }


        // Other items
        if (Input.GetKeyDown(KeyCode.D)) {
            UIManager ui = gameManager.GetUIManager();

            switch (ui.GetSelectorState()) {
                case 0: // Bomb
                    Instantiate(bomb, transform.position + new Vector3(0, 11 / GameSystem.PIXELS_PER_UNIT, 0), transform.rotation);
                    gameManager.UpdateBombCount(-1);
                    break;
                case 1: // Lightning
                    GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Lightning"), laserSource, 0, volume: 0.5f);
                    Instantiate(lightning, transform.position + new Vector3(-10 / GameSystem.PIXELS_PER_UNIT, 0, 0), Quaternion.Euler(0, 0, 90));
                    Instantiate(lightning, transform.position + new Vector3(0, -10 / GameSystem.PIXELS_PER_UNIT, 0), Quaternion.Euler(0, 0, 180));
                    Instantiate(lightning, transform.position + new Vector3(10 / GameSystem.PIXELS_PER_UNIT, 0, 0), Quaternion.Euler(0, 0, 270));
                    gameManager.UpdateLightningCount(-1);
                    break;
                case 2: // Missile
                    GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Missile"), laserSource, 0, volume: 0.35f);
                    Instantiate(missile, transform.position + new Vector3(-10 / GameSystem.PIXELS_PER_UNIT, 0, 0), Quaternion.Euler(0, 0, 45));
                    Instantiate(missile, transform.position + new Vector3(0, 5 / GameSystem.PIXELS_PER_UNIT, 0), Quaternion.Euler(0, 0, 0));
                    Instantiate(missile, transform.position + new Vector3(10 / GameSystem.PIXELS_PER_UNIT, 0, 0), Quaternion.Euler(0, 0, 315));
                    gameManager.UpdateMissileCount(-1);
                    break;
                case 3: // Shield
                    // Only spawn shield if one is not already active
                    if (gameManager.GetActiveShield() == null) {
                        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Shield"), laserSource, 0, volume: 0.5f);
                        gameManager.SetActiveShield(Instantiate(shield, transform.position, transform.rotation));
                        gameManager.UpdateShieldCount(-1);
                    }
                    break;
            }
        }
    }

    // Flash sprite red, emit particles, and play heal sound
    public void PlayHealAnimation(float particleTime) {
        if (!isFlashing) {
            StartCoroutine(GameSystem.FlashSprite(playerSprite, Resources.Load<Material>("Materials/SolidRed"), time: particleTime + 0.2f));
            isFlashing = true;
            StartCoroutine(FlashTimer(particleTime + 0.21f));
        }
            
        StartCoroutine(GameSystem.EmitParticles(gameObject, Resources.Load<GameObject>("Prefabs/Explosions/RedParticle"), 0, 0, particleTime));
        GameSystem.PlaySoundEffect(healSound, healSource, 0);
    }

    // Enter invincibility frames if hit
    public void Invincibility(float seconds) {
        // Play hit sound (louder in action mode)
        if (gameManager.GetGameMode() == 0)
            GameSystem.PlaySoundEffect(hitSound, hitSource, 0, volume: 0.5f);
        else
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
        // Don't flash the ship if the shield still exists!
        if (gameManager.GetActiveShield() == null)
            StartCoroutine(GameSystem.FlickerSprite(GetComponent<SpriteRenderer>(), seconds));
            
        yield return new WaitForSeconds(seconds);

        // Set iframes to false
        iframes = false;
    }

    // Play death sequence
    public void PlayDeathSequence() {
        gameManager.UpdateBp(-4);

        // Switch to death animation
        playerAnimator.Play("PlayerDeath");

        // Start flashing!
        Invincibility(10); // Long enough to go until end of animation

        // Put a bunch of random explosions around player and shoot out shrapnel
        StartCoroutine(GameSystem.StartExploding(playerSprite, (GameObject) Resources.Load("Prefabs/Explosions/SmallExplosion")));

        // Stop music
        musicManager.StopAllMusic();

        // If in turn-based mode, get rid of menu
        uiManager.SetTurnAlpha(0);
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

            // Start game over cutscene
            cutsceneManager.GameOverCutscene();
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

    public bool IsDead() {
        return dead;
    }

    public void SetHasControl(bool hasControl) {
        this.hasControl = hasControl;
    }

    public bool HasControl() {
        return hasControl;
    }
}
