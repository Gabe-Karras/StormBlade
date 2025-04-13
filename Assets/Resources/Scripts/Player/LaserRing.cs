using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRing : MonoBehaviour
{
    // Projectiles
    [SerializeField]
    private GameObject laser;
    
    // Object to circle around
    private GameObject target;

    // Distance from center to barrels
    private const float BIG_RING_DISTANCE = (float) 0.23;
    private const float SMALL_RING_DISTANCE = (float) 0.12;

    // Timing to fire
    private string fireFrame;
    private bool shotThisFrame = false;

    // Sound effect
    private AudioClip laserSound;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");
        transform.position = target.transform.position;
        fireFrame = "LaserRing_0";

        laserSound = Resources.Load<AudioClip>("SoundEffects/Projectiles/BasicLaser");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsPaused()) {
            // Always circle around the target
            transform.position = target.transform.position;

            // In action mode, constantly shoot lasers everywhere!
            if (gameManager.GetGameMode() == 0 && target.GetComponent<PlayerController>().HasControl()) {
                // Shoot when the time is right
                if (GetComponent<SpriteRenderer>().sprite.name.Equals(fireFrame)) {
                    SpawnLasers(fireFrame);

                    // Alternate frame
                    if (fireFrame.Equals("LaserRing_0"))
                        fireFrame = "LaserRing_2";
                    else
                        fireFrame = "LaserRing_0";
                }
            }

            // In turn-based mode, only fire when attack animation is playing
            else if (target.GetComponent<PlayerMoves>().IsAttacking()){
                ShootUp();
            }
        }
    }

    // Method to spawn lasers based on frame
    public void SpawnLasers(string frame) {
        // Invert positions/angles if needed
        float posInversion = 1;
        float angleInversion = 0;
        if (frame.Equals("LaserRing_2")) {
            posInversion = -1;
            angleInversion = 180;
        }

        // Set positions
        float x1 = 0;
        float x2 = BIG_RING_DISTANCE * -1 * posInversion;
        float x3 = BIG_RING_DISTANCE * posInversion;

        float y1 = BIG_RING_DISTANCE * posInversion;
        float y2 = -1 * SMALL_RING_DISTANCE * posInversion;
        float y3 = -1 * SMALL_RING_DISTANCE * posInversion;

        // Create lasers
        Instantiate(laser, transform.position + new Vector3(x1, y1), Quaternion.Euler(0, 0, 0 + angleInversion));
        Instantiate(laser, transform.position + new Vector3(x2, y2), Quaternion.Euler(0, 0, 120 + angleInversion));
        Instantiate(laser, transform.position + new Vector3(x3, y3), Quaternion.Euler(0, 0, 240 + angleInversion));

        // Play sound!
        GameSystem.PlaySoundEffect(laserSound, GetComponent<AudioSource>(), 0);
    }

    // Used in turn-based animation. Shoots a red laser straight up when ring is at frame 0
    private void ShootUp() {
        if (GetComponent<SpriteRenderer>().sprite.name.Equals("LaserRing_0")) {
            if (!shotThisFrame) {
                GameObject upLaser = Instantiate(laser, transform.position + new Vector3(0, BIG_RING_DISTANCE, 0), Quaternion.Euler(0, 0, 0));
                upLaser.GetComponent<Laser>().SetTarget(target.GetComponent<PlayerMoves>().GetCurrentTarget());
                // Play sound!
                GameSystem.PlaySoundEffect(laserSound, GetComponent<AudioSource>(), 0);
                shotThisFrame = true;
            }
        } else {
            shotThisFrame = false;
        }
    }
}
