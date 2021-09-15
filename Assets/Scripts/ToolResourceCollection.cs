using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToolResourceCollection : MonoBehaviour
{
    [SerializeField] public ParticleSystem particleEffect;
    [SerializeField] public GameObject blade;
    // Start is called before the first frame update
    void Start()
    {
        StaticVariables.toolResourceCollection = blade.GetComponent<ToolResourceCollection>();
        blade.GetComponent<CapsuleCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider obj) {
        particleEffect.Play();
        DisableBlade();
        CountInteractSubjectHits();
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
        StaticVariables.interactScript.GetClosestInteractable().hitsCurrentlyAppliedToResource += 1;
        if(StaticVariables.interactScript.GetClosestInteractable().hitsCurrentlyAppliedToResource > StaticVariables.interactScript.GetClosestInteractable().hitsRequiredToGatherResource) {
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();
            StaticVariables.interactScript.DestroyCurrentInteractable();
        }
    }
}
