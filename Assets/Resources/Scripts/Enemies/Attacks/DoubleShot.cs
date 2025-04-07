using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot forwards twice in succession
public class DoubleShot : EnemyAttack
{
    [SerializeField]
    private GameObject laser;
    
    [SerializeField]
    private AudioClip laserShot;

    [SerializeField]
    private float waitTime;

    // First shot
    public override void ExecuteAttack() {
        Instantiate(laser, transform.position, transform.rotation);
        GameSystem.PlaySoundEffect(laserShot, GetComponent<AudioSource>(), 0);

        StartCoroutine(WaitForSecond());
    }

    // Second shot
    private IEnumerator WaitForSecond() {
        yield return new WaitForSeconds(waitTime);

        Instantiate(laser, transform.position, transform.rotation);
        GameSystem.PlaySoundEffect(laserShot, GetComponent<AudioSource>(), 0);
    }
}
