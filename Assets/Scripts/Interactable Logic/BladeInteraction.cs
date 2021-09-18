using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BladeInteraction : MonoBehaviour
{
    [SerializeField] public ParticleSystem particleEffect;
    [SerializeField] public GameObject blade;

    private void OnTriggerEnter(Collider obj) {
        StaticVariables.interactScript.GetInteractionHandlerForClosestInteractable().ProcessBladeHittingObject(particleEffect);
        DisableBlade();
    }

     private void OnTriggerExit(Collider obj) {
    }

    public void EnableBlade() {
        blade.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void DisableBlade() {
        blade.GetComponent<CapsuleCollider>().enabled = false;
    }


}
