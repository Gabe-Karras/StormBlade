using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRing : MonoBehaviour
{
    [SerializeField]
    private float fireSpeed = 0;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject target;

    // Distance from center to barrels
    private const float BIG_RING_DISTANCE = (float) 0.23;
    private const float SMALL_RING_DISTANCE = (float) 0.12;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.transform.position;

        if (fireSpeed > 0) {
            StartCoroutine(LaserTimer());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Always circle around the target
        transform.position = target.transform.position;
    }

    IEnumerator LaserTimer() {
        while (true) {
            yield return new WaitForSeconds(1 / fireSpeed);
            Debug.Log("Shoot code goes here!!");
        }
    }
}
