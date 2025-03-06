using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    // Generic variables applying to all enemies
    [SerializeField]
    private int hp;
    [SerializeField]
    private int physicalDamage;
    [SerializeField]
    private bool facePlayer;

    // Other scripts dictating specific behavior
    [SerializeField]
    private EnemyBehavior behavior;


    private bool iframes = false;
    private bool dead = false;
    private GameObject player;
    private bool touchingPlayer;

    // Used for shading purposes when hit
    private Material defaultMaterial;

    private GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        defaultMaterial = gameObject.GetComponent<SpriteRenderer>().material;

        gameManager = GameObject.Find("GameManager");

        physicalDamage *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead) {
            // Face player if required
            if (facePlayer) {
                transform.rotation = Quaternion.Euler(0, 0, FacePoint(player.transform));
            }

            // Deal physical damage if colliding with player
            if (touchingPlayer) {
                gameManager.GetComponent<GameManager>().UpdateHp(physicalDamage);
            }

            // Execute behavior
            behavior.ExecuteBehavior();
        }
    }

    // Take damage from projectiles
    public void UpdateHp(int hpChange) {
        if (!iframes) {
            hp += hpChange;

            // Check for death
            if (hp <= 0) {
                dead = true;
                return;
            }

            // Flash sprite to be all white
            StartCoroutine(FlashWhite(0.05f));
        }
    }

    // Returns the Z angle to face the given point
    private float FacePoint(Transform other) {
        float angle;

        // Arctan formula
        angle = (float) Math.Atan((other.position.y - transform.position.y) / (other.position.x - transform.position.x));
        // Convert to degrees
        angle *= (float) (180 / Math.PI);
        // Add 90 for upward alignment
        angle += 90;
        // Adjust if target is on right side
        if (other.position.x > transform.position.x) {
            angle += 180;
        }

        return angle;
    }

    // Sets sprite to be white briefly and gives iframes
    IEnumerator FlashWhite(float iframeSeconds) {
        iframes = true;

        // Flash white for one 20th of a second
        gameObject.GetComponent<SpriteRenderer>().material = (Material) Resources.Load("Materials/SolidWhite");
        yield return new WaitForSeconds(0.05f);
        gameObject.GetComponent<SpriteRenderer>().material = defaultMaterial;

        // Wait out iframes
        yield return new WaitForSeconds(iframeSeconds);
        iframes = false;
    }

    // Deal with player collision
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Player")) {
            touchingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Player")) {
            touchingPlayer = false;
        }
    }

    public bool IsDead() {
        return dead;
    }
}
