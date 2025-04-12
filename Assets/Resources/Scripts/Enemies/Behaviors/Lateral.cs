using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Loiter back and horizontally at random
public class Lateral : EnemyBehavior
{
    // How fast the enemy moves back and forth
    [SerializeField]
    private float speed;

    // How long (on average) the enemy will take before thinking about changing directions
    [SerializeField]
    private float decisionTime;

    [SerializeField]
    private float attackTime;
    private bool attacking;

    [SerializeField]
    private EnemyAttack attack;

    private bool deciding = false;
    private int direction;

    private SpriteRenderer sRenderer;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;
        direction = GameSystem.RandomSign();

        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public override void ExecuteBehavior()
    {
        // Choose random direction to move in
        if (!deciding)
            StartCoroutine(ChooseDirection());

        // If out of bounds, steer back towards camera
        if (transform.position.x + sRenderer.bounds.extents.x >= GameSystem.X_ACTION_BOUNDARY)
            direction = -1;
        else if (transform.position.x - sRenderer.bounds.extents.x <= -1 * GameSystem.X_ACTION_BOUNDARY)
            direction = 1;

        // Move in direction
        transform.position += new Vector3(speed * direction, 0, 0);

        // Attack if equipped with one
        if (attack != null && !attacking)
            StartCoroutine(AttackWait());
    }

    // Choose a random direction, wait a random time
    private IEnumerator ChooseDirection() {
        deciding = true;
        direction = GameSystem.RandomSign();

        float waitTime = decisionTime + (decisionTime / 3) * GameSystem.RandomPercentage() * GameSystem.RandomSign();
        yield return new WaitForSeconds(waitTime);

        deciding = false;
    }

    // Execute attack, then wait for attack time
    private IEnumerator AttackWait() {
        attacking = true;
        attack.ExecuteAttack();

        yield return new WaitForSeconds(attackTime);
        attacking = false;
    }
}
