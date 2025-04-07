using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawn a single laser facing the direction of the enemy
public class BasicShot : EnemyAttack
{
    [SerializeField]
    private GameObject laser;

    [SerializeField]
    private AudioSource laserShotSource;
    [SerializeField]
    private AudioClip laserShot;

    public override void ExecuteAttack() {
        Instantiate(laser, transform.position, transform.rotation);
        if (laserShot != null)
            GameSystem.PlaySoundEffect(laserShot, laserShotSource, 0);
    }
}
