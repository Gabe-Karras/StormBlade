using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This script will control the scrolling backgrounds throughout the game.
public class ScrollManager : MonoBehaviour
{

    // Which level is this taking place in? (to tell it where to find tilemaps)
    [SerializeField]
    private int level;

    // Directly corresponds to background0 speed
    [SerializeField]
    private float scrollSpeed;

    [SerializeField]
    private GameObject background0;
    [SerializeField]
    private GameObject background1;
    [SerializeField]
    private GameObject foreground;

    // How fast to increase speed for other layers
    private float background1Factor = 2;
    private float foregroundFactor = 3;

    // All currently existing scrolling backgrounds are stored in here.
    // 0, 2, 4 = background0, background1, foreground
    // Odd numbers are the 'next' background objects.
    private GameObject[] backgrounds = new GameObject[6];

    // Start is called before the first frame update
    void Start()
    {
        scrollSpeed /= GameSystem.SPEED_DIVISOR;

        // Instantiate backgrounds
        backgrounds[0] = Instantiate(background0, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        backgrounds[2] = Instantiate(background1, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        backgrounds[4] = Instantiate(foreground, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        // Set sprites
        backgrounds[0].GetComponent<SpriteRenderer>().sprite = chooseBackground(background0, level);
        backgrounds[2].GetComponent<SpriteRenderer>().sprite = chooseBackground(background1, level);
        backgrounds[4].GetComponent<SpriteRenderer>().sprite = chooseBackground(foreground, level);
    }

    // Update is called once per frame
    void Update()
    {
        scrollBackground(0, 1, scrollSpeed, background0);
        scrollBackground(2, 3, scrollSpeed * background1Factor, background1);
        scrollBackground(4, 5, scrollSpeed * foregroundFactor, foreground);
    }

    // Scroll background at speed
    private void scrollBackground(int current, int next, float speed, GameObject prefab) {
        if (backgrounds[current] == null) {
            return;
        }

        SpriteRenderer currentSprite = backgrounds[current].GetComponent<SpriteRenderer>();
        SpriteRenderer nextSprite;

        // If next background doesn't exist, create one
        if (backgrounds[next] == null) {
            backgrounds[next] = Instantiate(prefab);
            nextSprite = backgrounds[next].GetComponent<SpriteRenderer>();
            nextSprite.sprite = chooseBackground(prefab, level);
            float position = currentSprite.bounds.extents.y + backgrounds[current].transform.position.y + nextSprite.bounds.extents.y;
            backgrounds[next].transform.position += new Vector3(0, position, 0);
        }  else {
            nextSprite = backgrounds[next].GetComponent<SpriteRenderer>();
        }

        backgrounds[current].transform.position += new Vector3(0, -1 * speed, 0);
        backgrounds[next].transform.position += new Vector3(0, -1 * speed, 0);

        // Destroy current if it goes out of camera
        if (backgrounds[current].transform.position.y < -1 * GameSystem.Y_ACTION_BOUNDARY - currentSprite.bounds.extents.y) {
            Destroy(backgrounds[current]);
            backgrounds[current] = backgrounds[next];
            currentSprite = nextSprite;
            backgrounds[next] = null;
        }
    }

    // Pick random tilemaps from tilemap folder
    private Sprite chooseBackground(GameObject prefab, int level) {
        Sprite result = null;

        // First, get what sprite folder to look in
        if (prefab.name.Equals("Background0")) {
            // Generate random number to pick sprite
            System.Random r = new System.Random();
            int temp = r.Next(0, 3); // 0 - 2
            result = Resources.Load<Sprite>("Sprites/Tilesets/Level" + level + "/Background0/" + temp);
        } else if (prefab.name.Equals("Background1")) {
            result = Resources.Load<Sprite>("Sprites/Tilesets/Level" + level + "/Background1/Map");
        } else if (prefab.name.Equals("Foreground")) {
            result = Resources.Load<Sprite>("Sprites/Tilesets/Level" + level + "/Foreground/Map");
        }
        
        return result;
    }
    
}
