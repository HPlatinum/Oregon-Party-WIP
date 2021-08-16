using UnityEngine;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour
    {
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.J;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;
        public KeyCode interactInput = KeyCode.Space;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;
        private Joystick joystick;

        private Interact interactScript;

        #endregion

        protected virtual void Start()
        {
            joystick = FindObjectOfType<FixedJoystick>();
            InitilizeController();
            InitializeTpCamera();
            interactScript = gameObject.transform.Find("InteractCollider").GetComponent<Interact>();
            cc.interactScript = interactScript;
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            CheckInteractState();
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        public virtual void CheckInteractState() {

            //if the interact animation has not been started
            if (cc.isInteracting) {
                if (!cc.interactAnimationStarted) {
                    if (cc.animator.GetCurrentAnimatorStateInfo(0).IsTag("Interact")) {
                        
                        cc.interactAnimationStarted = true;

                        //add any code you want run when the animation starts here
                        interactScript.StartInteract();
                    }
                }
                else {
                    if (!cc.animator.GetCurrentAnimatorStateInfo(0).IsTag("Interact")) {
                        
                        cc.interactAnimationStarted = false;
                        cc.isInteracting = false;
                        cc.lockMovement = false;
                        cc.lockRotation = false;

                        //add any code you want run when the animation ends here
                        interactScript.EndInteract();
                    }
                }
            }

        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle() {

            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
            InteractInput();
        }

        public virtual void MoveInput()
        {

            Vector2 joystickInput = joystick.Direction;
            cc.input.x = joystickInput.x;
            cc.input.z = joystickInput.y;
            //cc.input.x = Input.GetAxis(horizontalInput);
            //cc.input.z = Input.GetAxis(verticallInput);
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                cc.Jump();
        }

        /// <summary>
        /// Input to trigger interaction
        /// /// </summary>
        protected virtual void InteractInput() {
            if (Input.GetKeyDown(interactInput))
                cc.Interact();
        }

        #endregion       
    }
}