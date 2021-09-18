using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InteractionHandler : MonoBehaviour {

    public virtual void ProcessInteractAction() {
        //run when the player pushes the interact button in range of an interactable
        print("interact button hit, no interaction to execute");
    }

    /*
    public virtual void BeginInteractAnimation() {
        //run when the interact animation starts

    }
    */

    public virtual void ProcessInteractAnimationEnding() {
        //run when the interact animation ends - when the animator is no longer in state tagged "Interact"
        print("interact animation ended, no response to execute");
    }

    public virtual bool CanPlayerInteractWithObject(Interactable interactable) {
        return false;
    }

    public virtual void ProcessBladeHittingObject(ParticleSystem particleEffect) {
        print("Hit uninteractable object with blade");
    }

    
}
