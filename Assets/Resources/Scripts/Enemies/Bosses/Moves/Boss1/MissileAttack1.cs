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

    private float missileHeight;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Get vertical bounds of missile
        missileHeight = missile.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    public override IEnumerator ExecuteMove() {
        moveFinished = false;
        // Open doors if they aren't open already

        // Launch missile from silo

        // Bring exploding missile down

        // Deal damage

        // Set silo as active
        
        moveFinished = true;
        yield break;
    }
}
