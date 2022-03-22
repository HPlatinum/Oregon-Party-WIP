using UnityEngine;

public class BeaverController : BeaverAnimator
{
    bool stopMovement;
    bool hasWoodInHand;
    bool moveBeaver;
    bool interacting;
    Vector3 depositPosition;
    Vector3 startPosition;
    Vector3 velocity;
    GameObject depositBox;
    BoxCollider beaverCollider;
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
        GetBeaverCollider();
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

    private void GetBeaverCollider() {
        // beaverCollider = beaver.transform.Find("Beaver").GetComponent<BoxCollider>();
    }

    private void GetBeaverInteractionManager() {
        beaverInteractionManager = beaver.transform.Find("Interact Collider").GetComponent<NPCInteractionManager>();
    }

    private void ReleaseTheBeaver() {
        MoveBeaver();
    }

    private void MoveBeaverToDestination(Vector3 destination) {
        beaver.transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, .5f, speed);
        FaceToDirection(destination);
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
                CheckClosestInteractable();
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

    private void CheckClosestInteractable() {
        if(beaverInteractionManager.closestInteractable != null || interacting == true) {
            if(beaverInteractionManager.closestInteractable.interactType == Interactable.InteractTypes.Deposit) {
                StopMovement();
                Interacting();
                WoodInHand();
                speed = 0;
                StaticVariables.WaitTimeThenCallFunction(3f, Interacting);
                StaticVariables.WaitTimeThenCallFunction(3f, StopMovement);
            }
        }
        
    }
}
