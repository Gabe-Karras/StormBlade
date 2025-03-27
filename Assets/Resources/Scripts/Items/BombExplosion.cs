using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unique behaviors exclusive to the bomb explosion
public class BombExplosion : MonoBehaviour
{
    [SerializeField]
    private GameObject smallerExplosion;

    private List<GameObject> overlappingEnemies;

    // Used for turn-based animations
    private GameManager gameManager;
    private float animationLength;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameSystem.StartExploding(GetComponent<SpriteRenderer>(), smallerExplosion));
        overlappingEnemies = new List<GameObject>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        animationLength = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Damage enemies over and over as long as they are colliding
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Enemy")) {
            overlappingEnemies.Add(other.gameObject);
            StartCoroutine(HurtEnemy(other.gameObject));
        }

        // Flash sprite of boss components
        if (gameManager.GetGameMode() == 1) {
            List<BossComponent> bossComponents = gameManager.GetBoss().GetActiveComponents();

            if (bossComponents.Contains(other.GetComponent<BossComponent>())) {
                StartCoroutine(other.GetComponent<BossComponent>().FlashWhite(time: animationLength - 0.1f));
            }
        }
    }

    // If enemy escapes explosion, turn off damage
    private void OnTriggerExit2D(Collider2D other) {
        if (overlappingEnemies.Contains(other.gameObject))
            overlappingEnemies.Remove(other.gameObject);
    }

    // Will constantly hurt a given enemy until told to stop
    private IEnumerator HurtEnemy(GameObject enemy) {
        while (enemy != null && overlappingEnemies.Contains(enemy)) {
            // If enemy is about to die from explosion, set death animation to scorch
            if (enemy.GetComponent<Enemy>().GetHp() <= 5) {
                enemy.AddComponent<Scorch>();
                enemy.GetComponent<Enemy>().SetDeathBehavior(enemy.GetComponent<Scorch>());
            }

            // Deal damage
            enemy.GetComponent<Enemy>().UpdateHp(-5);

            // Wait
            yield return new WaitForSeconds(0.3f);
        }
    }
}
