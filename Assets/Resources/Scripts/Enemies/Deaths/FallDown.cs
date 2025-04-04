using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Start exploding and fall off the screen
public class FallDown : EnemyDeath
{
    [SerializeField]
    private float fallSpeed = 0.1f;

    [SerializeField]
    private float rotSpeed = 1;
    private float currentRotation = 0;

    // Used for momentum calculations
    private Vector3 previousMovement;
    private Vector3 currentMovement;

    private bool startDying = true;
    private SpriteRenderer sRenderer;

    // Start is called before the first frame update
    void Start()
    {
        fallSpeed /= GameSystem.SPEED_DIVISOR;
        sRenderer = GetComponent<SpriteRenderer>();
        currentRotation = transform.rotation.eulerAngles.z;

        // Slightly randomize variables
        rotSpeed += rotSpeed / 3 * GameSystem.RandomPercentage() * GameSystem.RandomSign();
        rotSpeed *= GameSystem.RandomSign();
    }

    // Happens every frame. Accelerate off the screen, then destroy
    public override void ExecuteDeath() {
        if (startDying) {
            // Start exploding
            StartCoroutine(GameSystem.FlickerSprite(sRenderer, 0));
            StartCoroutine(GameSystem.StartExploding(sRenderer, (GameObject) Resources.Load("Prefabs/Explosions/SmallExplosion")));
            startDying = false;
        }

        // Rotate sprite
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        currentRotation = (currentRotation + rotSpeed) % 360;
        
        // Fall off the screen
        currentMovement = GameSystem.MoveAtAngleWithMomentum(180, fallSpeed, previousMovement);
        previousMovement = currentMovement;
        transform.position += currentMovement;
        
        // Despawn when it disappears
        if (GameSystem.OutOfBounds(gameObject))
            Destroy(gameObject);
    }
}
