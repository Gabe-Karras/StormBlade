using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This script will control the scrolling backgrounds throughout the game.
public class ScrollManager : MonoBehaviour
{

    // Which level is this taking place in? (to tell it where to find tilemaps)
    [SerializeField]
    private string level;

    // Directly corresponds to background0 speed
    [SerializeField]
    private float scrollSpeed;

    [SerializeField]
    private bool hasBackground0;
    [SerializeField]
    private bool hasBackground1;
    [SerializeField]
    private bool hasForeground;

    // How fast to increase speed for other layers
    private float background1Factor = 1.5f;
    private float foregroundFactor = 2f;

    private GameObject current;
    private GameObject next;

    private SpriteRenderer currentSprite;
    private SpriteRenderer nextSprite;

    // Start is called before the first frame update
    void Start()
    {
        scrollSpeed /= GameSystem.SPEED_DIVISOR;

        current = GameObject.Find("Background0");
        currentSprite = current.GetComponent<SpriteRenderer>();
        current.transform.position += new Vector3(0, currentSprite.bounds.extents.y - GameSystem.Y_ACTION_BOUNDARY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // If next background doesn't exist, create one
        if (next == null) {
            next = Instantiate(Resources.Load<GameObject>("Prefabs/Background0"));
            nextSprite = next.GetComponent<SpriteRenderer>();
            float position = currentSprite.bounds.extents.y + current.transform.position.y + nextSprite.bounds.extents.y;
            next.transform.position += new Vector3(0, position, 0);
        }

        current.transform.position += new Vector3(0, -1 * scrollSpeed, 0);
        next.transform.position += new Vector3(0, -1 * scrollSpeed, 0);

        // Destroy current if it goes out of camera
        if (current.transform.position.y < -1 * GameSystem.Y_ACTION_BOUNDARY - currentSprite.bounds.extents.y) {
            Destroy(current);
            current = next;
            currentSprite = nextSprite;
            next = null;
        }
    }

    // Pick random tilemaps from tilemap folder
    /*
    Tilemap randomBackground0() {
        
    }
    */
}
