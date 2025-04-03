using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents an entity what will spawn in a level.
public class LevelEntity : MonoBehaviour
{
    // The object to spawn
    private GameObject obj;

    // The position to spawn at
    private Vector3 spawn;

    public LevelEntity(GameObject obj, Vector3 spawn) {
        this.obj = obj;
        this.spawn = spawn;
    }

    public GameObject GetObject() {
        return obj;
    }

    public Vector3 GetSpawn() {
        return spawn;
    }
}
