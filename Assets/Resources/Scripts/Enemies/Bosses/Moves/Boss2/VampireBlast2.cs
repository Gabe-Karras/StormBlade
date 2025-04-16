using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Zap out player's health and give it to boss
public class VampireBlast2 : BossMove
{
    // Reference to cannon
    [SerializeField]
    private GameObject cannon;
    // Reference to body for flashing/healing
    [SerializeField]
    private GameObject body;

    // Health drain object
    [SerializeField]
    private GameObject healthDrain;
    // Material to flash boss when healing
    [SerializeField]
    private Material healMaterial;

    // Boss heal sound effect
    [SerializeField]
    private AudioClip healSound;

    protected override void Update() {
        base.Update();

        if (cannon == null)
            boss.RemoveMove(this);
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;

        // Deal damage immediately and store value for healing
        player.GetComponent<PlayerController>().Invincibility(1);

        // Drink in some health objects
        for (int i = 0; i < 5; i ++) {
            // Get random position on player sprite
            float randomX = 0.15f * GameSystem.RandomPercentage() * GameSystem.RandomSign();
            Vector3 point = player.transform.position + new Vector3(randomX, 0, 0);

            // Get angle to cannon mouth
            float angle = GameSystem.FacePoint(point, cannon.transform.position);

            MoveDirection temp = Instantiate(healthDrain, point, Quaternion.Euler(0, 0, angle)).GetComponent<MoveDirection>();
            temp.SetAngle(angle);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        // Flash the body and play sound effect
        GameSystem.PlaySoundEffect(healSound, GetComponent<AudioSource>(), 0);
        for (int i = 0; i < 5; i ++) {
            StartCoroutine(GameSystem.FlashSprite(body.GetComponent<SpriteRenderer>(), healMaterial, time: 0.1f));
            yield return new WaitForSeconds(0.2f);
        }
        
        int healAmount = gameManager.UpdateHp(RandomizeDamage(damage) * -1);
        body.GetComponent<BossComponent>().UpdateHp(Math.Abs(healAmount));
        yield return new WaitForSeconds(1);

        moveFinished = true;
        yield break;
    }
}
