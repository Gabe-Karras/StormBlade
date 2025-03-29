using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the top-level generic class to be attached to boss parent objects
public class Boss : MonoBehaviour
{
    // Total health for this boss
    [SerializeField]
    private int maxHp;
    private int hp;

    // Seconds it takes to blow up
    [SerializeField]
    private float deathTime = 5;

    // List of components
    [SerializeField]
    private List<BossComponent> components;

    // List of currently interactible components (player can target)
    [SerializeField]
    private List<BossComponent> activeComponents;

    // List of moves that can currently be performed
    [SerializeField]
    private List<BossMove> moves;

    private bool startTurn = true;

    private GameManager gameManager;
    private MusicManager musicManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        musicManager = gameManager.GetMusicManager();

        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        // Take turn when game manager says so!
        //Debug.Log("bossturn: " + gameManager.GetBossTurn() + " startturn: " + startTurn);
        if (gameManager.GetBossTurn() && startTurn) {
            startTurn = false;
            StartCoroutine(TakeTurn());
        }
    }

    // Execute boss turn!
    public IEnumerator TakeTurn() {
        // First, check if any components have been destroyed
        for (int i = 0; i < components.Count; i ++) {
            if (components[i].GetHp() <= 0) {
                BossComponent temp = components[i];

                // Remove component from list
                RemoveComponent(temp);
                i --; // Loop offset

                // Destroy and wait until component is gone
                temp.Death();
                while (temp != null) {
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        // Then, check if the entire boss itself has been destroyed
        if (hp <= 0) {
            // Stop the boss music so he explodes in silence
            musicManager.StopMusic();

            // Explode everything at once!
            for (int i = 0; i < components.Count; i ++) {
                BossComponent temp = components[i];
                float tempDeathTime = deathTime;

                // Get randomized death time unless it is the most significant component (index 0)
                if (i != 0) {
                    // This generates a random value between 1/2 and full death time
                    tempDeathTime -= tempDeathTime / 2 * GameSystem.RandomPercentage();
                }

                // Set death time and destroy
                temp.SetDeathTime(tempDeathTime);
                temp.Death();

                // Destroy entire object at the end
                StartCoroutine(GameSystem.DelayedDestroy(gameObject, deathTime));
            }
        }

        // Pick a random move to execute!
        System.Random r = new System.Random();
        bool pickedMove = false;
        int choice = 0;

        // Get random move from list, and execute it if the chance is right
        while (!pickedMove) {
            choice = r.Next(moves.Count);
            BossMove temp = moves[choice];
            int chance = r.Next(101);

            if (chance <= moves[choice].GetChance())
                pickedMove = true;
        }

        StartCoroutine(moves[choice].ExecuteMove());

        // Wait until move is done
        while (!moves[choice].IsFinished()) {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        // Reset turn to player
        gameManager.SetBossTurn(false);
        startTurn = true;
        yield break;
    }

    // Add new component to list
    public void AddComponent(BossComponent component) {
        // Make sure it's not in the list already
        if (!components.Contains(component))
            components.Add(component);
    }

    // Remove component from all lists
    public void RemoveComponent(BossComponent component) {
        if (components.Contains(component))
            components.Remove(component);

        if (activeComponents.Contains(component))
            activeComponents.Remove(component);
    }

    // Set existing component active
    public void ActivateComponent(BossComponent component) {
        // Make sure it's not in the list already
        if (!activeComponents.Contains(component))
            activeComponents.Add(component);
    }

    // Remove active component
    public void DeactivateComponent(BossComponent component) {
        if (activeComponents.Contains(component))
            activeComponents.Remove(component);
    }

    // Add new move
    public void AddMove(BossMove move) {
        // Make sure it's not in the list already
        if (!moves.Contains(move))
            moves.Add(move);
    }

    // Remove boss move
    public void RemoveMove(BossMove move) {
        if (moves.Contains(move))
            moves.Remove(move);
    }

    // Get list of active components
    public List<BossComponent> GetActiveComponents() {
        return activeComponents;
    }

    // Get list of all components (used by boss moves)
    public List<BossComponent> GetComponents() {
        return components;
    }

    // Get list of all currently attached moves (used by boss moves)
    public List<BossMove> GetMoves() {
        return moves;
    }

    // Update health of entire boss
    public void UpdateHp(int change) {
        hp += change;

        if (hp > maxHp)
            hp = maxHp;
    }
}
