using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object exists to time the spawning of enemies for the level manager.
public class WaveTimer : MonoBehaviour
{
    // Destroy only once all enemies are gone
    private bool clearEnemies = true;

    // Seconds to wait before destroying (if 'clear enemies' is not selected)
    private float time = 0;

    void Start() {
        // Wait arbitrary amount of time
        if (!clearEnemies)
            StartCoroutine(GameSystem.DelayedDestroy(gameObject, time));
    }

    // Update is called once per frame
    void Update()
    {
        // If waiting till all enemies are gone:
        if (clearEnemies) {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
                Destroy(gameObject);
        }
    }

    public void SetTime(float time) {
        this.clearEnemies = false;
        this.time = time;
    }
}
