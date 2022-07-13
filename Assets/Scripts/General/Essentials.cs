using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Essentials : MonoBehaviour{

    // Start is called before the first frame update
    void Start() {
        Transform canvas = transform.Find("Canvas");

        //set up the player model and get the camera to follow it, then delete the player model setup object
        PlayerModelSetup pms = transform.Find("Player Model Setup").GetComponent<PlayerModelSetup>();
        int maxHealth = pms.GetMaxHealth();
        int maxSanity = pms.GetMaxSanity();
        pms.CreatePlayerModelInstanceInScene();
        pms.SetCameraToFollowPlayer();
        GameObject.Destroy(pms.gameObject);

        //misc stuff
        StaticVariables.essentials = this;
        StaticVariables.playerAnimator = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>().GetComponent<Animator>();
        StaticVariables.controller = FindObjectOfType<Invector.vCharacterController.vThirdPersonController>();
        StaticVariables.interactScript = FindObjectOfType<InteractionManager>();
        StaticVariables.playerAttackScript = FindObjectOfType<PlayerAttackManager>();
        StaticVariables.currentInteractionHandler = null;
        StaticVariables.tweenDummy = transform.Find("Empty Tween Dummy - For Delaying Function Calls");

        //UI elements        
        StaticVariables.itemDetails = canvas.Find("Item Details").GetComponent<ItemDetails>();
        StaticVariables.mainUI = canvas.Find("Main UI").GetComponent<MainUI>();
        StaticVariables.healthAndSanityTracker = StaticVariables.mainUI.GetComponent<HealthAndSanityTracker>();

        //interaction handlers
        StaticVariables.cookingHandler = canvas.Find("Cooking Interface").GetComponent<CookingHandler>();
        StaticVariables.fishingHandler = canvas.Find("Fishing Interface").GetComponent<FishingHandler>();
        StaticVariables.pickupHandler = canvas.Find("Pickup Interface").GetComponent<PickupHandler>();
        StaticVariables.woodcuttingHandler = canvas.Find("Woodcutting Interface").GetComponent<WoodcuttingHandler>();
        StaticVariables.sharpeningHandler = canvas.Find("Woodcutting Interface").GetComponent<SharpeningHandler>();
        StaticVariables.depositHandler = canvas.Find("Woodcutting Interface").GetComponent<DepositHandler>();
        StaticVariables.logHandler = canvas.Find("Woodcutting Interface").GetComponent<LogHandler>();
        StaticVariables.beaverHandler = canvas.Find("Woodcutting Interface").GetComponent<BeaverHandler>();
        StaticVariables.woodpileHandler = canvas.Find("Woodcutting Interface").GetComponent<WoodpileHandler>();
        StaticVariables.miningHandler = canvas.Find("Mining Interface").GetComponent<MiningHandler>();
        StaticVariables.forgeHandler = canvas.Find("Forge Interface").GetComponent<ForgeHandler>();
        StaticVariables.vehicleHandler = canvas.Find("Vehicle Interface").GetComponent<VehicleHandler>();
        StaticVariables.scrappingHandler = canvas.Find("Scrapping Interface").GetComponent<ScrappingHandler>();
        //StaticVariables.arrowHandler = canvas.Find("Arrows").GetComponent<AlertArrowHandler>();

        //Scene Handler
        StaticVariables.sceneHandler = transform.Find("Scene Handler").GetComponent<SceneHandler>();

        //Timer
        StaticVariables.timer = transform.Find("Timer").GetComponent<Timer>();

        //set the player's health and sanity
        StaticVariables.healthAndSanityTracker.SetStartingValues(maxHealth, maxSanity);

        //turn on all UI elements
        //they usually hide themselves on startup, after setting local variables
        foreach (Transform t in transform.Find("Canvas")) {
            t.gameObject.SetActive(true);
        }
    }

}