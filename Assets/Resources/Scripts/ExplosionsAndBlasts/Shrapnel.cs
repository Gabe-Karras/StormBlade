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
        speed /= 50;
    }

    // Update is called once per frame
    void Update()
    {
        // Move in direction of angle
        float xChange = (float) (speed * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
        float yChange = (float) (speed * Math.Cos(angle * (Math.PI / 180)));

        transform.position += new Vector3(xChange, yChange, 0f);

        // Spin sprite with rotation speed
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        currentRotation = (currentRotation + rotSpeed) % 360;
    }

    // Destroy shrapnel when it leaves the camera
    void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public void SetAngle(float angle) {
        this.angle = angle;
    }
}
