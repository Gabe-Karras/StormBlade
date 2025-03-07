using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Enemy moves to random point, pauses and attacks over and over.
public class StopAndShoot : EnemyBehavior
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int minRadius;
    [SerializeField]
    private int maxRadius;
    
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float shootTime; // Between 1 and 0, at what fraction of wait time to shoot
    [SerializeField]
    private EnemyAttack attack;

    [SerializeField]
    private bool chasePlayer;
    [SerializeField]
    private float yBoundary;

    private Vector3 colliderBounds;
    private GameObject player;

    // width/height of collider (halved for measuring from center)
    private float colliderX;
    private float colliderY;

    // Variables for behavior method
    private bool choosingPoint = true;
    private bool pauseBehavior = false;
    private Vector3 destination;

    void Start() {
        speed /= GameSystem.SPEED_DIVISOR;
        yBoundary /= GameSystem.PIXELS_PER_UNIT;
        colliderBounds = gameObject.GetComponent<Collider2D>().bounds.size;

        colliderX = colliderBounds.x / 2;
        colliderY = colliderBounds.y / 2;

        player = GameObject.Find("Player");
    }

    // Called every frame
    public override void ExecuteBehavior() {
        if (!pauseBehavior) {
            // Calculate position to move to
            if (choosingPoint) {
                destination = calculateDestination();
                choosingPoint = false;
            }

            // Move to that position
            transform.position += GameSystem.MoveTowardsPoint(transform.position, destination, speed);

            // Stop and shoot
            if (GameSystem.PointDistance(transform.position, destination) == 0) {
                StartCoroutine(WaitAndShoot());
                pauseBehavior = true;
            }
        }
    }

    
    // Calculate (x, y) position to move to
    private Vector3 calculateDestination() {
        System.Random rand = new System.Random();
        float pointX;
        float pointY;
        bool inBounds;

        do {
            // Generate random angle to go in
            // If moving towards player, should be 'y angled' towards them
            float angle;
            if (chasePlayer) {
                if (player.transform.position.y < transform.position.y) {
                    angle = rand.Next(90, 271); // Angle down
                } else {
                    angle = rand.Next(0, 90) + (271 * rand.Next(0, 2)); // Angle up
                }
            } else {
                angle = rand.Next(0, 360);
            }

            // Get random distance to travel
            float distance = rand.Next(minRadius, maxRadius + 1) / GameSystem.PIXELS_PER_UNIT;

            // Check if point is within bounds
            inBounds = false;
            Vector3 point = GameSystem.MoveAtAngle(angle, distance);

            pointX = point.x;
            pointY = point.y;

            pointX += transform.position.x;
            pointY += transform.position.y;

            if (pointX > GameSystem.X_ACTION_BOUNDARY * -1 + colliderX && pointX < GameSystem.X_ACTION_BOUNDARY - colliderX) {
                if ((!chasePlayer && pointY > GameSystem.Y_ACTION_BOUNDARY * -1 + colliderY && pointY < GameSystem.Y_ACTION_BOUNDARY - colliderY) || chasePlayer) {
                    if (yBoundary != 0 && pointY > GameSystem.Y_ACTION_BOUNDARY * -1 + yBoundary) {
                        inBounds = true;
                    } else if (yBoundary == 0) {
                        inBounds = true;
                    }
                }
            }
        // If not, restart process
        } while (!inBounds);

        // Return point to travel to
        Vector3 result = new Vector3(pointX, pointY, 0);
        return result;
    }

    // Wait and execute projectile code
    IEnumerator WaitAndShoot() {
        // Wait until time to shoot
        yield return new WaitForSeconds(waitTime * shootTime);

        // Attack (if there is one)
        if (attack != null) {
            attack.ExecuteAttack();
        }

        // Wait remainder of time
        yield return new WaitForSeconds(waitTime * (1 - shootTime));

        choosingPoint = true;
        pauseBehavior = false;
    }
}
