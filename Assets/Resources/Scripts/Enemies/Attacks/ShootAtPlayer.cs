using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot a projectile aimed at the player, regardless of what direction the enemy is facing
public class ShootAtPlayer : EnemyAttack
{
    [SerializeField]
    private GameObject laser;

    [SerializeField]
    private AudioClip laserShot;

    private GameObject player;
    private AudioSource source;

    void Start() {
        player = GameObject.Find("Player");
        source = GetComponent<AudioSource>();
    }

    public override void ExecuteAttack() {
        Instantiate(laser, transform.position, Quaternion.Euler(0, 0, GameSystem.FacePoint(transform.position, player.transform.position)));
        GameSystem.PlaySoundEffect(laserShot, source, 0);
    }
}
