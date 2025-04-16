using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spin cannon orientation to give boss different moves
public class SpinCannons2 : BossMove
{
    // Parent object of cannons
    [SerializeField]
    private GameObject ring;

    // References to cannons
    [SerializeField]
    private GameObject concussion;
    [SerializeField]
    private GameObject thunder;
    [SerializeField]
    private GameObject vampire;

    private GameObject[] cannons;
    public GameObject activeCannon;
    private int cannonIndex = 0;

    private float currentAngle = 0;

    protected override void Start() {
        base.Start();

        cannons = new GameObject[] {concussion, thunder, vampire};
        activeCannon = concussion;
    }

    protected override void Update() {
        base.Update();

        // If only one cannon remains and it is aimed at player, or if all cannons are gone
        if ((ExistingCannons() == 1 && activeCannon != null) || ExistingCannons() == 0)
            boss.RemoveMove(this);
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;

        // Spin to correct angle
        float targetAngle = (currentAngle - 120 + 360) % 360;

        while (currentAngle != targetAngle) {
            currentAngle = (currentAngle - 4 + 360) % 360;
            ring.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            yield return new WaitForSeconds(0.02f);
        }

        moveFinished = true;

        // Wait to remove bossmoves until boss script is done with it
        yield return new WaitForSeconds(0.2f);
        SwitchCannonMoves();
        cannonIndex = (cannonIndex + 1) % 3;
        activeCannon = cannons[cannonIndex];
    }

    // Appropriately add/remove cannon moves for rotation
    private void SwitchCannonMoves() {
        if (activeCannon.Equals(concussion)) {
            boss.RemoveMove(GetComponent<ConcussionBlast2>());

            if (thunder != null)
                boss.AddMove(GetComponent<ThunderBlast2>());
        } else if (activeCannon.Equals(thunder)) {
            boss.RemoveMove(GetComponent<ThunderBlast2>());

            if (vampire != null)
                boss.AddMove(GetComponent<VampireBlast2>());
        } else if (activeCannon.Equals(vampire)) {
            boss.RemoveMove(GetComponent<VampireBlast2>());

            if (concussion != null)
                boss.AddMove(GetComponent<ConcussionBlast2>());
        }
    }

    // Check if more than one cannon still exists
    private int ExistingCannons() {
        int total = 0;

        if (concussion != null)
            total ++;
        if (thunder != null)
            total ++;
        if (vampire != null)
            total ++;

        return total;
    }
}
