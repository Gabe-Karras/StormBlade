using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawAttack1 : BossMove
{
    // Boss body
    [SerializeField]
    private GameObject body;

    // Sawblade arms
    [SerializeField]
    private GameObject leftArm;
    [SerializeField]
    private GameObject rightArm;

    [SerializeField]
    private GameObject doors;

    [SerializeField]
    private GameObject silo;

    private float bodyHeight;

    private GameObject leftSaw;
    private GameObject rightSaw;

    private float sawWidth;

    private float leftArmPosition;
    private float rightArmPosition;

    private SpriteRenderer doorSprite;

    // How fast to move arms together
    [SerializeField]
    private float armSpeed;

    // How fast to move entire boss body
    [SerializeField]
    private float bodySpeed;

    protected override void Start() {
        base.Start();

        // Get saw objects from arm parents
        leftSaw = leftArm.transform.Find("LeftSaw").gameObject;
        rightSaw = rightArm.transform.Find("RightSaw").gameObject;

        sawWidth = leftSaw.GetComponent<SpriteRenderer>().bounds.size.x;
        bodyHeight = body.GetComponent<SpriteRenderer>().bounds.extents.y;

        leftArmPosition = leftArm.transform.position.x;
        rightArmPosition = rightArm.transform.position.x;

        armSpeed /= GameSystem.SPEED_DIVISOR;
        armSpeed *= GameSystem.FRAME_RATE / 60;
        bodySpeed /= GameSystem.SPEED_DIVISOR;
        bodySpeed *= GameSystem.FRAME_RATE / 60;

        // Used in door closing animation
        doorSprite = doors.GetComponent<SpriteRenderer>();
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;
        // If silo is open, close doors and deactivate component
        bool doorsClosed = true;
        if (doorSprite.sprite.name.Equals("SiloDoors_4"))
            doorsClosed = false;

        if (!doorsClosed) {
            boss.DeactivateComponent(silo.GetComponent<BossComponent>());

            for (int i = 3; i >= 0; i --) {
                doorSprite.sprite = Resources.LoadAll<Sprite>("Sprites/Enemies/Bosses/Boss1/SiloDoors")[i];
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Slide saws together
        bool sawsTouching = false;

        while (!sawsTouching) {
            leftArm.transform.position += new Vector3(armSpeed, 0, 0);
            rightArm.transform.position -= new Vector3(armSpeed, 0, 0);

            // If the distance between the center of both saws is less than the width of one saw
            if (rightSaw.transform.position.x - leftSaw.transform.position.x <= sawWidth - 0.04)
                sawsTouching = true;

            yield return new WaitForSeconds(0.02f);
        }

        // Start emitting sparks
        GameObject particle = Resources.Load<GameObject>("Prefabs/Explosions/YellowParticle");
        float xOffset = sawWidth / 2 - 0.02f;
        StartCoroutine(GameSystem.EmitParticles(leftSaw, particle, xOffset, 0, 1f));

        yield return new WaitForSeconds(0.6f);

        // Move boss into player
        while (body.transform.position.y - player.transform.position.y > bodyHeight - 0.08f) {
            body.transform.position -= new Vector3(0, bodySpeed, 0);

            // Make player flash if saws touch
            if (body.transform.position.y - bodyHeight < player.transform.position.y + player.GetComponent<SpriteRenderer>().bounds.extents.y - 0.02f && !player.GetComponent<PlayerController>().GetIframes())
                player.GetComponent<PlayerController>().Invincibility(1f);

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.3f);

        // Go back to position
        while (GameSystem.PointDistance(body.transform.position, new Vector3(0, GameSystem.BOSS_POSITION, 0)) != 0) {
            body.transform.position += GameSystem.MoveTowardsPoint(body.transform.position, new Vector3(0, GameSystem.BOSS_POSITION, 0), bodySpeed);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.3f);

        // Unlock sawblades
        while (GameSystem.PointDistance(leftArm.transform.position, new Vector3(leftArmPosition, leftArm.transform.position.y, 0)) != 0) {
            leftArm.transform.position += GameSystem.MoveTowardsPoint(leftArm.transform.position, new Vector3(leftArmPosition, leftArm.transform.position.y, 0), armSpeed);
            rightArm.transform.position += GameSystem.MoveTowardsPoint(rightArm.transform.position, new Vector3(rightArmPosition, rightArm.transform.position.y, 0), armSpeed);
            yield return new WaitForSeconds(0.02f);
        }

        // Deal damage
        gameManager.UpdateHp(RandomizeDamage(damage) * -1);

        moveFinished = true;
        yield break;
    }
}
