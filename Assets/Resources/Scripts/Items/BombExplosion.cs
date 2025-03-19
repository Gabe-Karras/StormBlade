using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unique behaviors exclusive to the bomb explosion
public class BombExplosion : MonoBehaviour
{
    [SerializeField]
    private GameObject smallerExplosion;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameSystem.StartExploding(GetComponent<SpriteRenderer>(), smallerExplosion));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On contact with an enemy, change death behavior to 'scorch' and set health to 0
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Enemy")) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            other.gameObject.AddComponent<Scorch>();
            enemy.SetDeathBehavior(other.gameObject.GetComponent<Scorch>());
            enemy.UpdateHp(-1000);
        }
    }
}
