using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableColliderTest : MonoBehaviour
{
    public bool collectableIsTouching;

    public void Start() {
    }

    private void OnTriggerEnter(Collider other) {
        collectableIsTouching = true;
        print("colliding with " + other);
    }
    
    private void OnTriggerExit(Collider other) {
        collectableIsTouching = false;
        print("Exiting Collision");
    }
    public bool CheckIfCollectablesAreTouching() {
        return collectableIsTouching;
    }
}
