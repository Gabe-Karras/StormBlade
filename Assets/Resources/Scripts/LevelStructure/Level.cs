using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generic class that represents a level. Contains all information in a large queue
public class Level : MonoBehaviour
{
    // List of bad guys/items/wave timers to spawn
    private Queue<LevelEntity> levelQueue;

    public void AddEntity(LevelEntity e) {
        levelQueue.Enqueue(e);
    }

    public Queue<LevelEntity> GetQueue() {
        return levelQueue;
    }
}
