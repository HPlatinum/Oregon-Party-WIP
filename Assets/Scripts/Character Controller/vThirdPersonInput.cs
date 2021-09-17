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

        //[HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;
        private Joystick joystick;

        //private InteractionManager interactScript;

        //thirdpersoncontroller variables
        //public Transform essentialsCanvas;

        #endregion

        protected virtual void Start()
        {
            joystick = FindObjectOfType<FixedJoystick>();
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            StaticVariables.controller.UpdateMotor();               // updates the ThirdPersonMotor methods
            StaticVariables.controller.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            StaticVariables.controller.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            
            InputHandle();                  // update the input methods
            StaticVariables.controller.UpdateAnimator();            // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            StaticVariables.controller.ControlAnimatorRootMotion(); // handle root motion animations 
        }



        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            //cc = GetComponent<vThirdPersonController>();

            if (StaticVariables.controller != null)
                StaticVariables.controller.Init();
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
            StaticVariables.controller.input.x = joystickInput.x;
            StaticVariables.controller.input.z = joystickInput.y;
            if (joystickInput == Vector2.zero) {
                StaticVariables.controller.input.x = Input.GetAxis(horizontalInput);
                StaticVariables.controller.input.z = Input.GetAxis(verticallInput);
            }
;
            


        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    StaticVariables.controller.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                StaticVariables.controller.UpdateMoveDirection(cameraMain.transform);
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
                StaticVariables.controller.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                StaticVariables.controller.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                StaticVariables.controller.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return StaticVariables.controller.isGrounded && StaticVariables.controller.GroundAngle() < StaticVariables.controller.slopeLimit && !StaticVariables.controller.isJumping && !StaticVariables.controller.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                StaticVariables.controller.Jump();
        }

        /// <summary>
        /// Input to trigger interaction
        /// /// </summary>
        protected virtual void InteractInput() {
            if (Input.GetKeyDown(interactInput))
                StaticVariables.interactScript.StartInteractionWithCurrentInteractable();
        }

        #endregion       
    }
}