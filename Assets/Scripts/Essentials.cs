using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essentials : MonoBehaviour{

    private Animator animator;
    private GameObject fishingCanvas;
    public bool waitToShowFishingCanvas;

    // Start is called before the first frame update
    void Start() {
        animator = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>().GetComponent<Animator>();
        fishingCanvas = transform.Find("Canvas").Find("Fishing Popup").gameObject;

        /*
        //turn on all canvas children
        //don't turn on the vehicle inventory, to be removed later
        foreach (Transform t in transform.Find("Canvas")) {
            t.gameObject.Set
            if (t.name == "Vehicle Inventory")
                t.gameObject.SetActive(false);
            if (t.name == "Fishing Popup")
                t.gameObject.SetActive(false);

        }
        */
    }

    // Update is called once per frame
    void Update() {
        WaitToShowCanvas();
    }

    private void WaitToShowCanvas() {
        if (waitToShowFishingCanvas) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fishing - Idle")) {
                fishingCanvas.GetComponent<FishingPopup>().BeginFishing();
                waitToShowFishingCanvas = false;
            }
        }
    }
}
