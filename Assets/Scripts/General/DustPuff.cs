using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DustPuff : MonoBehaviour {

    //this script is run as soon as the DustPuff gameobject is added to the scene
    //after the particle system is finished doing its particle stuff, this script deletes the object

    private void Start() {
        float particleDuration = GetComponent<ParticleSystem>().main.duration;
        StaticVariables.WaitTimeThenCallFunction(particleDuration, DestroyThis);
    }

    private void DestroyThis() {
        Destroy(gameObject);
    }
    
}
