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

        // Get random direction
        if (spinDirection != 1 && spinDirection != -1)
            spinDirection = RandomSign(rand);

        // Get random angle
        if (throwAngle < 0) 
            throwAngle = rand.Next(90, 271);

        // Apply random factors to speeds
        float spinRange = rand.Next(0, 101) / 100f * randomRange * spinSpeed * RandomSign(rand);
        float throwRange = rand.Next(0, 101) / 100f * randomRange * throwSpeed * RandomSign(rand);

        spinSpeed += spinRange;
        throwSpeed += throwRange;

        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Called every frame until destruction
    public override void ExecuteDeath() {
        if (startFlickering) {
            // Make sure to flicker long enough
            StartCoroutine(GameSystem.FlickerSprite(GetComponent<SpriteRenderer>(), deathTime + 1));
            StartCoroutine(DelayedDestroy(deathTime));
            StartCoroutine(SpawnExplosions());
            startFlickering = false;
        }

        // Spin sprite
        float realSpin = (transform.rotation.eulerAngles.z + spinSpeed) % 360;
        transform.rotation = Quaternion.Euler(0, 0, realSpin);

        // Move in thrown direction
        transform.position += GameSystem.MoveAtAngle(throwAngle, throwSpeed);
    }

    // Randomly returns -1 or 1
    private int RandomSign(System.Random rand) {
        return -1 + 2 * rand.Next(0, 2);
    }

    IEnumerator DelayedDestroy(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    // Spawn a random explosion somewhere on the enemy
    private void RandomExplosion() {
        System.Random rand = new System.Random();
        float explosionX = ((float) rand.Next((int) (sRenderer.bounds.size.x / 2 * -1 * GameSystem.PIXELS_PER_UNIT),
                            (int) (sRenderer.bounds.size.x / 2 * GameSystem.PIXELS_PER_UNIT + 1))) / GameSystem.PIXELS_PER_UNIT;
        float explosionY = ((float) rand.Next((int) (sRenderer.bounds.size.y / 2 * -1 * GameSystem.PIXELS_PER_UNIT),
                            (int) (sRenderer.bounds.size.y / 2 * GameSystem.PIXELS_PER_UNIT + 1))) / GameSystem.PIXELS_PER_UNIT;

        Instantiate(Resources.Load("Prefabs/Explosions/SmallExplosion"), transform.position + new Vector3(explosionX, explosionY), transform.rotation);
    }

    // Put a bunch of random explosions around enemy
    IEnumerator SpawnExplosions() {
        float explodeTime = 0.2f; // 5th of a second

        while (true) {
            RandomExplosion();
            yield return new WaitForSeconds(explodeTime);
        }
    }
}
