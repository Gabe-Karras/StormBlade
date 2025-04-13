using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Speed of laser
    [SerializeField]
    protected float speed;

    // Hp damage
    [SerializeField]
    private int damage;

    [SerializeField]
    private bool hurtsPlayer;
    [SerializeField]
    private bool hurtsEnemies;

    // Whether this destroys on impact from an enemy or not
    [SerializeField]
    private bool destroysOnHit;

    // Used in turn-based mode to determine what to hit
    private GameObject animationTarget;
    private List<BossComponent> bossComponents;
    // What game mode was this laser created in?
    private int creationMode;

    protected GameObject gameManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Dividing for vector math
        speed /= GameSystem.SPEED_DIVISOR;

        damage *= -1;

        gameManager = GameObject.Find("GameManager");
        creationMode = gameManager.GetComponent<GameManager>().GetGameMode();

        // If laser is created out of bounds, immediately destroy it
        if (GameSystem.OutOfBounds(gameObject)) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!gameManager.GetComponent<GameManager>().IsPaused()) {
            // Move forward in action mode or to target in turn-based mode
            if (gameManager.GetComponent<GameManager>().GetGameMode() == 0 || !destroysOnHit)
                MoveForward();
            else {
                MoveTowardsTarget();
            }
        }
    }

    // Move laser forward at speed
    protected void MoveForward() {
        transform.position += transform.up * speed;
    }

    // Destroy laser when it leaves the camera
    protected virtual void OnBecameInvisible() {
        if (gameManager != null) {
            // Destroy immediately in action mode
            if (gameManager.GetComponent<GameManager>().GetGameMode() == 0)
                Destroy(gameObject);

            // For purposes of running animation, keep alive for a short time in turn-based mode
            else
                StartCoroutine(GameSystem.DelayedDestroy(gameObject, 0.1f));
        }
    }

    // Move towards target in turn-based mode
    protected void MoveTowardsTarget() {
        if (creationMode == 0)
            Destroy(gameObject);
        else
            transform.position += GameSystem.MoveTowardsPoint(transform.position, animationTarget.transform.position, speed);
    }

    // Hurt target and destroy self on collision
    private void OnTriggerEnter2D(Collider2D other) {

        // In action mode, hurt enemies or player
        if (gameManager.GetComponent<GameManager>().GetGameMode() == 0) {
            if (hurtsEnemies && other.gameObject.tag.Equals("Enemy")) {
                if (other.gameObject.GetComponent<Enemy>() != null) {
                    if (!other.gameObject.GetComponent<Enemy>().IsDead()) {
                        other.gameObject.GetComponent<Enemy>().UpdateHp(damage);

                        if (destroysOnHit)
                            Destroy(gameObject);
                    }
                }// Hit homing bomb
                else if (other.gameObject.GetComponent<HomingBomb>() != null) {
                    other.gameObject.GetComponent<HomingBomb>().SetHit();
                }
            }

            if (hurtsPlayer && other.gameObject.tag.Equals("Player")) {
                gameManager.GetComponent<GameManager>().UpdateHp(damage);
                
                if (destroysOnHit)
                    Destroy(gameObject);
            }
        }

        // In turn-based mode, simply destroy and flash target for animation purposes
        else {
            if (destroysOnHit) {
                if (other.gameObject.Equals(animationTarget)) {
                    // If target is boss component
                    if (other.gameObject.GetComponent<BossComponent>() != null) {
                        StartCoroutine(other.gameObject.GetComponent<BossComponent>().FlashWhite());
                        transform.position = new Vector3(100, 100, 0);
                    }

                    // If target is player
                    else {
                        if (!other.gameObject.GetComponent<PlayerController>().GetIframes())
                            other.gameObject.GetComponent<PlayerController>().Invincibility(1);
                        transform.position = new Vector3(100, 100, 0);
                    }
                }
            }
            // If it doesn't destroy on hit (lightning) allow it to light up any boss component it touches
            else {
                if (bossComponents.Contains(other.gameObject.GetComponent<BossComponent>())) {
                    StartCoroutine(other.gameObject.GetComponent<BossComponent>().FlashWhite(time: 0.1f));
                }
            }
        }
    }

    // Setup methods for when this is instantiated as part of a turn-based animation
    public void SetTarget(GameObject target) {
        animationTarget = target;
    }

    public void SetComponents(List<BossComponent> components) {
        bossComponents = components;
    }
}
