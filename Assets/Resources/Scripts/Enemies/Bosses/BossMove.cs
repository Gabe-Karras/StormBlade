using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a generic class for a move a boss can do.
// Each move has an animation to perform as the move takes place.
public abstract class BossMove : MonoBehaviour
{
    // Reference to parent boss
    [SerializeField]
    private Boss boss;

    // Chance the boss will perform this move
    [SerializeField]
    private float chance;

    // The move itself
    public abstract IEnumerator ExecuteMove();
}
