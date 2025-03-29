using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a generic class for a move a boss can do.
// Each move has an animation to perform as the move takes place.
public abstract class BossMove : MonoBehaviour
{
    // Reference to parent boss
    [SerializeField]
    protected Boss boss;

    // Chance the boss will perform this move (1 - 100)
    [SerializeField]
    protected int chance;

    // How much damage to inflict (or heal)
    [SerializeField]
    protected int damage = 60;

    // This should be used by all child classes to communicate with the parent boss!
    protected bool moveFinished;

    // Current boss data
    protected List<BossComponent> components;
    protected List<BossComponent> activeComponents;
    protected List<BossMove> moves;

    // Player reference
    protected GameObject player;
    protected GameManager gameManager;

    protected virtual void Start() {
        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Constantly get updated info from boss
    protected virtual void Update()
    {
        components = boss.GetComponents();
        activeComponents = boss.GetActiveComponents();
        moves = boss.GetMoves();
    }

    // The move itself
    public abstract IEnumerator ExecuteMove();

    // Method to randomize damage dealt by move
    protected int RandomizeDamage(int damage) {
        return damage + ((int) (damage / 10 * GameSystem.RandomPercentage() * GameSystem.RandomSign()));
    }

    public int GetChance() {
        return chance;
    }

    public bool IsFinished() {
        return moveFinished;
    }
}
