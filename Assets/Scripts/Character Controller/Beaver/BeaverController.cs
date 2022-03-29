using UnityEngine;
using UnityEngine.AI;

public class BeaverController : BeaverAnimator
{
    bool stopMovement;
    bool hasWoodInHand;
    bool moveBeaver;
    bool interacting;
    public NavMeshAgent agent;
    Vector3 depositPosition;
    Vector3 startPosition;
    Vector3 velocity;
    GameObject depositBox;
    NPCInteractionManager beaverInteractionManager;
    private void Start() {
        Init();
    }

    private void Update() {
        UpdateAnimator();
    }
    public virtual void Fart()
    {
        isFarting = !isFarting;
    }

    public virtual void Squirm() {
        isSquirming = !isSquirming;
    }

    public virtual void Walk() {
        isWalking = !isWalking;
    }

    public void Init() {
        GetAnimator();
        GetRigidBody();
        GetBeaverGameObject();
        GetBeaverInteractionManager();
        GetDepositBoxGameObject();
        isWalking = true;
        isFarting = false;
        isSquirming = false;
        interacting = false;
        stopMovement = true;
        moveBeaver = false;
        hasWoodInHand = false;
        speed = 0;
        depositPosition = new Vector3(depositBox.transform.position.x, 0 ,depositBox.transform.position.z);
        startPosition = new Vector3(beaver.transform.position.x,0,beaver.transform.position.z);
    }

    private void GetAnimator() {
        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void GetRigidBody() {
        rigidBody = GetComponent<Rigidbody>();
    }
    private void GetBeaverGameObject() {
        beaver = gameObject;
    }
    private void GetDepositBoxGameObject() {
        depositBox = GameObject.Find("Storage");
    }


    private void GetBeaverInteractionManager() {
        beaverInteractionManager = beaver.transform.Find("Interact Collider").GetComponent<NPCInteractionManager>();
    }

    private void ReleaseTheBeaver() {
        MoveBeaver();
    }

    private void MoveBeaverToDestination(Vector3 destination) {
        agent.SetDestination(destination);
    }

    private void WoodInHand() {
        hasWoodInHand = !hasWoodInHand;
    }
    private void MoveBeaver() { 
        moveBeaver = !moveBeaver;
    }
    private void StopMovement() {
        stopMovement = !stopMovement;
    }
    private void Interacting() {
        interacting = !interacting;
    }
    private void FixedUpdate() {
        MoveToDepositBox();
        SetSpeed();
        ReleaseTheBeaver();
    }

    
    private void MoveToDepositBox() {
        if(moveBeaver && !interacting) {
            if(!hasWoodInHand) {
                if(stopMovement) {
                    stopMovement = false;
                }
                MoveBeaverToDestination(depositPosition);
            }
            if(hasWoodInHand) {
                if(stopMovement) {
                    stopMovement = false;
                }
                MoveBeaverToDestination(startPosition);
            }
        }
    }

    private void SetSpeed() {
        if(!stopMovement && speed < 1f) {
            speed += Time.deltaTime * .5f;
        }
    }

    private void FaceToDirection(Vector3 direction) {
        beaver.transform.LookAt(direction);
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.GetComponent<Interactable>() != null && collider.gameObject.name == "Storage" && beaverInteractionManager.itemInHand == null) {
            Interactable pickupInteractable = collider.gameObject.GetComponent<Interactable>();
            StopMovement();
            Interacting();
            WoodInHand();
            if(pickupInteractable.inventory.GetTotalItemQuantity(pickupInteractable.item) > 0) {
                pickupInteractable.inventory.RemoveItemFromInventory(pickupInteractable.item, 1);
                beaverInteractionManager.PutItemInNPCHand(pickupInteractable.item);
            }
            speed = 0;
            StaticVariables.WaitTimeThenCallFunction(3f, Interacting);
            StaticVariables.WaitTimeThenCallFunction(3f, StopMovement);
        }        
    }
}