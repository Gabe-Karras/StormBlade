using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fun animation to tack on to damage numbers in turn-based mode
public class TextBounce : MonoBehaviour
{
    // How fast to initially launch the text upwards
    [SerializeField]
    private float jumpSpeed;

    // How strong to pull the text back down
    [SerializeField]
    private float gravityForce;
    private float gravity = 0;

    // How fast to move the text in a horizontal direction
    [SerializeField]
    private float maxDrift;
    private float drift;

    // How far below the y-origin to stop the text
    [SerializeField]
    private float maxStopPoint;
    private float stopPoint;

    // How long for text to exist after bouncing
    [SerializeField]
    private float existTime;

    private bool hasBounced = false;
    private float initialY;
    private RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        initialY = rect.anchoredPosition.y;

        drift = maxDrift * GameSystem.RandomPercentage() * GameSystem.RandomSign();
        stopPoint = maxStopPoint * GameSystem.RandomPercentage();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasBounced) {
            rect.anchoredPosition += new Vector2(drift, jumpSpeed - gravity);
            gravity += gravityForce;

            if (rect.anchoredPosition.y < initialY - stopPoint)
                hasBounced = true;
        } else {
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, existTime));
        }
    }
}
