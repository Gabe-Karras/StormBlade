using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Speed of laser
    [SerializeField]
    private float speed;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Dividing for vector math
        speed /= 50;
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
    }

    // Move laser forward at speed
    protected void MoveForward() {
        transform.position += transform.up * speed;
    }

    // Destroy laser when it leaves the camera
    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
