using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Status effect to hurt player with sparks
public class Electrocuted : StatusEffect
{
    private GameObject spark;
    private int damage = 15;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Set turns
        turns = 3;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spark = Resources.Load<GameObject>("Prefabs/Explosions/Spark");

        // Emit sparks on player
        StartCoroutine(GameSystem.StartExploding(gameObject.GetComponent<SpriteRenderer>(), spark));
    }

    public override void ExecuteEffect() {
        // Hit player
        StartCoroutine(WaitForEffect(2.5f));

        StartCoroutine(DamagePlayer());
    }

    private IEnumerator DamagePlayer() {
        yield return new WaitForSeconds(0.75f);
        gameObject.GetComponent<PlayerController>().Invincibility(1);

        // Randomize damage and turn negative
        int realDamage = (int) ((damage + (damage / 10 * GameSystem.RandomPercentage() * GameSystem.RandomSign())) * -1);
        gameManager.UpdateHp(realDamage);
    }
}
