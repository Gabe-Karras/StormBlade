using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attempt to fly into the player and explode
public class Kamikaze : EnemyBehavior
{
    // Momentum speed
    [SerializeField]
    private float speed;
    private Vector3 currentMovement;
    private Vector3 previousMovement;

    // Downward speed
    [SerializeField]
    private float downSpeed;

    // Explosion if enemy hits player
    [SerializeField]
    private GameObject explosion;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;
        downSpeed /= GameSystem.SPEED_DIVISOR;
        currentMovement = new Vector3(0, 0, 0);

        player = GameObject.Find("Player");
    }

    public override void ExecuteBehavior() {
        previousMovement = currentMovement;

        // Move to be aligned with player
        Vector3 target = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        if (transform.position != target)
            currentMovement = GameSystem.MoveTowardsPointWithMomentum(transform.position, target, speed, previousMovement);

        transform.position += currentMovement;

        // Move down
        transform.position -= new Vector3(0, downSpeed, 0);
    }

    // If collision with player, destroy and explode!
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.Equals(player)) {
            Instantiate(explosion, transform.position, transform.rotation);

            // Slight delay to let other enemy collision code occur
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, 0.05f));
        }
    }
}
