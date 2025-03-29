using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Angle turrets at player and unleash fireball attack
public class TurretAttack1 : BossMove
{
    // Turret components
    [SerializeField]
    private GameObject leftTurret;
    [SerializeField]
    private GameObject rightTurret;

    // Fireball prefab
    [SerializeField]
    private GameObject fireball;

    // Speed at which turrets rotate
    [SerializeField]
    private float rotSpeed = 5;

    // Update is called once per frame
    protected override void Update()
    {
        // Get most current boss data
        base.Update();

        // If both turrets are destroyed, remove move from list
        if (leftTurret == null && rightTurret == null) {
            boss.RemoveMove(this);
        }
    }

    // Angle turrets at player, fire volley of fireballs
    public override IEnumerator ExecuteMove() {
        moveFinished = false;
        bool angled = false;

        // Turn turrets to face player
        while (!angled) {
            angled = true;

            if (leftTurret != null && leftTurret.transform.eulerAngles.z != GameSystem.FacePoint(leftTurret.transform.position, player.transform.position)) {
                angled = false;
                leftTurret.transform.rotation = GameSystem.TurnToAngle(leftTurret.transform.eulerAngles.z, GameSystem.FacePoint(leftTurret.transform.position, player.transform.position), rotSpeed);
            }

            if (rightTurret != null && rightTurret.transform.eulerAngles.z != GameSystem.FacePoint(rightTurret.transform.position, player.transform.position)) {
                angled = false;
                rightTurret.transform.rotation = GameSystem.TurnToAngle(rightTurret.transform.eulerAngles.z, GameSystem.FacePoint(rightTurret.transform.position, player.transform.position), rotSpeed);
            }

            yield return new WaitForSeconds(0.02f);
        }

        // Shoot fireballs
        for (int i = 0; i < 3; i ++) {
            GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Projectiles/HeavyLaser"), boss.GetComponent<AudioSource>(), 0);

            if (leftTurret != null) {
                GameObject temp = Instantiate(fireball, leftTurret.transform.position, leftTurret.transform.rotation);
                temp.GetComponent<Laser>().SetTarget(player);
            }

            if (rightTurret != null) {
                GameObject temp = Instantiate(fireball, rightTurret.transform.position, rightTurret.transform.rotation);
                temp.GetComponent<Laser>().SetTarget(player);
            }

            yield return new WaitForSeconds(0.2f);
        }

        // Turn cannons back
        angled = false;
        while (!angled) {
            angled = true;

            if (leftTurret != null && leftTurret.transform.eulerAngles.z != 180) {
                angled = false;
                leftTurret.transform.rotation = GameSystem.TurnToAngle(leftTurret.transform.eulerAngles.z, 180, rotSpeed);
            }

            if (rightTurret != null && rightTurret.transform.eulerAngles.z != 180) {
                angled = false;
                rightTurret.transform.rotation = GameSystem.TurnToAngle(rightTurret.transform.eulerAngles.z, 180, rotSpeed);
            }

            yield return new WaitForSeconds(0.02f);
        }

        // Deal damage!!
        int tempDamage = damage;
        if (leftTurret == null || rightTurret == null)
            tempDamage /= 2;

        gameManager.UpdateHp(RandomizeDamage(tempDamage) * -1);

        moveFinished = true;
        yield break;
    }
}
