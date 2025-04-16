using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBlast2 : BossMove
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

        // Shoot once
        GameSystem.PlaySoundEffect(laserSound, GetComponent<AudioSource>(), 0);

        GameObject temp = Instantiate(laser, cannon.transform.position, cannon.transform.rotation);
        temp.GetComponent<Laser>().SetTarget(player);
        yield return new WaitForSeconds(0.5f);

        // Deal damage!!
        gameManager.UpdateHp(RandomizeDamage(damage) * -1);

        // Apply status effect
        StatusEffect s = player.AddComponent<Electrocuted>();
        gameManager.ApplyStatusEffect(s);
        yield return new WaitForSeconds(0.5f);

        moveFinished = true;
        yield break;
    }
}
