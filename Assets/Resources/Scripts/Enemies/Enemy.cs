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
    [SerializeField]
    private EnemyDeath death;

    // Audio that plays when this enemy takes damage
    [SerializeField]
    private AudioSource damageSource;
    private AudioClip damage;

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

        damage = Resources.Load<AudioClip>("SoundEffects/Damage/DamageEnemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead) {
            // Face player if required
            if (facePlayer) {
                transform.rotation = Quaternion.Euler(0, 0, GameSystem.FacePoint(transform.position, player.transform.position));
            }

            // Deal physical damage if colliding with player
            if (touchingPlayer) {
                gameManager.GetComponent<GameManager>().UpdateHp(physicalDamage);
            }

            // Execute behavior
            behavior.ExecuteBehavior();
        } else {
            // Play death animation
            death.ExecuteDeath();
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
            GameSystem.PlaySoundEffect(damage, damageSource, 0.3f);
        }
    }

    // Sets sprite to be white briefly and gives iframes
    IEnumerator FlashWhite(float iframeSeconds) {
        iframes = true;

        // Flash white for one 20th of a second
        StartCoroutine(GameSystem.FlashSprite(GetComponent<SpriteRenderer>(), (Material) Resources.Load("Materials/SolidWhite")));

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
