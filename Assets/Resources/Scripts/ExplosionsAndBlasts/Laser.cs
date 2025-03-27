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

    protected GameObject gameManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Dividing for vector math
        speed /= GameSystem.SPEED_DIVISOR;

        damage *= -1;

        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (gameManager.GetComponent<GameManager>().GetGameMode() == 0 || !destroysOnHit)
            MoveForward();
        else
            MoveTowardsTarget();
    }

    // Move laser forward at speed
    protected void MoveForward() {
        transform.position += transform.up * speed;
    }

    // Destroy laser when it leaves the camera
    protected virtual void OnBecameInvisible() {
        // Destroy immediately in action mode
        if (gameManager.GetComponent<GameManager>().GetGameMode() == 0)
            Destroy(gameObject);

        // For purposes of running animation, keep alive for a short time in turn-based mode
        else
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, 0.1f));
    }

    // Move towards target in turn-based mode
    protected void MoveTowardsTarget() {
        transform.position += GameSystem.MoveTowardsPoint(transform.position, animationTarget.transform.position, speed);
    }

    // Hurt target and destroy self on collision
    private void OnTriggerEnter2D(Collider2D other) {

        // In action mode, hurt enemies or player
        if (gameManager.GetComponent<GameManager>().GetGameMode() == 0) {
            if (hurtsEnemies && other.gameObject.tag.Equals("Enemy")) {
                if (!other.gameObject.GetComponent<Enemy>().IsDead()) {
                    other.gameObject.GetComponent<Enemy>().UpdateHp(damage);

                    if (destroysOnHit)
                        Destroy(gameObject);
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
                    StartCoroutine(other.gameObject.GetComponent<BossComponent>().FlashWhite());
                    transform.position = new Vector3(100, 100, 0);
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
