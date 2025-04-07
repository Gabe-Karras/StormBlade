using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip explosionSound;

    // Does this explosion hurt entities?
    [SerializeField]
    private bool hurtsPlayer = false;
    [SerializeField]
    private bool hurtsEnemies = false;

    [SerializeField]
    private int damage = 0;

    private AnimatorStateInfo info;
    private GameManager gameManager;

    // Play sound!!
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (explosionSound != null)
            GameSystem.PlaySoundEffect(explosionSound, GetComponent<AudioSource>(), 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (hurtsPlayer && other.gameObject.tag.Equals("Player")) {
            gameManager.UpdateHp(-1 * damage);
        }

        if (hurtsEnemies && other.gameObject.tag.Equals("Enemy")) {
            if (GetComponent<Enemy>() != null)
                other.GetComponent<Enemy>().UpdateHp(-1 * damage);
        }
    }

    // Method corresponds to event in animation
    public void AnimationHandler(string message)
    {
        // Destroy this object once animation ends
        if (message.Equals("AnimationEnded"))
        {
            Destroy(gameObject);
        }
    }
}
