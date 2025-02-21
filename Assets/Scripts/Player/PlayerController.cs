using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player speed
    [SerializeField]
    private float speed = 4;

    // Start is called before the first frame update
    void Start()
    {
        // Divide for vector math
        speed = speed / 1000;
    
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
    }

    // Move player with arrow keys
    public void movePlayer() {
        if (Input.GetKey(KeyCode.RightArrow))
            transform.position += new Vector3(speed, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.position += new Vector3(-1 * speed, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow))
            transform.position += new Vector3(0, speed, 0);
        if (Input.GetKey(KeyCode.DownArrow))
            transform.position += new Vector3(0, -1 * speed, 0);
    }
}
