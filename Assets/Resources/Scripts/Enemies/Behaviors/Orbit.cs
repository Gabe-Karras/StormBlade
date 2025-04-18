using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This behavior will make the enemy orbit the player
public class Orbit : EnemyBehavior
{
    // How fast it moves in to begin orbiting
    [SerializeField]
    private float chaseSpeed;

    // How fast the enemy actually orbits (angle speed)
    [SerializeField]
    private float orbitSpeed;

    // Distance from player
    [SerializeField]
    private float radius;

    // Whether or not to crash into the player
    [SerializeField]
    private bool divebomb;

    // Speed to move up and down radius
    [SerializeField]
    private float radiusSpeed;

    // The explosion a successful divebomb will result in
    [SerializeField]
    private GameObject explosion;

    // From which direction to begin orbiting
    private float entryAngle;
    private bool orbiting = false;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        chaseSpeed /= GameSystem.SPEED_DIVISOR;
        radiusSpeed /= GameSystem.SPEED_DIVISOR;
        radius /= GameSystem.PIXELS_PER_UNIT;
        orbitSpeed /= GameSystem.ROTATION_DIVISOR;

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    public override void ExecuteBehavior()
    {
        // Chase mode: Chase player until meeting distance of radius
        if (!orbiting) {
            // Angle from player to enemy
            float angle = GameSystem.FacePoint(player.transform.position, transform.position);

            // Target point is radius distance from player along angle
            float xTarget = player.transform.position.x + (float) (radius * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
            float yTarget = player.transform.position.y + (float) (radius * Math.Cos(angle * (Math.PI / 180)));
            xTarget *= -1; // <-- Idk why I have to do this but it works
            Vector3 target = new Vector3(xTarget, yTarget, 0);

            // Move towards target
            transform.position += GameSystem.MoveTowardsPoint(transform.position, target, chaseSpeed);

            // If reached point, switch to orbit mode
            if (transform.position.Equals(target)) {
                orbiting = true;
                entryAngle = angle - (angle * 2);
            }
        } 
        // Orbit mode: orbit around player at radius
        else {
            // Increase angle at orbit speed
            entryAngle = (entryAngle + orbitSpeed) % 360;

            // Target point is radius distance from player along updated angle
            float xTarget = player.transform.position.x + (float) (radius * Math.Sin(entryAngle * (Math.PI / 180))); // Convert to radians
            float yTarget = player.transform.position.y + (float) (radius * Math.Cos(entryAngle * (Math.PI / 180)));
            Vector3 target = new Vector3(xTarget, yTarget, 0);

            // Snap to new location
            transform.position = target;

            // If divebomb is selected, reduce radius by given speed
            if (divebomb)
                radius -= radiusSpeed;
        }
    }

    // If hits player, deal damage and explode
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.Equals(player)) {
            Instantiate(explosion, transform.position, transform.rotation);

            // Give enemy script time to deal damage
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, 0.05f));
        }
    }
}
