using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Speed of rotation
    [SerializeField]
    private float rotSpeed;
    private float currentRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        // Spin sprite with rotation speed
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        currentRotation = (currentRotation + rotSpeed) % 360;
    }
}
