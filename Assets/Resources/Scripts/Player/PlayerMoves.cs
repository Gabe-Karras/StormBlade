using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class contains all of the player's moves/animations in turn-based mode
public class PlayerMoves : MonoBehaviour
{
    // Movement speed in animations
    [SerializeField]
    private float moveSpeed;

    // How long to wait after player finishes move before letting boss take over
    [SerializeField]
    private float afterMoveTime = 2;

    // Source to play zaps from
    [SerializeField]
    private AudioSource laserSource;
    // The zap sound in question:
    [SerializeField]
    private AudioClip laserSound;

    [SerializeField]
    private AudioSource healSource;

    // Projectile/item prefabs
    [SerializeField]
    private GameObject laser0;
    [SerializeField]
    private GameObject laser1;
    [SerializeField]
    private GameObject laser2;
    [SerializeField]
    private GameObject bomb;
    [SerializeField]
    private GameObject lightning;
    [SerializeField]
    private GameObject missile;
    [SerializeField]
    private GameObject shield;

    // Damage values for items
    private int level1Damage = 30;
    private int level2Damage = 60;
    private int level3Damage = 90;
    private int level4Damage = 120;
    private int level5Damage = 180;
    private int bombDamage = 100;
    private int lightningDamage = 130;
    private int missileDamage = 70;
    private int smallHeal = 100;
    private int bigHeal = 200;

    // The target x position to move the player to
    private float xTarget;

    // Whether or not to move the player towards the target x position
    private bool move = false;

    // How long to wait between shooting lasers
    private float shootSpeed = 0.2f;

    // Is true when the ship is shooting a basic attack. Used to tell the laser ring when to fire in the animation
    private bool attacking = false;
    private GameObject currentTarget;

    // Keep track of occuring bomb explosion in animation
    private GameObject bombExplosion;

    // Game manager reference
    private GameManager gameManager;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed /= GameSystem.SPEED_DIVISOR;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerController = gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move x position
        if (move) {
            Vector3 destination = new Vector3(xTarget, GameSystem.TURN_BASED_Y_POSITION, 0);

            if (GameSystem.PointDistance(transform.position, destination) == 0) {
                move = false;
                return;
            }

            transform.position += GameSystem.MoveTowardsPoint(transform.position, destination, moveSpeed);
        }
    }

    // Basic attack concerning the player's blaster level
    public IEnumerator Attack(GameObject target) {
        currentTarget = target;
        int bp = gameManager.GetBp();
        int damage = 0;

        // How much damage to deal based on blaster level
        switch (bp) {
            case 1:
                damage = level1Damage;
                break;
            case 2:
                damage = level2Damage;
                break;
            case 3:
                damage = level3Damage;
                break;
            case 4:
                damage = level4Damage;
                break;
            case 5:
                damage = level5Damage;
                break;
        }

        // Move player under target
        xTarget = target.transform.position.x;
        move = true;

        // Wait for player to move
        while (move) {
            yield return new WaitForSeconds(0.1f);
        }

        // When player has successfully moved, pause for a moment
        yield return new WaitForSeconds(0.5f);

        // Then unleash attack!!
        // Decide what lasers to shoot based on blaster level
        attacking = true;
        
        if (gameManager.GetBp() > 1) {
            // Shoot red lasers
            for (int i = 0; i < 3; i ++) {
                SpawnLasers(laser1, target);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0, pitch: 1);
                yield return new WaitForSeconds(shootSpeed);
            }
        }

        if (gameManager.GetBp() > 2) {
            // Shoot wave lasers
            for (int i = 0; i < 3; i ++) {
                SpawnLasers(laser2, target);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0, pitch: 0.5f);
                yield return new WaitForSeconds(shootSpeed);
            }
        }

        if (gameManager.GetBp() == 1 || gameManager.GetBp() > 3) {
            // Shoot yellow lasers
            for (int i = 0; i < 3; i ++) {
                SpawnLasers(laser0, target);
                GameSystem.PlaySoundEffect(laserSound, laserSource, 0, pitch: 0.75f);
                yield return new WaitForSeconds(shootSpeed);
            }
        }

        attacking = false;

        // Move back to normal position
        xTarget = 0;
        move = true;

        // Wait for ship to get there
        while (move) {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.3f);

        // Deal damage to component
        damage = RandomizeDamage(damage);
        target.GetComponent<BossComponent>().UpdateHp(damage * -1);

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Attack boss with a bomb
    public IEnumerator Bomb(GameObject target, List<GameObject> otherTargets) {
        // Move player under target
        xTarget = target.transform.position.x;
        move = true;

        // Wait for player to move
        while (move) {
            yield return new WaitForSeconds(0.1f);
        }

        // When player has successfully moved, pause for a moment
        yield return new WaitForSeconds(0.5f);

        // Then unleash attack!!
        GameObject temp = Instantiate(bomb, transform.position, Quaternion.Euler(0, 0, 0));
        // Multiplied by pixels per unit because of bomb Start method
        temp.GetComponent<Bomb>().SetTravelDistance(GameSystem.PointDistance(transform.position, target.transform.position) * GameSystem.PIXELS_PER_UNIT);

        // Wait for a bomb explosion to start
        while (bombExplosion == null) {
            yield return new WaitForSeconds(0.1f);
        }

        // Move back to center
        yield return new WaitForSeconds(2);
        
        xTarget = 0;
        move = true;

        // Wait for ship to get there
        while (move) {
            yield return new WaitForSeconds(0.1f);
        }

        // Wait for explosion to end
        while (bombExplosion != null) {
            yield return new WaitForSeconds(0.1f);
        }

        // Deal damage to all components involved
        target.GetComponent<BossComponent>().UpdateHp(RandomizeDamage(bombDamage * -1));

        for (int i = 0; i < otherTargets.Count; i ++) {
            if (!otherTargets[i].Equals(target))
                otherTargets[i].GetComponent<BossComponent>().UpdateHp(RandomizeDamage(bombDamage * -1));
        }

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Attack boss with lightning
    public IEnumerator Lightning(GameObject target, List<GameObject> otherTargets) {
        // Move player under target
        xTarget = target.transform.position.x;
        move = true;

        // Wait for player to move
        while (move) {
            yield return new WaitForSeconds(0.1f);
        }

        // When player has successfully moved, pause for a moment
        yield return new WaitForSeconds(0.5f);

        // Unleash attack
        GameObject temp = Instantiate(lightning, transform.position, Quaternion.Euler(0, 0, 0));
        temp.GetComponent<Laser>().SetComponents(gameManager.GetBoss().GetActiveComponents());
        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Lightning"), healSource, 0, volume: 0.2f);

        // Move back to center
        yield return new WaitForSeconds(1);
        
        xTarget = 0;
        move = true;

        // Wait for ship to get there
        while (move) {
            yield return new WaitForSeconds(0.1f);
        }

        // Deal damage to all components involved
        target.GetComponent<BossComponent>().UpdateHp(RandomizeDamage(lightningDamage * -1));

        for (int i = 0; i < otherTargets.Count; i ++) {
            if (!otherTargets[i].Equals(target))
                otherTargets[i].GetComponent<BossComponent>().UpdateHp(RandomizeDamage(lightningDamage * -1));
        }

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Attack boss with missiles
    public IEnumerator Missile(List<GameObject> targets) {
        yield return new WaitForSeconds(0.5f);

        // Figure out how many missiles to spawn and what angles to spawn them at
        float benchmark = 180f / (targets.Count + 1); // Degrees between each angle
        float current = 0; // The current angle in the pattern

        // Play sound and spawn spreadshot of missiles
        for (int i = 0; i < targets.Count; i ++) {
            // Add benchmark to current
            current += benchmark;

            // Convert into game angle
            float temp;
            if (current <= 90) // Left side
                temp = 90 - current;
            else // Right side
                temp = 450 - current;

            Instantiate(missile, transform.position, Quaternion.Euler(0, 0, temp));
        }

        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Missile"), healSource, 0, volume: 0.2f);

        // Wait a second, then create missiles that fall to the target components
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < targets.Count; i ++) {
            Vector3 position = new Vector3(targets[i].transform.position.x, GameSystem.Y_ACTION_BOUNDARY + 0.1f, 0);
            GameObject temp = Instantiate(missile, position, Quaternion.Euler(0, 0, 180));
            temp.GetComponent<Laser>().SetTarget(targets[i]);
            temp.GetComponent<HomingLaser>().SetHomingTarget(targets[i]);
        }

        // Deal damage to all components
        yield return new WaitForSeconds(1);

        for (int i = 0; i < targets.Count; i ++) {
            targets[i].GetComponent<BossComponent>().UpdateHp(RandomizeDamage(missileDamage * -1));
        }

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Give player shield
    public IEnumerator Shield() {
        yield return new WaitForSeconds(0.5f);

        // Create shield object
        GameObject temp = Instantiate(shield, transform.position, Quaternion.Euler(0, 0, 0));
        // Overwrite current active shield
        if (gameManager.GetActiveShield() != null)
            Destroy(gameManager.GetActiveShield());
        gameManager.SetActiveShield(temp);

        // Play shield noise
        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Shield"), healSource, 0);

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Heal player with small repair
    public IEnumerator SmallHealth() {
        yield return new WaitForSeconds(0.5f);

        // Play heal animation
        playerController.PlayHealAnimation(0.4f);
        yield return new WaitForSeconds(0.8f);

        // Give player health!
        gameManager.UpdateHp(RandomizeDamage(smallHeal));

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Heal player with small repair
    public IEnumerator BigHealth() {
        yield return new WaitForSeconds(0.5f);

        // Play heal animation
        playerController.PlayHealAnimation(0.6f);
        yield return new WaitForSeconds(1.2f);

        // Give player health!
        gameManager.UpdateHp(RandomizeDamage(bigHeal));

        // Wait, then set boss turn to be true
        yield return new WaitForSeconds(afterMoveTime);
        gameManager.SetBossTurn(true);
    }

    // Randomize a damage value based on 10%
    private int RandomizeDamage(int damage) {
        return damage + ((int) (damage / 10 * GameSystem.RandomPercentage() * GameSystem.RandomSign()));
    }

    // Spawn lasers out of turrets
    private void SpawnLasers (GameObject laser, GameObject target) {
        Vector3[] positions = playerController.GetTurretPositions();
        Vector3 leftTurret = positions[0];
        Vector3 rightTurret = positions[1];

        // Spawn lasers
        GameObject firstLaser = Instantiate(laser, leftTurret, transform.rotation);
        GameObject secondLaser = Instantiate(laser, rightTurret, transform.rotation);

        firstLaser.GetComponent<Laser>().SetTarget(target);
        if (laser.GetComponent<WaveLaser>() != null)
            firstLaser.GetComponent<WaveLaser>().SetDirection(-1);
        secondLaser.GetComponent<Laser>().SetTarget(target);
    }

    public bool IsAttacking() {
        return attacking;
    }

    public GameObject GetCurrentTarget() {
        return currentTarget;
    }

    public void SetBombExplosion(GameObject ex) {
        bombExplosion = ex;
    }
}
