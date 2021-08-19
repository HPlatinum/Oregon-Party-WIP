using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        public virtual void ControlAnimatorRootMotion()
        {
            if (!this.enabled) return;

            if (inputSmooth == Vector3.zero)
            {
                transform.position = animator.rootPosition;
                transform.rotation = animator.rootRotation;
            }

            if (useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlLocomotionType()
        {
            if (lockMovement) return;

            if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
            {
                SetControllerMoveSpeed(freeSpeed);
                SetAnimatorMoveSpeed(freeSpeed);
            }
            else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
            {
                isStrafing = true;
                SetControllerMoveSpeed(strafeSpeed);
                SetAnimatorMoveSpeed(strafeSpeed);
            }

            if (!useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlRotationType()
        {
            if (lockRotation) return;

            bool validInput = input != Vector3.zero || (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera);

            if (validInput)
            {
                // calculate input smooth
                inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

                Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
                RotateToDirection(dir);
            }
        }

        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
        {
            if (input.magnitude <= 0.01)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
                return;
            }

            if (referenceTransform && !rotateByWorld)
            {
                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.right;
                right.y = 0;
                //get the forward direction relative to referenceTransform Right
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
            }
            else
            {
                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
            }
        }

        public virtual void Sprint(bool value)
        {
            var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded &&
                !(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

            if (value && sprintConditions)
            {
                if (input.sqrMagnitude > 0.1f)
                {
                    if (isGrounded && useContinuousSprint)
                    {
                        isSprinting = !isSprinting;
                    }
                    else if (!isSprinting)
                    {
                        isSprinting = true;
                    }
                }
                else if (!useContinuousSprint && isSprinting)
                {
                    isSprinting = false;
                }
            }
            else if (isSprinting)
            {
                isSprinting = false;
            }
        }

        public virtual void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;

            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
        }

        public virtual void Interact() {

            if (isInteracting) {
                InteractCeption();
                return;
            }

            //interact with the minigame
            if (interactScript.interactSubject == null) {
                return;
            }
            if (!interactScript.IsInteractAllowed()) {
                print("you cannot perform the " + interactScript.interactSubject.interactType.ToString() + " action");
                return;
            }
            if (interactScript.interactSubject.interactType == Interactable.InteractTypes.Pickup) {
                StartAnimation("Lifting", 0.2f);
                return;
            }
            
            if (interactScript.interactSubject.interactType == Interactable.InteractTypes.Fishing) {
                StartAnimation("Fishing - Cast", 0.2f);
                FindObjectOfType<Essentials>().waitToShowFishingCanvas = true;
                return;
            }

            if (interactScript.interactSubject.interactType == Interactable.InteractTypes.Chest) {
                //StartAnimation("Fishing", 0.2f);
                return;
            }

            
        }

        private void InteractCeption() {
            //checks to see if the player can do any interaction while already interacting during a minigame
            if (interactScript.interactSubject == null)
                return;
            //is the player fishing, and at the idle fishing animation?
            if (interactScript.interactSubject.interactType == Interactable.InteractTypes.Fishing) {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fishing - Idle")){
                    FindObjectOfType<FishingPopup>().ReelIn();
                    return;
                }
            }

        }


        private void StartAnimation(string animationName, float transitionDuration) {
            //plays the specified animation. transitions to the desired animation over the specified transitionDuration (in seconds)

            //set flags for the controller and input scripts
            isInteracting = true;
            interactAnimationStarted = false;

            //freeze player's position and rotation, and reduce momentum to zero
            lockMovement = true;
            lockRotation = true;
            HaltVelocity();
            
            //face toward the interacted object
            FaceTo(interactScript.interactSubject.gameObject, transitionDuration);
            //start the animation
            animator.CrossFadeInFixedTime(animationName, transitionDuration);


        }

        
        private void FaceTo(GameObject go, float duration) {
            //rotates the player towards the specified gameobject over duration time in seconds
            //only rotates in the x-z plane
            Vector3 pos = new Vector3(go.transform.position.x, transform.position.y, go.transform.position.z);

            var rotation = Quaternion.LookRotation(pos - gameObject.transform.position);
            StartCoroutine(RotatePlayer(duration, rotation));
        }

        private IEnumerator RotatePlayer(float lerpTime, Quaternion rotation) {
            //carries out player rotation
            float elapsedTime = 0f;

            while (elapsedTime <= lerpTime) {
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, rotation, elapsedTime / lerpTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        
    }

}