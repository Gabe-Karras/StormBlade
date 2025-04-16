using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot heavy lasers out of concussion cannon
public class ConcussionBlast2 : BossMove
{
    // Reference to cannon
    [SerializeField]
    private GameObject cannon;

    // Laser
    [SerializeField]
    private GameObject laser;

    // Sound effect
    [SerializeField]
    private AudioClip laserSound;

    protected override void Update() {
        base.Update();

        if (cannon == null)
            boss.RemoveMove(this);
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;

        // Shoot 3 times
        for (int i = 0; i < 3; i ++) {
            GameSystem.PlaySoundEffect(laserSound, GetComponent<AudioSource>(), 0);

            GameObject temp = Instantiate(laser, cannon.transform.position, cannon.transform.rotation);
            temp.GetComponent<Laser>().SetTarget(player);

            yield return new WaitForSeconds(0.2f);
        }

        // Deal damage!!
        yield return new WaitForSeconds(0.3f);
        gameManager.UpdateHp(RandomizeDamage(damage) * -1);

        moveFinished = true;
        yield break;
    }
}
