using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Destroy self after limited time
public class PopUp : MonoBehaviour
{
    [SerializeField]
    private float existTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameSystem.DelayedDestroy(gameObject, existTime));
        StartCoroutine(GameSystem.FlickerSprite(gameObject.GetComponent<SpriteRenderer>(), existTime, flashTime: 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
