using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    // Time between flashing on and off
    [SerializeField]
    private float flashTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(flicker());
    }

    private IEnumerator flicker() {
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        Color temp = s.color;

        // Loop for flash time
        while (true) {
            // Flash alpha color
            if (temp.a == 1)
                temp.a = 0f;
            else 
                temp.a = 1f;

            s.color = temp;

            // Wait for flash time
            yield return new WaitForSeconds(flashTime);
        }
    }
}
