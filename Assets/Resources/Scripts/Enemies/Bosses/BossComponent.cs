using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents a component of a boss
// Active components have their own health or can be a direct pipe to the boss' health
// If a component is not active, the health is not touched
public class BossComponent : MonoBehaviour
{
    // The boss this component is attached to
    [SerializeField]
    private Boss boss;

    // Health of this component (disregard if component will not be active)
    [SerializeField]
    private int hp = 1;

    // Whether the health is shared by the whole boss or not
    [SerializeField]
    private bool hasTotalBossHealth;

    // Level of defense this boss has (What to divide incoming attacks by)
    [SerializeField]
    private float defense = 1;

    // The text that will pop up when this component is selected
    [SerializeField]
    private string componentName;

    // The type of explosion this component will have when it is destroyed
    [SerializeField]
    private GameObject explosion;

    // How long it takes the standard component to destroy
    private float deathTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Update hp up or down after an animation is performed
    // Returns change if modified by defense
    public int UpdateHp(int change) {
        // If change is negative, apply defense
        if (change < 0) {
            change = (int) (change / defense);
        }

        hp += change;
        return change;
    }

    // Flash and explode!
    private void Death(int seconds) {
        StartCoroutine(GameSystem.DelayedDestroy(gameObject, seconds));
        StartCoroutine(GameSystem.FlickerSprite(GetComponent<SpriteRenderer>(), seconds));
        StartCoroutine(GameSystem.StartExploding(GetComponent<SpriteRenderer>(), explosion));
    }

    // Getters and setters yooooo
    public int GetHp() {
        return hp;
    }

    public void SetDeathTime(float seconds) {
        deathTime = seconds;
    }

    public string GetName() {
        return componentName;
    }
}
