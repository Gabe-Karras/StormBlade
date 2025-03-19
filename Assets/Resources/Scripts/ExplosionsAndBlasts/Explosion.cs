using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip explosionSound;

    private AnimatorStateInfo info;

    // Play sound!!
    void Start()
    {
        if (explosionSound != null)
            GameSystem.PlaySoundEffect(explosionSound, GetComponent<AudioSource>(), 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        
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
