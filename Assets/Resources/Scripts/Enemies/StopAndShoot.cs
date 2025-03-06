using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private GameObject laser;

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
    private float behaviorAngle = 0;
    private float behaviorDistance = 0;

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
                float[] positionInfo = calculateDestination();
                behaviorAngle = positionInfo[0];
                behaviorDistance = positionInfo[1];
                choosingPoint = false;
            }

            // Move to that position
            behaviorDistance -= MoveTowardsPoint(behaviorAngle, behaviorDistance);

            // Stop and shoot
            if (behaviorDistance == 0) {
                StartCoroutine(WaitAndShoot());
                pauseBehavior = true;
            }
        }
    }

    
    // Calculate position to move to. Returns array with angle and distance
    private float[] calculateDestination() {
        System.Random rand = new System.Random();
        float angle;
        float distance;
        bool inBounds;

        do {
            // Generate random angle to go in
            // If moving towards player, should be 'y angled' towards them
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
            distance = rand.Next(minRadius, maxRadius + 1) / GameSystem.PIXELS_PER_UNIT;

            // Check if point is within bounds
            inBounds = false;
            float pointX = (float) (distance * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
            float pointY = (float) (distance * Math.Cos(angle * (Math.PI / 180)));

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
        } while (!inBounds);
        // If not, restart process

        // Return point to travel to
        float[] result = {angle, distance};
        return result;
    }

    // Moves one step towards a point defined by angle and distance.
    // Returns the distance moved.
    private float MoveTowardsPoint(float angle, float distance) {
        float xChange;
        float yChange;
        float distanceMoved;

        // Check distance between points
        if (speed < distance) {
            // If speed is smaller, move in direction
            xChange = (float) (speed * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
            yChange = (float) (speed * Math.Cos(angle * (Math.PI / 180)));

            transform.position += new Vector3(xChange, yChange, 0f);
            distanceMoved = speed;
        } else {
            // If distance is smaller, snap to position
            xChange = (float) (distance * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
            yChange = (float) (distance * Math.Cos(angle * (Math.PI / 180)));

            transform.position += new Vector3(xChange, yChange, 0f);
            distanceMoved = distance;
        }

        return distanceMoved;
    }

    IEnumerator WaitAndShoot() {
        // Wait until time to shoot
        yield return new WaitForSeconds(waitTime * shootTime);

        // Shoot projectile (if there is one)
        if (laser != null) {
            Instantiate(laser, transform.position, transform.rotation);
        }

        // Wait remainder of time
        yield return new WaitForSeconds(waitTime * (1 - shootTime));

        choosingPoint = true;
        pauseBehavior = false;
    }
}
