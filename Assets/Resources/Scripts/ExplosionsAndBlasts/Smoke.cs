using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Travel in one direction and fade out of existence over time
public class Smoke : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float direction;
    [SerializeField]
    private float angleVariance;

    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private float fadeDelay;
    private float fadeAmount;
    private int totalFadeSteps;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;

        // Randomize orientation/trajectory
        transform.rotation = Quaternion.Euler(0, 0, GameSystem.RandomPercentage() * 360);
        angleVariance *= GameSystem.RandomPercentage() * GameSystem.RandomSign();
        direction += angleVariance;

        // Calculate fadeout step (How much to fade in one 50th of a second)
        totalFadeSteps = (int) (fadeTime / 0.02f);
        fadeAmount = 1f / totalFadeSteps;

        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        // Move in chosen trajectory
        transform.position += GameSystem.MoveAtAngle(direction, speed);
    }

    // Wait for fadeDelay seconds, then fade out and destroy in fadeTime seconds
    private IEnumerator FadeOut() {
        yield return new WaitForSeconds(fadeDelay);

        for (int i = 0; i < totalFadeSteps; i ++) {
            // Fade by one step
            Color temp = GetComponent<SpriteRenderer>().color;
            temp.a -= fadeAmount;
            GetComponent<SpriteRenderer>().color = temp;

            yield return new WaitForSeconds(0.02f); // One 50th of a second
        }

        Destroy(gameObject);
    }
}
