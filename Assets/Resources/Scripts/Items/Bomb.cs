using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot spinning bomb forwards, come to a halt, execute light effect, spawn explosion
public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float travelDistance;
    [SerializeField]
    private float travelSpeed;
    [SerializeField]
    private float spinSpeed;
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private GameObject bombLight;

    // Position where the bomb was thrown from
    private float startPos;
    private float currentRotation;

    private bool finalRotation = false;
    private bool effectFinished = false;

    // This is the x/y distance to spawn the "bomblight" object
    private const float LIGHT_DISTANCE = 21f / GameSystem.PIXELS_PER_UNIT;

    // player reference
    private PlayerMoves playerMoves;

    // Start is called before the first frame update
    void Start()
    {
        travelSpeed /= GameSystem.SPEED_DIVISOR;
        travelDistance /= GameSystem.PIXELS_PER_UNIT;
        spinSpeed /= GameSystem.ROTATION_DIVISOR;

        startPos = transform.position.y;

        playerMoves = GameObject.Find("Player").GetComponent<PlayerMoves>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSystem.IsPaused()) {
            // Move forwards until travel distance is achieved
            if (transform.position.y - startPos < travelDistance) {
                transform.position += new Vector3(0, travelSpeed, 0);
                rotate();
            } else {
                finalRotation = true;

                // Rotate until at correct alignment
                if (transform.rotation.z != 0)
                    rotate();
                
                // When light effect is finished, destroy and spawn explosion
                if (effectFinished) {
                    GameObject temp = Instantiate(explosion, transform.position, transform.rotation);

                    // Send explosion to moves class for animation
                    playerMoves.SetBombExplosion(temp);
                    
                    Destroy(gameObject);
                }
            }
        }
    }

    // Rotate the bomb sprite and snap to 360 on final rotation
    private void rotate() {
        // Spin sprite with rotation speed
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        currentRotation = (currentRotation + spinSpeed) % 360;

        // If on final rotation and about to hit zero, snap to it
        if (finalRotation && currentRotation >= 360 - spinSpeed)
            currentRotation = 0;

        if (currentRotation == 0 && finalRotation)
            StartCoroutine(lightEffect());
    }

    // Spawn four bomblight elements around this sprite
    private IEnumerator lightEffect() {
        // Used to get length of animation
        GameObject temp = Instantiate(bombLight, transform.position + new Vector3(LIGHT_DISTANCE, LIGHT_DISTANCE, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(bombLight, transform.position + new Vector3(LIGHT_DISTANCE, LIGHT_DISTANCE * -1, 0), Quaternion.Euler(0, 0, 270));
        Instantiate(bombLight, transform.position + new Vector3(LIGHT_DISTANCE * -1, LIGHT_DISTANCE * -1, 0), Quaternion.Euler(0, 0, 180));
        Instantiate(bombLight, transform.position + new Vector3(LIGHT_DISTANCE * -1, LIGHT_DISTANCE, 0), Quaternion.Euler(0, 0, 90));

        // Wait until animations are finished
        yield return new WaitForSeconds(temp.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        effectFinished = true;
    }

    // Set travel distance for animation
    public void SetTravelDistance(float distance) {
        travelDistance = distance;
    }
}
