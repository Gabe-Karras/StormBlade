using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to a star background object
public class Star : MonoBehaviour
{
    // Speed at which stars move (far away stars will move at half this speed)
    [SerializeField]
    private float speed;

    private Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        // Normalize speed
        speed /= GameSystem.SPEED_DIVISOR;

        // Get random sprite
        System.Random r = new System.Random();
        int spriteChoice = r.Next(4);

        sprite = Resources.Load<Sprite>("Sprites/Tilesets/Star" + spriteChoice);
        GetComponent<SpriteRenderer>().sprite = sprite;

        // If sprite is small, turn down speed
        if (spriteChoice < 2)
            speed /= 2;

        // Randomize speed
        speed += speed / 2 * GameSystem.RandomPercentage() * GameSystem.RandomSign();
    }

    // Update is called once per frame
    void Update()
    {
        // Move to the left
        transform.position -= new Vector3(speed, 0, 0);
    }

    // Destroy once off screen
    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
