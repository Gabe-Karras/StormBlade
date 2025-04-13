using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic behavior to move an object at an angle at a given speed
public class MoveDirection : MonoBehaviour
{
    [SerializeField]
    private float angle;
    [SerializeField]
    private float speed;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsPaused())
            transform.position += GameSystem.MoveAtAngle(angle, speed);
    }
}
