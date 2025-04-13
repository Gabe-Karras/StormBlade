using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bomb that chases player
public class HomingBomb : MonoBehaviour
{
    // Speed to pursue target
    [SerializeField]
    private float speed;

    // Explosion to result in when hit/finding target
    [SerializeField]
    private GameObject explosion;

    // Player target
    private GameObject target;

    private bool hit = false;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        speed /= GameSystem.SPEED_DIVISOR;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        target = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.IsPaused()) {
            transform.position += GameSystem.MoveTowardsPoint(transform.position, target.transform.position, speed);

            // Destroy and explode if hit
            if (hit) {
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    // If set up as an enemy to be hit
    public void SetHit() {
        hit = true;
    }

    // Explode if found target
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Player"))
            hit = true;
    }
}
