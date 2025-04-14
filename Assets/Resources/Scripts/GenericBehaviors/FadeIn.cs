using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Start this object's alpha at zero and slowly fade it in
public class FadeIn : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed;
    private SpriteRenderer sRenderer;

    // Start is called before the first frame update
    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        Color temp = sRenderer.color;
        temp.a = 0;
        sRenderer.color = temp;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSystem.IsPaused()) {
            Color temp = sRenderer.color;
            temp.a += fadeSpeed;
            sRenderer.color = temp;
        }
    }
}
