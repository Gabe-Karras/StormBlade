using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple script to chase player
public class ChasePlayer : EnemyBehavior
{
    [SerializeField]
    private float chaseSpeed;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        chaseSpeed /= GameSystem.SPEED_DIVISOR;

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    public override void ExecuteBehavior()
    {
        if (transform.position != player.transform.position)
            transform.position += GameSystem.MoveTowardsPoint(transform.position, player.transform.position, chaseSpeed);
    }
}
