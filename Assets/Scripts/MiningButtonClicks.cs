using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningButtonClicks : MonoBehaviour
{
    GameObject interactButton;
    // Start is called before the first frame update
    public void ReturnButtonMineableLayer(GameObject go) {
        interactButton = go;
        StaticVariables.WaitTimeThenCallFunction(.6f, DestroyButton);
    }
    public void PlayParticleEffect(ParticleSystem particle) {
        particle.Play();
    }

    public void DestroyButton() {
        StaticVariables.miningHandler.DestroyMineableLayer(interactButton);
    }
    
}
