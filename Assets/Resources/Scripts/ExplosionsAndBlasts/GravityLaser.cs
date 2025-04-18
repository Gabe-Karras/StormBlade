using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLaser : Laser
{
    // How hard to pull the laser back down
    [SerializeField]
    private float gravityForce;
    private float gravity = 0;

    // How fast the laser can skew laterally
    [SerializeField]
    private float maxDrift;
    private float drift;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        maxDrift /= GameSystem.SPEED_DIVISOR;
        gravityForce /= GameSystem.SPEED_DIVISOR;
        gravityForce *= 50;
        drift = maxDrift * GameSystem.RandomPercentage() * GameSystem.RandomSign();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!GameSystem.IsPaused()) {
            transform.position += new Vector3(drift, speed - (gravity / (GameSystem.FRAME_RATE / 60)), 0);
            gravity += gravityForce;
        }
    }
}
