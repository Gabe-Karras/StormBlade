using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Important player variables
    [SerializeField]
    public int hp = 10;

    [SerializeField]
    public int bp = 1;

    public const int MAX_ACTION_HP = 10;
    public const int MAX_ACTION_BP = 5;

    // UI elements
    private GameObject healthCells;
    private GameObject blasterCells;

    // Start is called before the first frame update
    void Start()
    {
        // Set framerate
        Application.targetFrameRate = 60;

        // Get UI elements
        healthCells = GameObject.Find("HealthCells");
        blasterCells = GameObject.Find("BlasterCells");

        // Make sure key elements exist
        if (bp == 5 && GameObject.Find("LaserRing") == null) {
            Instantiate(Resources.Load("Prefabs/Player/LaserRing"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            UpdateBP(1);
        }
    }

    // Method to update all references of player hp
    public void UpdateHP(int hpChange) {
        hp += hpChange;

        // Make sure hp stays within bounds
        if (hp > MAX_ACTION_HP) {
            hp = MAX_ACTION_HP;
        } else if (hp < 0) {
            hp = 0;
        }

        // Update display
        Image img = healthCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/HealthCells")[hp];
    }

    public void UpdateBP(int bpChange) {
        bp += bpChange;

        // Make sure bp stays within bounds
        if (bp > MAX_ACTION_BP) {
            bp = MAX_ACTION_BP;
        } else if (bp < 1) {
            bp = 1;
        }

        // Update display
        Image img = blasterCells.GetComponent<Image>();
        img.sprite = (Sprite) Resources.LoadAll<Sprite>("Sprites/UI/BlasterCells")[bp];

        // Add/remove laser ring
        if (bp == 5 && GameObject.Find("LaserRing") == null) {
            Instantiate(Resources.Load("Prefabs/Player/LaserRing"));
        } else if (bp != 5 && GameObject.Find("LaserRing") != null) {
            Destroy(GameObject.Find("LaserRing"));
        }
    }
}