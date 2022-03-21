using UnityEngine;

public class BeaverController : BeaverAnimator
{
    bool stopMovement;
    bool hasWoodInHand;
    bool moveBeaver;
    Vector3 depositPosition;
    Vector3 startPosition;
    Vector3 velocity;
    GameObject depositBox;
    BoxCollider beaverCollider;
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
        GetDepositBoxGameObject();
        GetBeaverCollider();
        isWalking = true;
        isFarting = false;
        isSquirming = false;
        stopMovement = true;
        moveBeaver = false;
        hasWoodInHand = true;
        moveBeaver = true;
        speed = 0;
        depositPosition = new Vector3(depositBox.transform.position.x, 0 ,depositBox.transform.position.z);
        startPosition = new Vector3(beaver.transform.position.x,0,beaver.transform.position.z);
        StaticVariables.WaitTimeThenCallFunction(5f, MoveBeaver);
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
        Debug.Log("Deposit box " + depositBox.name);
    }

    private void GetBeaverCollider() {
        beaverCollider = beaver.transform.Find("Beaver").GetComponent<BoxCollider>();
    }

    private void WoodInHand() {
        hasWoodInHand = false;
    }
    private void MoveBeaver() { 
        moveBeaver = true;
    }
    private void FixedUpdate() {
        MoveToDepositBox();
        SetSpeed();
    }

    
    private void MoveToDepositBox() {
        if(moveBeaver) {
            if(!hasWoodInHand) {
                stopMovement = false;
                beaver.transform.position = Vector3.SmoothDamp(transform.position, depositPosition, ref velocity, .5f, speed);
                FaceToDirection(depositPosition);
            }
            if(hasWoodInHand) {
                stopMovement = false;
                beaver.transform.position = Vector3.SmoothDamp(transform.position, startPosition, ref velocity, .5f, speed);
                FaceToDirection(startPosition);
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

    private void OnTriggerEnter(Collider other) {
        stopMovement = true;
        WoodInHand();
        speed = 0;
    }
}
