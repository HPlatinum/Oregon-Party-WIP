using UnityEngine;

public class BeaverAnimator : BeaverMotor
{
    public virtual void UpdateAnimator() {
        if (animator == null || !animator.enabled) return;

        animator.SetBool(BeaverAnimatorParameters.IsSquirming, isSquirming);
        animator.SetBool(BeaverAnimatorParameters.IsFarting, isFarting);
        animator.SetBool(BeaverAnimatorParameters.IsWalking, isWalking);
        // animator.SetFloat(BeaverAnimatorParameters.Speed)
    }
    // public virtual void SetAnimatorMoveSpeed(vMovementSpeed speed) {
    //     Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
    //     verticalSpeed = relativeInput.z;
    //     horizontalSpeed = relativeInput.x;

    //     var newInput = new Vector2(verticalSpeed, horizontalSpeed);

    //     if (speed.walkByDefault)
    //         inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
    //     else
    //         inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
    // }
    public static partial class BeaverAnimatorParameters
    {
        public static int IsSquirming = Animator.StringToHash("IsSquirming");
        public static int IsFarting = Animator.StringToHash("IsFarting");
        public static int IsWalking = Animator.StringToHash("IsWalking");
        public static int Speed = Animator.StringToHash("Speed");
    }
}
