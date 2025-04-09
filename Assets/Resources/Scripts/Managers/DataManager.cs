using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Persistent object that holds onto player game data
public class DataManager : MonoBehaviour
{
    [SerializeField]
    private int level;

    [SerializeField]
    private int bp;

    [SerializeField]
    private int bombCount;
    [SerializeField]
    private int lightningCount;
    [SerializeField]
    private int missileCount;
    [SerializeField]
    private int shieldCount;
    [SerializeField]
    private int smallHealthCount;
    [SerializeField]
    private int bigHealthCount;

    // Static reference to this class
    public static DataManager Instance;

    // Make object persistent
    private void Awake() {
        // Set framerate
        Application.targetFrameRate = GameSystem.FRAME_RATE;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Returns array of all data
    public int[] GetData() {
        int[] data = {level, bp, bombCount, lightningCount, missileCount, shieldCount, smallHealthCount, bigHealthCount};
        return data;
    }

    // Update data from game manager
    public void SetData(int[] data) {
        level = data[0];
        bp = data[1];
        bombCount = data[2];
        lightningCount = data[3];
        missileCount = data[4];
        shieldCount = data[5];
        smallHealthCount = data[6];
        bigHealthCount = data[7];
    }
}
