using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class serves as a blueprint for all enemy behaviors.
// The real code is in methods that inherit from this class.
public abstract class EnemyAttack : MonoBehaviour
{
    // Called once when behavior dictates it
    public abstract void ExecuteAttack();
}
