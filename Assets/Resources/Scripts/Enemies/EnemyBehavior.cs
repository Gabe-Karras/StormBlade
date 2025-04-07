using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class serves as a blueprint for all enemy behaviors.
// The real code is in methods that inherit from this class.
public abstract class EnemyBehavior : MonoBehaviour
{
    // This method will be called every frame the enemy is alive in the generic Enemy class.
    public abstract void ExecuteBehavior();

    // Quit any behavior code if enemy dies
    private void Update() {
        if (gameObject.GetComponent<Enemy>().GetHp() <= 0)
            Destroy(this);
    }
}
