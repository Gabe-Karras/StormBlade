using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Destroy self after limited time
public class PopUp : MonoBehaviour
{
    [SerializeField]
    private float existTime;

    [SerializeField]
    private bool flash;

    // Start is called before the first frame update
    void Start()
    {
        // If exist time = 0, exist forever
        if (existTime != 0)
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, existTime));
        if (flash)
            StartCoroutine(GameSystem.FlickerSprite(gameObject.GetComponent<SpriteRenderer>(), existTime, flashTime: 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
