using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generic class for status effect behavior
public abstract class StatusEffect : MonoBehaviour
{
    // How many turns this effect lasts
    protected int turns;

    // This can be used to buffer the player's turn
    private bool inProgress = false;

    // Effect (What happens each turn)
    public abstract void ExecuteEffect();

    public bool EffectInProgress() {
        return inProgress;
    }

    public void SpendTurn() {
        turns --;

        if (turns == 0)
            Destroy(this);
    }

    protected IEnumerator WaitForEffect(float seconds) {
        inProgress = true;
        yield return new WaitForSeconds(seconds);
        inProgress = false;
    }
}
