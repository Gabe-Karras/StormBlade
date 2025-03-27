using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will be attached to any player shield prefabs that are spawned
public class Shield : MonoBehaviour
{
    // Shield can take 2 hits. 2 = full, 1 = flashing, 0 = destroy
    // In turn-based mode, the shield can withstand 6 hits!
    [SerializeField]
    private int shieldState = 2;
    [SerializeField]
    private int turnShieldState = 6;

    private GameObject player;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager.GetGameMode() == 1) {
            shieldState = turnShieldState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Keep x, y on player
        transform.position = player.transform.position;
    }

    // Increase or decrease the shield health and react visually
    public void UpdateShieldState(int update) {
        shieldState += update;
        
        if (shieldState == 1) {
            StartCoroutine(GameSystem.FlickerSprite(GetComponent<SpriteRenderer>(), 0));
        } else if (shieldState == 0) {
            Destroy(gameObject);
        }        
    }
}
