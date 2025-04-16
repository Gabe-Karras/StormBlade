using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Basic behavior to move an object at an angle at a given speed
public class MoveDirection : MonoBehaviour
{
    [SerializeField]
    private float angle = 0;
    [SerializeField]
    private float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSystem.IsPaused())
            transform.position += GameSystem.MoveAtAngle(angle, speed);
    }

    public void SetAngle(float a) {
        angle = a;
    }

    public void SetSpeed(float s) {
        speed = s / GameSystem.SPEED_DIVISOR;
    }
}
