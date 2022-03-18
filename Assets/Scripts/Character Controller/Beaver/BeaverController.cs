using UnityEngine;

public class BeaverController : BeaverAnimator
{
    private void Start() {
        Debug.Log("here....");
        StaticVariables.WaitTimeThenCallFunction(5f, WaitFiveSecondsThenFart);
        isWalking = true;
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

    private void GetAnimator() {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        Debug.Log("Farting.. " + isFarting);
    }
    private void WaitFiveSecondsThenFart() {
        Walk();
        Fart();
        Debug.Log("Farting.." + isFarting);
    }
}
