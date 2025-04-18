using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weave down the screen in a wave, and shoot if a laser is assigned
public class WeaveDown : EnemyBehavior
{
    // How fast to fly straight down
    [SerializeField]
    private float downSpeed;
    // Max lateral speed
    [SerializeField]
    private float horizontalSpeed;
    // Rate of lateral speed change
    [SerializeField]
    private float velocity;
    private float realSpeed;
    
    // Which attack to invoke
    [SerializeField]
    private EnemyAttack attack;
    // How often to attack
    [SerializeField]
    private float attackTime;
    private bool attacking = false;

    // Start is called before the first frame update
    void Start()
    {
        downSpeed /= GameSystem.SPEED_DIVISOR;
        horizontalSpeed /= GameSystem.SPEED_DIVISOR;
        realSpeed = horizontalSpeed;
        velocity = horizontalSpeed / velocity;

        // Face down
        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public override void ExecuteBehavior() {
        // Reverse direction if speed gets too great
        if (realSpeed >= horizontalSpeed) {
            velocity *= -1;
            realSpeed = horizontalSpeed;
        } else if (realSpeed <= horizontalSpeed * -1) {
            velocity *= -1;
            realSpeed = horizontalSpeed * -1;
        }

        // Move in wave
        transform.position += new Vector3(0, -1 * downSpeed, 0);
        transform.position += transform.right * realSpeed;
        realSpeed += GameSystem.CalculateAcceleration(horizontalSpeed, velocity);

        // Attack when the time is right
        if (!attacking && attack != null) {
            StartCoroutine(AttackWait(attackTime));
        }

        // Destroy if below camera
        if (GameSystem.OutOfBounds(gameObject) && transform.position.y < GameSystem.Y_ACTION_BOUNDARY * -1) {
            Destroy(gameObject);
        }
    }

    private IEnumerator AttackWait(float seconds) {
        attacking = true;
        attack.ExecuteAttack();
        yield return new WaitForSeconds(seconds);
        attacking = false;
    }
}
