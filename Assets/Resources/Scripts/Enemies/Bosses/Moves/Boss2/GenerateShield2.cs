using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateShield2 : BossMove
{
    // References to shield generator and force field prefab
    [SerializeField]
    private GameObject shieldGenerator;
    [SerializeField]
    private GameObject forceField;
    private GameObject forceFieldInstance;
    private bool shieldFell = true;

    // Other component references for deactivation when shield is created
    [SerializeField]
    private GameObject concussion;
    [SerializeField]
    private GameObject thunder;
    [SerializeField]
    private GameObject vampire;
    [SerializeField]
    private GameObject body;

    // Light effects for animation
    [SerializeField]
    private GameObject energyRay;
    [SerializeField]
    private Material solidBlue;

    // Shield creation noise
    [SerializeField]
    private AudioClip shieldNoise;

    // Distance to spawn rays
    private const float LIGHT_DISTANCE = 0.32f;

    protected override void Start() {
        base.Start();

        forceFieldInstance = GameObject.Find("ForceField");
    }

    protected override void Update() {
        base.Update();

        // Activate components and add moves approprately after shield goes down
        if (forceFieldInstance == null && shieldFell == true) {
            boss.ActivateComponent(body.GetComponent<BossComponent>());

            if (shieldGenerator != null)
                boss.ActivateComponent(shieldGenerator.GetComponent<BossComponent>());

            if (concussion != null) {
                boss.ActivateComponent(concussion.GetComponent<BossComponent>());

                if (concussion.Equals(boss.GetComponent<SpinCannons2>().activeCannon))
                    boss.AddMove(boss.GetComponent<ConcussionBlast2>());
            }

            if (thunder != null) {
                boss.ActivateComponent(thunder.GetComponent<BossComponent>());
                
                if (thunder.Equals(boss.GetComponent<SpinCannons2>().activeCannon))
                    boss.AddMove(boss.GetComponent<ThunderBlast2>());
            }

            if (vampire != null) {
                boss.ActivateComponent(vampire.GetComponent<BossComponent>());
                
                if (vampire.Equals(boss.GetComponent<SpinCannons2>().activeCannon))
                    boss.AddMove(boss.GetComponent<VampireBlast2>());
            }
            
            boss.AddMove(GetComponent<SpinCannons2>());

            shieldFell = false;
        }

        if (forceFieldInstance != null)
            shieldFell = true;

        // On the turn after shield is destroyed, add this move
        if (!gameManager.GetBossTurn() && forceFieldInstance == null && shieldGenerator != null)
            boss.AddMove(this);

        // If shield generator is destroyed, remove this move
        if (shieldGenerator == null)
            boss.RemoveMove(this);
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;

        // Spawn rays and glow the generator
        GameObject temp = Instantiate(energyRay, transform.position + new Vector3(LIGHT_DISTANCE, LIGHT_DISTANCE + 0.05f, 0), Quaternion.Euler(0, 0, 0));
        Instantiate(energyRay, transform.position + new Vector3(LIGHT_DISTANCE, LIGHT_DISTANCE * -1 + 0.05f, 0), Quaternion.Euler(0, 0, 270));
        Instantiate(energyRay, transform.position + new Vector3(LIGHT_DISTANCE * -1, LIGHT_DISTANCE * -1 + 0.05f, 0), Quaternion.Euler(0, 0, 180));
        Instantiate(energyRay, transform.position + new Vector3(LIGHT_DISTANCE * -1, LIGHT_DISTANCE + 0.05f, 0), Quaternion.Euler(0, 0, 90));

        float glowTime = temp.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(GameSystem.FlashSprite(shieldGenerator.GetComponent<SpriteRenderer>(), solidBlue, time: glowTime));

        // Wait until animations are finished
        yield return new WaitForSeconds(glowTime + 1);

        // Create shield and play noise
        forceFieldInstance = Instantiate(forceField, boss.transform.position, boss.transform.rotation);
        forceFieldInstance.GetComponent<BossComponent>().SetBoss(boss);
        GameSystem.PlaySoundEffect(shieldNoise, boss.GetComponent<AudioSource>(), 0);

        // Add and activate force field
        boss.AddComponent(forceFieldInstance.GetComponent<BossComponent>());
        boss.ActivateComponent(forceFieldInstance.GetComponent<BossComponent>());

        // Deactivate all other components
        if (concussion != null)
            boss.DeactivateComponent(concussion.GetComponent<BossComponent>());
        if (thunder != null)
            boss.DeactivateComponent(thunder.GetComponent<BossComponent>());
        if (vampire != null)
            boss.DeactivateComponent(vampire.GetComponent<BossComponent>());
        boss.DeactivateComponent(body.GetComponent<BossComponent>());
        if (shieldGenerator != null)
            boss.DeactivateComponent(shieldGenerator.GetComponent<BossComponent>());

        yield return new WaitForSeconds(2);
        moveFinished = true;

        // Wait to remove bossmoves until boss script is done with it
        yield return new WaitForSeconds(0.2f);

        // Remove all moves but basic attack
        boss.RemoveMove(GetComponent<ConcussionBlast2>());
        boss.RemoveMove(GetComponent<ThunderBlast2>());
        boss.RemoveMove(GetComponent<VampireBlast2>());
        boss.RemoveMove(GetComponent<SpinCannons2>());
        boss.RemoveMove(this);
    }
}
