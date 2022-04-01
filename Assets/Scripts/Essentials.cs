using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Essentials : MonoBehaviour{

    // Start is called before the first frame update
    void Start() {
        Transform canvas = transform.Find("Canvas");

        //create the player object
        GetComponent<PlayerModelSetup>().CreatePlayerModelInstanceInScene();
        //get the camera to follow the player
        GetComponent<PlayerModelSetup>().SetCameraToFollowPlayer();

        //misc stuff
        StaticVariables.essentials = this;
        StaticVariables.playerAnimator = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>().GetComponent<Animator>();
        StaticVariables.controller = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
        StaticVariables.interactScript = FindObjectOfType<InteractionManager>();
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.tweenDummy = transform.Find("Empty Tween Dummy - For Delaying Function Calls");

        //UI elements        
        StaticVariables.itemDetails = canvas.Find("Item Details").GetComponent<ItemDetails>();
        StaticVariables.mainUI = canvas.Find("Main UI").GetComponent<MainUI>();
        StaticVariables.interactButtonText = StaticVariables.mainUI.transform.Find("Interact").Find("Text").GetComponent<Text>();
        StaticVariables.commonUI = canvas.Find("Common Interface Elements").GetComponent<CommonUI>();

        //interaction handlers
        StaticVariables.cookingHandler = canvas.Find("Cooking Interface").GetComponent<CookingHandler>();
        StaticVariables.fishingHandler = canvas.Find("Fishing Interface").GetComponent<FishingHandler>();
        StaticVariables.pickupHandler = canvas.Find("Pickup Interface").GetComponent<PickupHandler>();
        StaticVariables.woodcuttingHandler = canvas.Find("Woodcutting Interface").GetComponent<WoodcuttingHandler>();
        StaticVariables.sharpeningHandler = canvas.Find("Woodcutting Interface").GetComponent<SharpeningHandler>();
        StaticVariables.depositHandler = canvas.Find("Woodcutting Interface").GetComponent<DepositHandler>();
        StaticVariables.miningHandler = canvas.Find("Mining Interface").GetComponent<MiningHandler>();
        StaticVariables.forgeHandler = canvas.Find("Forge Interface").GetComponent<ForgeHandler>();

        //Scene Handler
        StaticVariables.sceneHandler = transform.Find("Scene Handler").GetComponent<SceneHandler>();

        //Timer
        StaticVariables.timer = transform.Find("Timer").GetComponent<Timer>();

        //turn on all UI elements
        //they usually hide themselves on startup, after setting local variables
        foreach (Transform t in transform.Find("Canvas")) {
            t.gameObject.SetActive(true);
        }
    }

}