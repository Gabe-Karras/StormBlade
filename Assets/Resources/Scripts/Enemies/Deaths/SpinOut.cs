using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spin out of control, exploding
public class SpinOut : EnemyDeath
{
    [SerializeField]
    private float deathTime;
    [SerializeField]
    private float randomRange; // Applies to spin speed and throw speed, % of speed to consider randomizing

    [SerializeField]
    private int spinDirection; // 1 = clockwise, -1 = counterclockwise, 0 = random
    [SerializeField]
    private float spinSpeed;

    [SerializeField]
    private float throwAngle; // -1 = random
    [SerializeField]
    private float throwSpeed;

    private bool startFlickering = true;

    private SpriteRenderer sRenderer;

    void Start() {
        System.Random rand = new System.Random();

        throwSpeed /= GameSystem.SPEED_DIVISOR;
        spinSpeed /= GameSystem.ROTATION_DIVISOR;

        // Get random direction
        if (spinDirection != 1 && spinDirection != -1)
            spinDirection = GameSystem.RandomSign();

        // Get random angle
        if (throwAngle < 0) 
            throwAngle = rand.Next(90, 271);

        // Apply random factors to speeds
        float spinRange = rand.Next(0, 101) / 100f * randomRange * spinSpeed * GameSystem.RandomSign();
        float throwRange = rand.Next(0, 101) / 100f * randomRange * throwSpeed * GameSystem.RandomSign();

        spinSpeed += spinRange;
        throwSpeed += throwRange;

        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Called every frame until destruction
    public override void ExecuteDeath() {
        if (startFlickering) {
            // Make sure to flicker long enough
            StartCoroutine(GameSystem.FlickerSprite(sRenderer, deathTime + 1));
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, deathTime));
            StartCoroutine(GameSystem.StartExploding(sRenderer, (GameObject) Resources.Load("Prefabs/Explosions/SmallExplosion")));
            startFlickering = false;
        }

        // Spin sprite
        float realSpin = (transform.rotation.eulerAngles.z + spinSpeed) % 360;
        transform.rotation = Quaternion.Euler(0, 0, realSpin);

        // Move in thrown direction
        transform.position += GameSystem.MoveAtAngle(throwAngle, throwSpeed);
    }
}
