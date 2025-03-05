using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private AnimatorStateInfo info;

    // Start is called before the first frame update
    void Start()
    {
        
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
