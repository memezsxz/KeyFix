using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SpeedersInteraction))]
[RequireComponent(typeof(ScalerInteraction))]
[RequireComponent(typeof(Animator))]
// [RequireComponent(typeof(PlayerBindingManage))]
public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    // TODO Maryam: should add require Animator to the script 
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private static readonly int Horizontal = Animator.StringToHash("horizontal");
    private static readonly int isFalling = Animator.StringToHash("isFalling");
    // private PlayerBindingManage bindingManage;
    private SpeedersInteraction mover;
    // private ScalerInteraction scaler;
    private bool canMove = true;
    private Vector3 externalForce = Vector3.zero; // to receive wind push

    [SerializeField]
    private CharacterType _charecterType = CharacterType.Robot; // the type of character that is holding the script

    public CharacterType CharecterType => _charecterType;

    #region Testing Other Input system

    // public GameManagement GameManagement;
    // public GameObject LevelSuccessParticles;
    // public ParticleSystem LevelFailureParticles;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 5f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private InputAction moveAction, jumpAction;
    public Animator anim;
    public float allowPlayerRotation = 1f;
    [Range(0, 1f)] public float StartAnimTime = 0.3f;
    [Range(0, 1f)] public float StopAnimTime = 0.15f;
    private float lastYPosition;


    private void Start()
    {
        // var d = SaveManager.Instance.SaveData;
        // LoadData(ref d);
        lastYPosition = transform.position.y;
        controller = gameObject.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();
        // anim.applyRootMotion = false;
        // bindingManage = gameObject.GetComponent<PlayerBindingManage>();
        var actions = gameObject.GetComponent<PlayerInput>().actions;
        if (_charecterType == CharacterType.Robot)
        {
            actions.FindActionMap("Robot").Enable();
            actions.FindActionMap("Robota").Disable();
        }
        else
        {
            actions.FindActionMap("Robot").Disable();
            actions.FindActionMap("Robota").Enable();
        }

        moveAction = actions.FindAction("Move");
        jumpAction = actions.FindAction("Jump");
        // print("found " + moveAction.bindings[0].name + " to " + gameObject.name);
        mover = GetComponent<SpeedersInteraction>();
        // scaler = GetComponent<ScalerInteraction>();
    }

    void Update()
    {
        lastYPosition = transform.position.y;

        // Check if grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (!canMove)
        {
            // Apply gravity only, no input
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            anim.SetFloat("Blend", 0f, StopAnimTime, Time.deltaTime);
            return;
        }

        // Read input from new Input System
        if (mover.IsBeingPushed) return; // Block input during movement

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float inputX = moveValue.x;
        float inputZ = moveValue.y;


        // Move vector and motion

        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y);

// Get player intended movement
        Vector3 moveInput = new Vector3(moveValue.x, 0.0f, moveValue.y) * playerSpeed;

// Compete wind vs input
        Vector3 finalMove = moveInput + externalForce;

// If player moving against wind, reduce wind impact
        if (moveInput != Vector3.zero && Vector3.Dot(moveInput.normalized, externalForce.normalized) < 0)
        {
            // If moving against wind, reduce wind effect
            finalMove += externalForce * 0.5f; // Wind is only half as effective
        }

// Move the player
        controller.Move(finalMove * Time.deltaTime);

// Reset wind for next frame
        externalForce = Vector3.zero;

        // Rotate to direction of movement
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;

            // Quaternion currentRotation = transform.rotation;
            // Quaternion targetRotation = Quaternion.LookRotation(move);
            // transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, allowPlayerRotation);
        }


        // Jumping logic
        // if (jumpAction.IsPressed() && groundedPlayer)
        if (jumpAction.IsPressed() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            anim.SetBool(isFalling, false); // Reset fall state if jumping
        }
        else
        {
            if (!groundedPlayer && playerVelocity.y < -1f)
            {
                anim.SetBool(isFalling, true); // Falling down
            }
            else if (groundedPlayer)
            {
                anim.SetBool(isFalling, false); // Landed or idle
            }
        }


        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // animation
        float speed = new Vector2(inputX, inputZ).sqrMagnitude;

        if (speed > 0.1f)
        {
            anim.SetFloat("Blend", speed, StartAnimTime, Time.deltaTime);
        }
        else if (speed < allowPlayerRotation)
        {
            anim.SetFloat("Blend", speed, StopAnimTime, Time.deltaTime);
        }

        // float currentY = transform.position.y;
        // float deltaY = currentY - lastYPosition;
        //
        // if (Mathf.Abs(deltaY) < 0.001f)
        // {
        //     anim.SetInteger(YMovement, 0); // Not moving vertically
        // }
        // else if (deltaY > 0f)
        // {
        // }
        // else
        // {
        //     anim.SetInteger(YMovement, -1); // Moving down (falling)
        // }
        //
        // lastYPosition = currentY;
    }

    public void ApplyExternalForce(Vector3 force)
    {
        externalForce += force;
    }

    #endregion

//
//     #region Testing Other Input system
//     public float Velocity = 5;
//     [Space]
//     private InputAction moveAction, jumpAction;
//
//     public float InputX;
//     public float InputZ;
//     public Vector3 desiredMoveDirection;
//     public bool blockRotationPlayer;
//     public float desiredRotationSpeed = 0.1f;
//     public Animator anim;
//     public float Speed;
//     public float allowPlayerRotation = 0.1f;
//     public Camera cam;
//     public CharacterController controller;
//     public bool isGrounded;
//
//     private Vector3 playerVelocity;
//     private bool groundedPlayer;
//     private float jumpHeight = 1.0f;
//
//     private float gravityValue = -9.81f;
//
//     [Header("Animation Smoothing")]
//     [Range(0, 1f)]
//     public float HorizontalAnimSmoothTime = 0.2f;
//     [Range(0, 1f)]
//     public float VerticalAnimTime = 0.2f;
//     [Range(0,1f)]
//     public float StartAnimTime = 0.3f;
//     [Range(0, 1f)]
//     public float StopAnimTime = 0.15f;
//
//     public float verticalVel;
//     private Vector3 moveVector;
//
// // Use this for initialization
// 	void Start () {
// 		anim = this.GetComponent<Animator> ();
// 		cam = Camera.main;
// 		controller = this.GetComponent<CharacterController> ();
// 		var actions = gameObject.GetComponent<PlayerInput>().actions;
// 		moveAction = actions.FindAction("Move");
// 		jumpAction = actions.FindAction("Jump");
// 	}
// 	
// 	// Update is called once per frame
// 	void Update () {
// 		InputMagnitude ();
//
//         isGrounded = controller.isGrounded;
//         if (isGrounded)
//         {
//             verticalVel -= 0;
//         }
//         else
//         {
//             verticalVel -= 1;
//         }
//         
//         // Jumping logic
//         if (jumpAction.IsPressed() && groundedPlayer)
//         {
//             playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
//         }
//         // Apply gravity
//         playerVelocity.y += gravityValue * Time.deltaTime;
//
//         moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
//         controller.Move(moveVector);
//         controller.Move(playerVelocity * Time.deltaTime);
//     }
//
//     void PlayerMoveAndRotation() {
// 	    Vector2 moveValue = moveAction.ReadValue<Vector2>();
// 	    InputX = moveValue.x;
// 	    InputZ = moveValue.y;
// 	    
// 		var camera = Camera.main;
// 		var forward = cam.transform.forward;
// 		var right = cam.transform.right;
// 		
// 		forward.y = 0f;
// 		right.y = 0f;
// 		
// 		forward.Normalize ();
// 		right.Normalize ();
// 		
// 		desiredMoveDirection = forward * InputZ + right * InputX;
//
// 		if (blockRotationPlayer == false) {
// 			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
//             controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
// 		}
// 	}
//
//     public void LookAt(Vector3 pos)
//     {
//         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
//     }
//
//     public void RotateToCamera(Transform t)
//     {
//         var camera = Camera.main;
//         var forward = cam.transform.forward;
//         var right = cam.transform.right;
//
//         desiredMoveDirection = forward;
//
//         t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
//     }
//
// 	void InputMagnitude() {
// 		//Calculate Input Vectors
// 		Vector2 moveValue = moveAction.ReadValue<Vector2>();
// 		InputX = moveValue.x;
// 		InputZ = moveValue.y;
// 		
// 		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
// 		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);
//
// 		//Calculate the Input Magnitude
// 		Speed = new Vector2(InputX, InputZ).sqrMagnitude;
//
//         //Physically move player
//
// 		if (Speed > allowPlayerRotation) {
// 			anim.SetFloat ("Blend", Speed, StartAnimTime, Time.deltaTime);
// 			PlayerMoveAndRotation ();
// 		} else if (Speed < allowPlayerRotation) {
// 			anim.SetFloat ("Blend", Speed, StopAnimTime, Time.deltaTime);
// 		}
// 	}
//
//     #endregion


    public void ToggleMovement(bool value)
    {
        canMove = value;
    }

    private void UpdatePlayerMovementBinding()
    {
        var moveAction = GetComponent<PlayerInput>().actions["Move"];

        var upBinding = moveAction.bindings
            .Select((binding, index) => new { binding, index })
            .FirstOrDefault(b => b.binding.name == "up" && b.binding.isPartOfComposite);

        if (upBinding != null)
        {
            moveAction.ApplyBindingOverride(upBinding.index, new InputBinding { overridePath = " " });
            Debug.Log("Overrode 'W' key (up) with none");
        }
        else
        {
            Debug.LogWarning("Couldn't find the 'up' binding to override");
        }
    }


    public void SaveData(ref SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(_charecterType);
        psd.Position = transform.position;
        psd.Yaw = transform.rotation.eulerAngles.y;
    }

    public void LoadData(ref SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(_charecterType);
        var controller = GetComponent<CharacterController>();

        controller.enabled = false; // Disable to avoid conflict
        transform.position = psd.Position;
        transform.rotation = Quaternion.Euler(0, psd.Yaw, 0);
        controller.enabled = true; // Re-enable
    }
}