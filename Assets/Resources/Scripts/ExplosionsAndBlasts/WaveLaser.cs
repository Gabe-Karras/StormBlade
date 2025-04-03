using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveLaser : Laser
{
    // Max speed of wave
    [SerializeField]
    float horizontalSpeed;

    // How fast the turnaround is (low = faster)
    [SerializeField]
    float velocity;

    // What direction does this initially fly in? (-1 = left, 1 = right)
    [SerializeField]
    int direction;

    float realSpeed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        horizontalSpeed /= GameSystem.SPEED_DIVISOR;
        realSpeed = horizontalSpeed * direction;
        velocity = horizontalSpeed / velocity * direction;
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Reverse direction if speed gets too great
        if (realSpeed >= horizontalSpeed) {
            velocity *= -1;
            realSpeed = horizontalSpeed;
        } else if (realSpeed <= horizontalSpeed * -1) {
            velocity *= -1;
            realSpeed = horizontalSpeed * -1;
        }

        // Move in wave
        base.MoveForward();
        transform.position += transform.right * realSpeed;
        realSpeed += velocity;
    }

    // Accepts 1 or -1. Sets immediate wave direction.
    public void SetDirection(int direction) {
        this.direction = direction;
    }
}
