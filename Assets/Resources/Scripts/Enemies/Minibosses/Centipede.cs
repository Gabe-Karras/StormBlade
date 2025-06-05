using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the top-level centipede object!
public class Centipede : MonoBehaviour
{
    [SerializeField]
    private GameObject head;
    [SerializeField]
    private GameObject rightSegment;
    [SerializeField]
    private GameObject leftSegment;
    [SerializeField]
    private int segmentCount;
    [SerializeField]
    private float segmentSpawnTime = 0.5f;
    
    private List<GameObject> segments;
    private bool headDestroyed = false;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(spawnSegments(segmentCount));
        segments = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // If head is destroyed, kill all remaining segments
        if (head != null && !headDestroyed && head.GetComponent<Enemy>().IsDead()) {
            for (int i = 0; i < segments.Count; i ++) {
                if (segments[i] != null) {
                    Enemy e = segments[i].GetComponent<Enemy>();
                    if (!e.IsDead())
                        e.SetHp(0);
                }
            }

            headDestroyed = true;
        }
    }

    // Spawn segments systematically while head is alive
    private IEnumerator spawnSegments(int count) {
        yield return new WaitForSeconds(segmentSpawnTime);
        GameObject toSpawn = leftSegment;
        if (count % 2 == 1)
            toSpawn = rightSegment;

        if (head != null && !headDestroyed) {
            segments.Add(Instantiate(toSpawn, startPos, Quaternion.Euler(0, 0, 0)));
            count --;
            if (count > 0)
                StartCoroutine(spawnSegments(count));
        }
    }
}
