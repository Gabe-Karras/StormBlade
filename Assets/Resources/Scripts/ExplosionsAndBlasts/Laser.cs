using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Speed of laser
    [SerializeField]
    protected float speed;

    // Hp damage
    [SerializeField]
    private int damage;

    [SerializeField]
    private bool hurtsPlayer;
    [SerializeField]
    private bool hurtsEnemies;

    // Whether this destroys on impact from an enemy or not
    [SerializeField]
    private bool destroysOnHit;

    private GameObject gameManager;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Dividing for vector math
        speed /= 50;

        damage *= -1;

        gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        MoveForward();
    }

    // Move laser forward at speed
    protected void MoveForward() {
        transform.position += transform.up * speed;
    }

    // Destroy laser when it leaves the camera
    protected virtual void OnBecameInvisible() {
        Destroy(gameObject);
    }

    // Hurt target and destroy self on collision
    private void OnTriggerEnter2D(Collider2D other) {
        if (hurtsEnemies && other.gameObject.tag.Equals("Enemy")) {
            if (!other.gameObject.GetComponent<Enemy>().IsDead()) {
                other.gameObject.GetComponent<Enemy>().UpdateHp(damage);

                if (destroysOnHit)
                    Destroy(gameObject);
            }
        }

        if (hurtsPlayer && other.gameObject.tag.Equals("Player")) {
            gameManager.GetComponent<GameManager>().UpdateHp(damage);
            
            if (destroysOnHit)
                Destroy(gameObject);
        }
    }
}
