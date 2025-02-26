using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Speed of laser
    [SerializeField]
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        // Dividing for vector math
        speed /= 50;
    }

    // Update is called once per frame
    void Update()
    {
        // Move laser forward
        transform.position += transform.up * speed;
    }

    // Destroy laser when it leaves the camera
    void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
