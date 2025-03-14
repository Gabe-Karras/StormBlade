using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class executes the behavior for item drops. Should be attached to items
public class Pickup : MonoBehaviour
{
    // Set this to the name of the item this will modify!
    // smallHealth, bigHealth, bomb, lightning, missile, shield, blaster
    [SerializeField]
    private string item;

    private float horizontalSpeed = 0;
    private float maxHorizontalSpeed = 0.3f;
    private float verticalSpeed = 0.3f;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        verticalSpeed /= GameSystem.SPEED_DIVISOR;
        maxHorizontalSpeed /= GameSystem.SPEED_DIVISOR;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move down
        transform.position += new Vector3(0, -1 * verticalSpeed, 0);

        // Loiter back and forth
        CalculateHorizontalSpeed();
        transform.position += new Vector3(horizontalSpeed, 0, 0);

        // Destroy object if it gets below the screen
        if (transform.position.y <= GameSystem.Y_ACTION_BOUNDARY * -1 - spriteRenderer.bounds.extents.y) {
            Destroy(gameObject);
        }
    }

    private void CalculateHorizontalSpeed() {
        System.Random r = new System.Random();

        // Get speed change
        float speedChange = r.Next(1, 101) / 1000.0f; // from 0.01 to 0.1
        speedChange *= maxHorizontalSpeed;
        speedChange *= GameSystem.RandomSign();

        // If the item is going off screen, bring it back
        if (transform.position.x <= GameSystem.X_ACTION_BOUNDARY * -1 + spriteRenderer.bounds.extents.x)
            speedChange = Math.Abs(speedChange);
        else if (transform.position.x >= GameSystem.X_ACTION_BOUNDARY - spriteRenderer.bounds.extents.x)
            speedChange = Math.Abs(speedChange) * -1;

        horizontalSpeed += speedChange;

        // Adjust for bounds
        if (horizontalSpeed > maxHorizontalSpeed)
            horizontalSpeed = maxHorizontalSpeed;
        else if (horizontalSpeed < maxHorizontalSpeed * -1)
            horizontalSpeed = maxHorizontalSpeed * -1;
    }

    public string GetItem() {
        return item;
    }
}
