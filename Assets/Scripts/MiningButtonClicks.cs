using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningButtonClicks : MonoBehaviour
{
    GameObject interactButton;
    public GameObject mineArea;
    public ParticleSystem particle;

    public void Start() {
    }
    public void ReturnButtonMineableLayer(GameObject go) {
        interactButton = go;
        DestroyButton();
    }
    public void PlayParticleEffect() {
        Debug.Log("Playing..." + particle);
        particle.Play();
    }

    public void SetButtonInactive() {
        mineArea.SetActive(false);
    }

    public void DestroyButton() {
        StaticVariables.miningHandler.DestroyMineableLayer(interactButton);
    }
    
}
