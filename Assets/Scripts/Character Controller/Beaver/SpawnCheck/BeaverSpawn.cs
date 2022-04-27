using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    bool beaverSpawned;
    private void Start() {
        beaverSpawned = false;
    }

    public void SpawnBeaver() {
        beaverSpawned = !beaverSpawned;
    }

    public bool BeaverHasBeenSpawned() {
        return beaverSpawned;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.parent.gameObject.GetComponent<BeaverController>().hasWoodInHand) {
            StaticVariables.woodcuttingHandler.DestroyBeaver(other.transform.parent.gameObject.GetComponent<BeaverController>().beaverSpawnInt, other.transform.parent.gameObject);
        }
    }
}
