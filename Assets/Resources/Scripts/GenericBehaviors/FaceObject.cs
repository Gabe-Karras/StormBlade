using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceObject : MonoBehaviour
{
    // Object to face
    [SerializeField]
    private GameObject obj;

    // Update is called once per frame
    void Update()
    {
        // As long as object exists, look at it
        if (obj != null)
            transform.rotation = Quaternion.Euler(0, 0, GameSystem.FacePoint(transform.position, obj.transform.position));
    }
}
