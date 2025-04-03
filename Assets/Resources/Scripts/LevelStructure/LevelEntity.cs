using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents an entity what will spawn in a level.
public class LevelEntity
{
    // The object to spawn
    private GameObject obj;

    // The position to spawn at
    private Vector3 spawn;

    // If the entity is a wave timer it is set to this
    // 0 indicates 'clear enemies'
    private float timer = 0;

    public LevelEntity(GameObject obj, Vector3 spawn) {
        this.obj = obj;
        this.spawn = spawn;
    }

    public LevelEntity(GameObject obj, Vector3 spawn, float timer) {
        this.obj = obj;
        this.spawn = spawn;
        this.timer = timer;
    }

    public GameObject GetObject() {
        return obj;
    }

    public Vector3 GetSpawn() {
        return spawn;
    }

    public float GetTimer() {
        return timer;
    }
}
