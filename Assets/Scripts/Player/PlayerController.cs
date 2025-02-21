using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player speed
    [SerializeField]
    private float speed = 4;

    // X coordinates for animation
    private float x;
    private float previousX;

    private Animator playerAnimator;

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
}
