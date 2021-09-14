using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Woodcutting : MonoBehaviour
{
    [SerializeField] public ParticleSystem treeParticles;
    [SerializeField] public GameObject blade;
    // Start is called before the first frame update
    void Start()
    {
        StaticVariables.woodcutting = blade.GetComponent<Woodcutting>();
        blade.GetComponent<CapsuleCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider obj) {
        treeParticles.Play();
        DisableBlade();
        CountTreeHits();
    }

     private void OnTriggerExit(Collider obj) {
    }

    public void EnableBlade() {
        blade.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void DisableBlade() {
        blade.GetComponent<CapsuleCollider>().enabled = false;
    }

    private void CountTreeHits() {
        StaticVariables.interactScript.GetClosestInteractable().hitCount += 1;
        if(StaticVariables.interactScript.GetClosestInteractable().hitCount > StaticVariables.interactScript.GetClosestInteractable().requiredHits) {
            StaticVariables.interactScript.AddCurrentInteractableItemToInventory();
            StaticVariables.interactScript.DestroyCurrentInteractable();
        }
    }
}
