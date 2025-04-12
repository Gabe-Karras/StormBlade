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

    // If you want the laser to spawn higher/lower on the enemy
    [SerializeField]
    private float yOffset = 0;

    private void Start() {
        yOffset /= GameSystem.PIXELS_PER_UNIT;
    }

    public override void ExecuteAttack() {
        Instantiate(laser, transform.position + new Vector3(0, yOffset, 0), transform.rotation);
        if (laserShot != null)
            GameSystem.PlaySoundEffect(laserShot, laserShotSource, 0);
    }
}
