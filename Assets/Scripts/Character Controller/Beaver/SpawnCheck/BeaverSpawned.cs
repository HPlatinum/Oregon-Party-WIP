using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverSpawned : MonoBehaviour
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
}
