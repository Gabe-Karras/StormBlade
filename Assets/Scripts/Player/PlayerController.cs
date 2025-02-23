using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player speed
    [SerializeField]
    private float speed = 4;

    // Player projectiles
    [SerializeField]
    private GameObject laser0;

    // X coordinates for animation
    private float x;
    private float previousX;

    // Animation elements
    private Animator playerAnimator;
    private string currentAnimation;

    // Constants for turret distance between animations
    private const float TURRET_Y = (float) 0.03;
    private const float TURRET_X = (float) 0.1;
    private const float CLOSE_TURRET_X = (float) 0.07;
    private const float FAR_TURRET_X = (float) 0.09;

    // Start is called before the first frame update
    void Start()
    {
        // Divide for vector math
        speed = speed / 1000;

        x = transform.position.x;
        previousX = x;

        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

        // Update x coordinates for animate function
        x = transform.position.x;
        AnimatePlayer();
        previousX = x;

        // Get current animation
        currentAnimation = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        shoot();
    }

    // Move player with arrow keys
    public void MovePlayer() {
        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += new Vector3(speed, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position += new Vector3(-1 * speed, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += new Vector3(0, speed, 0);
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position += new Vector3(0, -1 * speed, 0);
    }

    // Tilt the ship if it's going in a certain direction
    public void AnimatePlayer() {
        if (x < previousX) {
            playerAnimator.Play("PlayerLeft");
        } else if (x > previousX) {
            playerAnimator.Play("PlayerRight");
        } else {
            playerAnimator.Play("PlayerIdle");
        }
    }

    // Shoot lasers with spacebar
    public void shoot() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Check animation to spawn lasers at correct positions
            if (currentAnimation.Equals("PlayerIdle")) {
                Instantiate(laser0, transform.position + new Vector3(-1 * TURRET_X, TURRET_Y), transform.rotation);
                Instantiate(laser0, transform.position + new Vector3(TURRET_X, TURRET_Y), transform.rotation);
            } else if (currentAnimation.Equals("PlayerLeft")) {
                Instantiate(laser0, transform.position + new Vector3(-1 * CLOSE_TURRET_X, TURRET_Y), transform.rotation);
                Instantiate(laser0, transform.position + new Vector3(FAR_TURRET_X, TURRET_Y), transform.rotation);
            } else if (currentAnimation.Equals("PlayerRight")) {
                Instantiate(laser0, transform.position + new Vector3(-1 * FAR_TURRET_X, TURRET_Y), transform.rotation);
                Instantiate(laser0, transform.position + new Vector3(CLOSE_TURRET_X, TURRET_Y), transform.rotation);
            }
        }
    }
}
