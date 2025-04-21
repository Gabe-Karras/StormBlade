using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAttack1 : BossMove
{
    // Missile silo doors
    [SerializeField]
    private GameObject doors;

    // Missile silo
    [SerializeField]
    private GameObject silo;

    // Missile
    [SerializeField]
    private GameObject missile; 

    // Explosions when missile falls
    [SerializeField]
    private GameObject missileExplosion;

    private float missileHeight;
    private SpriteRenderer doorSprite;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Get vertical bounds of missile
        missileHeight = missile.GetComponent<SpriteRenderer>().bounds.size.y;
        doorSprite = doors.GetComponent<SpriteRenderer>();
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;
        // Open doors if they aren't open already
        bool doorsOpen = true;
        if (doorSprite.sprite.name.Equals("SiloDoors_0"))
            doorsOpen = false;

        if (!doorsOpen) {
            for (int i = 1; i <= 4; i ++) {
                doorSprite.sprite = Resources.LoadAll<Sprite>("Sprites/Enemies/Bosses/Boss1/SiloDoors")[i];
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Launch missile from silo
        yield return new WaitForSeconds(0.5f);
        Instantiate(missile, transform.position, transform.rotation);
        GameSystem.PlaySoundEffect(Resources.Load<AudioClip>("SoundEffects/Items/Missile"), GetComponent<AudioSource>(), 0, volume: 0.2f);

        // Bring exploding missile down
        yield return new WaitForSeconds(0.8f);
        Vector3 missilePosition = new Vector3(0, GameSystem.Y_ACTION_BOUNDARY + missileHeight / 2 - 0.01f, 0);
        Quaternion missileRotation = Quaternion.Euler(0, 0, 180);
        GameObject temp = Instantiate(missile, missilePosition, missileRotation);
        // Set sorting order to show above boss
        temp.GetComponent<SpriteRenderer>().sortingOrder = 20;
        StartCoroutine(GameSystem.StartExploding(temp.GetComponent<SpriteRenderer>(), missileExplosion));

        // Figure out when missile hits player
        while (temp != null) {
            if (temp.transform.position.y - missileHeight / 2 <= player.transform.position.y) {
                if (!player.GetComponent<PlayerController>().GetIframes())
                    player.GetComponent<PlayerController>().Invincibility(1);
            }

            yield return new WaitForSeconds(0.01f);
        }

        // Deal damage
        yield return new WaitForSeconds(0.8f);
        gameManager.UpdateHp(RandomizeDamage(damage) * -1);

        // Set silo as active
        boss.ActivateComponent(silo.GetComponent<BossComponent>());
        
        moveFinished = true;
        yield break;
    }
}
