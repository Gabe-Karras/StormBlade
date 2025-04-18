using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shrapnel : MonoBehaviour
{
    // Speed of object
    [SerializeField]
    private float speed;

    // Speed of rotation
    [SerializeField]
    private float rotSpeed;
    private float currentRotation = 0f;

    // Angle particle shoots out at
    [SerializeField]
    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;
        rotSpeed /= GameSystem.ROTATION_DIVISOR;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSystem.IsPaused()) {
            // Move in direction of angle
            transform.position += GameSystem.MoveAtAngle(angle, speed);

            // Spin sprite with rotation speed
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
            currentRotation = (currentRotation + rotSpeed) % 360;
        }
    }

    // Destroy shrapnel when it leaves the camera
    void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public void SetAngle(float angle) {
        this.angle = angle;
    }
}
