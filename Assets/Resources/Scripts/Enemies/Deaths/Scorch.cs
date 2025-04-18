using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Turn sprite black and drop off the screen with momentum
public class Scorch : EnemyDeath
{
    [SerializeField]
    private float fallSpeed = 0.1f;

    // Delay before falling off screen
    [SerializeField]
    private float waitSeconds = 0.5f;
    private bool readyToFall = false;

    [SerializeField]
    private float rotSpeed = 1;
    private float currentRotation = 0;

    // Used for momentum calculations
    private Vector3 previousMovement;
    private Vector3 currentMovement;

    // Start is called before the first frame update
    void Start()
    {
        fallSpeed /= GameSystem.SPEED_DIVISOR;
        // 60 fps momentum
        fallSpeed *= GameSystem.FRAME_RATE / 60;

        // Slightly randomize variables
        waitSeconds += waitSeconds / 3 * GameSystem.RandomPercentage() * GameSystem.RandomSign();
        rotSpeed += rotSpeed / 3 * GameSystem.RandomPercentage() * GameSystem.RandomSign();
        rotSpeed *= GameSystem.RandomSign();
        rotSpeed /= GameSystem.ROTATION_DIVISOR;

        // Turn sprite black
        GetComponent<SpriteRenderer>().material = Resources.Load<Material>("Materials/SolidBlack");
        previousMovement = new Vector3(0, 0, 0);

        // Hover in place before falling
        StartCoroutine(WaitForFall());
        StartCoroutine(Momentum60Fps());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Delay before falling
    private IEnumerator WaitForFall() {
        yield return new WaitForSeconds(waitSeconds);
        readyToFall = true;
    }

    // Happens every frame. Accelerate off the screen, then destroy
    public override void ExecuteDeath() {
        // Rotate sprite
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        currentRotation = (currentRotation + rotSpeed) % 360;
        
        // Despawn when it disappears
        if (GameSystem.OutOfBounds(gameObject))
            Destroy(gameObject);
    }

    private IEnumerator Momentum60Fps() {
        while (true) {
            if (!GameSystem.IsPaused()) {
                if (readyToFall) {
                    // Fall off the screen
                    currentMovement = GameSystem.MoveAtAngleWithMomentum(180, fallSpeed, previousMovement);
                    previousMovement = currentMovement;
                    transform.position += currentMovement;
                }
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
