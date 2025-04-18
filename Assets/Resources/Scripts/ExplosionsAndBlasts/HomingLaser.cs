using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Projectile that uses momentum physics to seek a target
public class HomingLaser : Laser
{
    private GameObject target;

    private Vector3 currentMovement;
    private Vector3 previousMovement;

    private bool readyToSeek = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // 60 fps momentum
        speed *= GameSystem.FRAME_RATE / 60;

        currentMovement = new Vector3(0, 0, 0);
        StartCoroutine(WaitToSeek());
        StartCoroutine(Momentum60Fps());
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (readyToSeek && target == null) {
            FindTarget();

            // If out of bounds without a target, destroy
            if (target == null && GameSystem.OutOfBounds(gameObject))
                Destroy(gameObject);
        }
    }

    // Find enemy target
    private void FindTarget() {
        // Get list of currently existing enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float distance = float.MaxValue;

        // Find the closest one
        foreach (GameObject enemy in enemies) {
            float temp = GameSystem.PointDistance(transform.position, enemy.transform.position);
            if (temp < distance) {
                distance = temp;
                target = enemy;
            }
        } 
    }

    // Wait for a second before seeking targets
    private IEnumerator WaitToSeek() {
        yield return new WaitForSeconds(0.3f);

        readyToSeek = true;
    }

    // Override against parent
    protected override void OnBecameInvisible() {
        // For animations in turn-based mode
        if (gameManager.GetComponent<GameManager>().GetGameMode() == 1 && target != null) {
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, 1));
        }
    }

    // Manually set a target for animation purposes
    public void SetHomingTarget(GameObject obj) {
        target = obj;
    }

    private IEnumerator Momentum60Fps() {
        while (true) {
            if (!GameSystem.IsPaused()) {
                // Update previous movement
                previousMovement = currentMovement;

                // Move towards/face target
                if (target != null) {
                    // Set target to null if enemy is dead in action mode
                    if (gameManager.GetComponent<GameManager>().GetGameMode() == 0 && target.GetComponent<HomingBomb>() == null && target.GetComponent<Enemy>().IsDead())
                        target = null;
                    else {
                        currentMovement = GameSystem.MoveTowardsPointWithMomentum(transform.position, target.transform.position, speed, previousMovement);
                        transform.rotation = Quaternion.Euler(0, 0, GameSystem.FacePoint(transform.position, target.transform.position));
                    }
                }
                // If no target, move up and look for new target
                else {
                    currentMovement = transform.up * speed + previousMovement;
                }

                // Commit to movement
                transform.position += currentMovement;
            }

            yield return new WaitForSecondsRealtime(0.015f);
        }
    }
}
