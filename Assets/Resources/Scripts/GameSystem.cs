using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class holds important constants concerning all objects of the game.
public class GameSystem : MonoBehaviour
{
    // Divide all serialized speeds by this to get appropriate frame speed for in-game physics
    public const int SPEED_DIVISOR = 50;

    // Boundaries of action game screen
    public const float X_ACTION_BOUNDARY = 1.06f;
    public const float Y_ACTION_BOUNDARY = 1.28f;

    public const float PIXELS_PER_UNIT = 100;
}
