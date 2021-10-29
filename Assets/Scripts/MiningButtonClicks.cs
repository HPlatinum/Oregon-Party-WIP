using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningButtonClicks : MonoBehaviour
{
    GameObject interactButton;
    public ParticleSystem particle;
    public void ReturnButtonMineableLayer(GameObject go) {
        interactButton = go;
        StaticVariables.WaitTimeThenCallFunction(.6f, DestroyButton);
    }
    public void PlayParticleEffect() {
        Debug.Log("Playing...");
        particle.Play();
    }

    public void DestroyButton() {
        StaticVariables.miningHandler.DestroyMineableLayer(interactButton);
    }
    
}
