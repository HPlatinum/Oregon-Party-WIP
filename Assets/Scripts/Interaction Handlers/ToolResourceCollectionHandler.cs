using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToolResourceCollectionHandler : InteractionHandler
{
    [SerializeField] public ParticleSystem particleEffect;
    [SerializeField] public GameObject blade;
    // Start is called before the first frame update
    void Start()
    {
        StaticVariables.toolResourceCollection = blade.GetComponent<ToolResourceCollectionHandler>();
        blade.GetComponent<CapsuleCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider obj) {
        particleEffect.Play();
        DisableBlade();
        CountInteractSubjectHits();
        if(StaticVariables.interactScript.closestInteractable.hitsCurrentlyAppliedToResource == StaticVariables.interactScript.closestInteractable.hitsRequiredToGatherResource) {
            StaticVariables.interactScript.DestroyCurrentInteractable();
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();
        }
    }

     private void OnTriggerExit(Collider obj) {
    }

    public void EnableBlade() {
        blade.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void DisableBlade() {
        blade.GetComponent<CapsuleCollider>().enabled = false;
    }

    private void CountInteractSubjectHits() {
        StaticVariables.interactScript.closestInteractable.hitsCurrentlyAppliedToResource ++;
    }
}
