using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableColliderTest : MonoBehaviour
{
    public GameObject collectable;
    public ParticleSystem destroyParticle;

    public void Start() {
    }

    private void OnTriggerEnter(Collider other) {
        PlayDestroyParticle();
        StaticVariables.WaitTimeThenCallFunction(2f, SetCollectableInactive);
    }
    
    private void OnTriggerExit(Collider other) {
    }

    public void PlayDestroyParticle() {
        destroyParticle.Play();
    }

    public void SetCollectableInactive() {
        collectable.SetActive(false);
    }
}
