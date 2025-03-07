using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class serves as a blueprint for all enemy deaths.
// The real code is in methods that inherit from this class.
public abstract class EnemyDeath : MonoBehaviour
{
    // Called once when hp runs out
    public abstract void ExecuteDeath();
}
