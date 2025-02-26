using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    // Player speed
    [SerializeField]
    private float speed = 4;

    // Player projectiles
    [SerializeField]
    private GameObject laser0;
    [SerializeField]
    private GameObject laser1;

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

    // Collider to use collision physics
    private Collider2D playerCollider;

    // Game manager
    private GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Divide for vector math
        speed = speed / 50;

        x = transform.position.x;
        previousX = x;

        playerAnimator = GetComponent<Animator>();

        playerCollider = GetComponent<Collider2D>();

        gameManager = GameObject.Find("GameManager");
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

        Shoot();
    }

    // Move player with arrow keys
    public void MovePlayer() {
        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += MoveWithCollision(new Vector3(speed, 0, 0));
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position += MoveWithCollision(new Vector3(-1 * speed, 0, 0));
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += MoveWithCollision(new Vector3(0, speed, 0));
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position += MoveWithCollision(new Vector3(0, -1 * speed, 0));
    }


    // Move with respect to collision
    private Vector3 MoveWithCollision(Vector3 direction) {
        Vector3 result = direction;

        // Check for UI object in the way of current movement
        float playerBoundX = transform.position.x + direction.x + MathF.Sign(direction.x) * playerCollider.bounds.extents.x;
        float playerBoundY = transform.position.y + direction.y + MathF.Sign(direction.y) * playerCollider.bounds.extents.y;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(playerBoundX, playerBoundY), 0.0001f);
        
        foreach (Collider2D c in colliders) {
            // Check if it is a barrier
            if (c.gameObject.CompareTag("Barrier")) {

                // If so, snap to position next to it instead of going into it
                result = new Vector3(0, 0, 0);

                if (direction.x != 0) { // Horizontal
                    int sign = MathF.Sign(direction.x);
                    float wall = c.bounds.center.x - sign * c.bounds.extents.x;     // x value of colliding wall
                    float playerWall = transform.position.x + sign * playerCollider.bounds.extents.x; // x value of hitbox wall

                    // Distance between colliders
                    float distance = Math.Abs(wall - playerWall);
                    result += new Vector3(sign * distance, 0, 0);
                }
                
                if (direction.y != 0) { // Vertical
                    int sign = MathF.Sign(direction.y);
                    float wall = c.bounds.center.y - sign * c.bounds.extents.y;     // y value of colliding wall
                    float playerWall = transform.position.y + sign * playerCollider.bounds.extents.y; // y value of hitbox wall

                    // Distance between colliders
                    float distance = Math.Abs(wall - playerWall);
                    result += new Vector3(0, sign * distance, 0);
                }
            }
        }
        

        return result;
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
    public void Shoot() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Decide how to shoot based on BP
            if (gameManager.GetComponent<GameManager>().bp == 1)
                SpawnLasers(laser0);
            else if (gameManager.GetComponent<GameManager>().bp == 2)
                SpawnLasers(laser1);
            else if (gameManager.GetComponent<GameManager>().bp == 3)
                SpawnLasers(laser1, laser0);
            else if (gameManager.GetComponent<GameManager>().bp == 4 || gameManager.GetComponent<GameManager>().bp == 5)
                SpawnLasers(laser1, laser1);
        }
    }

    // Spawn lasers out of turrets
    private void SpawnLasers (GameObject laser) {
        Vector3 leftTurret = transform.position;
        Vector3 rightTurret = transform.position;

        // Check animation to get correct positions
        if (currentAnimation.Equals("PlayerIdle")) {
            leftTurret += new Vector3(-1 * TURRET_X, TURRET_Y);
            rightTurret += new Vector3(TURRET_X, TURRET_Y);
        } else if (currentAnimation.Equals("PlayerLeft")) {
            leftTurret += new Vector3(-1 * CLOSE_TURRET_X, TURRET_Y);
            rightTurret += new Vector3(FAR_TURRET_X, TURRET_Y);
        } else if (currentAnimation.Equals("PlayerRight")) {
            leftTurret += new Vector3(-1 * FAR_TURRET_X, TURRET_Y);
            rightTurret += new Vector3(CLOSE_TURRET_X, TURRET_Y);
        }

        // Spawn lasers
        Instantiate(laser, leftTurret, transform.rotation);
        Instantiate(laser, rightTurret, transform.rotation);
    }

    // Spawn lasers out of turrets (with spreadshot)
    private void SpawnLasers (GameObject forwardLaser, GameObject spreadLaser) {
        Vector3 leftTurret = transform.position;
        Vector3 rightTurret = transform.position;
        float angle1 = 30;
        float angle2 = 60;

        // Check animation to get correct positions
        if (currentAnimation.Equals("PlayerIdle")) {
            leftTurret += new Vector3(-1 * TURRET_X, TURRET_Y);
            rightTurret += new Vector3(TURRET_X, TURRET_Y);
        } else if (currentAnimation.Equals("PlayerLeft")) {
            leftTurret += new Vector3(-1 * CLOSE_TURRET_X, TURRET_Y);
            rightTurret += new Vector3(FAR_TURRET_X, TURRET_Y);
        } else if (currentAnimation.Equals("PlayerRight")) {
            leftTurret += new Vector3(-1 * FAR_TURRET_X, TURRET_Y);
            rightTurret += new Vector3(CLOSE_TURRET_X, TURRET_Y);
        }

        // Spawn lasers
        Instantiate(forwardLaser, leftTurret, transform.rotation);
        Instantiate(forwardLaser, rightTurret, transform.rotation);
        Instantiate(spreadLaser, leftTurret, Quaternion.Euler(0, 0, transform.rotation.z + angle1));
        Instantiate(spreadLaser, rightTurret, Quaternion.Euler(0, 0, transform.rotation.z - angle1));
        Instantiate(spreadLaser, leftTurret, Quaternion.Euler(0, 0, transform.rotation.z + angle2));
        Instantiate(spreadLaser, rightTurret, Quaternion.Euler(0, 0, transform.rotation.z - angle2));
    }
}
