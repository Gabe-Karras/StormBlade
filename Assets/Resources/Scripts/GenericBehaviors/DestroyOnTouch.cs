using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object is destroyed when it touches the collider of the specified object
public class DestroyOnTouch : MonoBehaviour
{
    // String name of object to look for
    [SerializeField]
    private string targetName;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find(targetName);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.Equals(target))
            Destroy(gameObject);
    }
}
