using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Speed of rotation
    [SerializeField]
    private float rotSpeed;
    private float currentRotation = 0f;

    private GameManager gameManager;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsPaused()) {
            // Spin sprite with rotation speed
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
            currentRotation = (currentRotation + rotSpeed) % 360;
        }
    }
}
