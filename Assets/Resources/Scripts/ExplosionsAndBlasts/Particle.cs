using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to a particle for randomized effects
public class Particle : MonoBehaviour
{
    // How fast the particle will shoot out
    [SerializeField]
    private float speed;
    // How long the particle exists before despawning (seconds)
    [SerializeField]
    private float existenceTime;

    // How much deviance allowed for the previous two fields
    private float speedVariance = 0.3f;
    private float timeVariance = 0.3f;

    // Random tragectory and size
    private float angle;
    private float scale;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;

        // Randomize speed and time
        System.Random r = new System.Random();
        speedVariance *= speed * r.Next(0, 101) / 100.0f * GameSystem.RandomSign();
        timeVariance *= existenceTime * r.Next(0, 101) / 100.0f * GameSystem.RandomSign();
        speed += speedVariance;
        existenceTime += timeVariance;

        // Get random angle and scale
        angle = r.Next(0, 360);
        scale = GameSystem.RandomSign();
        if (scale < 0) {
            scale = 2;
        }

        // Set scale
        transform.localScale = new Vector3(scale, scale, 1);

        // Set up destruction
        StartCoroutine(GameSystem.DelayedDestroy(gameObject, existenceTime));
    }

    // Update is called once per frame
    void Update()
    {
        // Move in decided angle
        transform.position += GameSystem.MoveAtAngle(angle, speed);
    }
}
