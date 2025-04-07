using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Zigzag back and forth down the screen like in space invaders!
public class SpaceInvader : EnemyBehavior
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float downDistance;

    [SerializeField]
    private EnemyAttack attack;

    [SerializeField]
    private bool faceDown;

    [SerializeField]
    private float attackTime;

    private bool goingDown = true;
    private float distance = 0;

    private bool attacking = false;
    private int direction;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;
        downDistance /= GameSystem.PIXELS_PER_UNIT;

        if (faceDown)
            transform.rotation = Quaternion.Euler(0, 0, 180);

        // Move the farthest direction on start
        if (transform.position.x >= 0)
            direction = -1;
        else
            direction = 1;
    }

    // Update is called once per frame
    public override void ExecuteBehavior() {
        // Destroy if it goes below the screen
        if (GameSystem.OutOfBounds(gameObject) && transform.position.y < GameSystem.Y_ACTION_BOUNDARY * -1)
            Destroy(gameObject);

        // Two phases: going down or going across
        if (goingDown) {
            transform.position -= new Vector3(0, speed, 0);
            distance += speed;
            if (distance >= downDistance) {
                goingDown = false;
                distance = 0;
            }
        } else {
            // Check for boundary collision
            SpriteRenderer s = GetComponent<SpriteRenderer>();
            if (direction == -1) {
                if (transform.position.x - s.bounds.extents.x - speed < -1 * GameSystem.X_ACTION_BOUNDARY) {
                    transform.position = new Vector3(-1 * GameSystem.X_ACTION_BOUNDARY + s.bounds.extents.x, transform.position.y, transform.position.z);
                    direction = 1;
                    goingDown = true;
                }
            } else {
                if (transform.position.x + s.bounds.extents.x + speed > GameSystem.X_ACTION_BOUNDARY) {
                    transform.position = new Vector3(GameSystem.X_ACTION_BOUNDARY - s.bounds.extents.x, transform.position.y, transform.position.z);
                    direction = -1;
                    goingDown = true;
                }
            }

            if (!goingDown) {
                transform.position += new Vector3(speed * direction, 0, 0);
            }
        }

        // Attack
        if (!attacking)
            StartCoroutine(AttackWait());
    }

    // Attack at set intervals
    private IEnumerator AttackWait() {
        attacking = true;
        yield return new WaitForSeconds(attackTime);

        if (attack != null)
            attack.ExecuteAttack();

        attacking = false;
    }
}
