using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shoot player with lasers out of twin cannons
public class BasicAttack2 : BossMove
{
    // Reference to central body component
    [SerializeField]
    private GameObject body;

    // Laser
    [SerializeField]
    private GameObject laser;

    // Shoot noise
    [SerializeField]
    private AudioClip laserSound;

    public override IEnumerator ExecuteMove () {
        moveFinished = false;

        // Get (x, y) coordinates and angles to spawn lasers
        Vector3 rightCannon = body.transform.position + new Vector3(0.24f, -0.22f, 0);
        Vector3 leftCannon = body.transform.position + new Vector3(-0.24f, -0.22f, 0);
        Quaternion rightAngle = Quaternion.Euler(0, 0, GameSystem.FacePoint(rightCannon, player.transform.position));
        Quaternion leftAngle = Quaternion.Euler(0, 0, GameSystem.FacePoint(leftCannon, player.transform.position));

        // Shoot 3 times
        for (int i = 0; i < 3; i ++) {
            GameSystem.PlaySoundEffect(laserSound, boss.GetComponent<AudioSource>(), 0);

            GameObject temp = Instantiate(laser, rightCannon, rightAngle);
            temp.GetComponent<Laser>().SetTarget(player);

            temp = Instantiate(laser, leftCannon, leftAngle);
            temp.GetComponent<Laser>().SetTarget(player);

            yield return new WaitForSeconds(0.2f);
        }

        // Deal damage!!
        yield return new WaitForSeconds(0.4f);
        gameManager.UpdateHp(RandomizeDamage(damage) * -1);

        moveFinished = true;
        yield break;
    }
}
