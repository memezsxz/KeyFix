using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SpeedersInteraction))]
[RequireComponent(typeof(ScalerInteraction))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Fields and References

    // Animation parameter hashes
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private static readonly int Horizontal = Animator.StringToHash("horizontal");
    private static readonly int isFalling = Animator.StringToHash("isFalling");

    [SerializeField] private CharacterType _charecterType = CharacterType.Robot;

    private CharacterController controller;
    private SpeedersInteraction mover;

    private Vector3 playerVelocity;
    private Vector3 externalForce = Vector3.zero;

    private bool groundedPlayer;
    private bool canMove = true;

    private InputAction moveAction, jumpAction;

    [Header("Animation Settings")] public Animator anim;
    public float allowPlayerRotation = 1f;
    [Range(0, 1f)] public float StartAnimTime = 0.3f;
    [Range(0, 1f)] public float StopAnimTime = 0.15f;

    // Movement constants
    private readonly float playerSpeed = 5f;
    private readonly float jumpHeight = 1.0f;
    private readonly float gravityValue = -9.81f;

    private float lastYPosition;

    #endregion

    #region Properties

    /// <summary>
    /// Returns the character type (e.g., Robot or Robota) associated with this player.
    /// </summary>
    public CharacterType CharecterType => _charecterType;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        lastYPosition = transform.position.y;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        mover = GetComponent<SpeedersInteraction>();

        var actions = GetComponent<PlayerInput>().actions;

        // Enable correct input map based on character type
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
    }

    private void Update()
    {
        lastYPosition = transform.position.y;
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0f)
            playerVelocity.y = 0f;

        if (!canMove)
        {
            // Apply gravity only
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
            anim.SetFloat("Blend", 0f, StopAnimTime, Time.deltaTime);
            return;
        }

        if (mover.IsBeingPushed)
            return;

        HandleMovement();
        HandleJumping();
        HandleAnimation();
    }

    #endregion

    #region Movement Logic

    /// <summary>
    /// Handles directional movement and wind force resistance.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 move = new(moveValue.x, 0f, moveValue.y);
        Vector3 moveInput = move * playerSpeed;

        Vector3 finalMove = moveInput + externalForce;

        // Reduce wind impact if player is pushing against it
        if (moveInput != Vector3.zero && Vector3.Dot(moveInput.normalized, externalForce.normalized) < 0)
        {
            finalMove += externalForce * 0.5f;
        }

        controller.Move(finalMove * Time.deltaTime);
        externalForce = Vector3.zero;

        if (move != Vector3.zero)
            transform.forward = move;
    }

    /// <summary>
    /// Handles jumping and gravity.
    /// </summary>
    private void HandleJumping()
    {
        if (jumpAction.IsPressed() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            anim.SetBool(isFalling, false);
        }
        else
        {
            if (!groundedPlayer && playerVelocity.y < -1f)
                anim.SetBool(isFalling, true);
            else if (groundedPlayer)
                anim.SetBool(isFalling, false);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Updates blend animations based on movement speed.
    /// </summary>
    private void HandleAnimation()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float speed = moveValue.sqrMagnitude;

        if (speed > 0.1f)
            anim.SetFloat("Blend", speed, StartAnimTime, Time.deltaTime);
        else if (speed < allowPlayerRotation)
            anim.SetFloat("Blend", speed, StopAnimTime, Time.deltaTime);
    }

    #endregion

    #region Interaction

    /// <summary>
    /// Enables or disables player movement input.
    /// </summary>
    /// <param name="value">True to allow movement, false to lock movement.</param>
    public void ToggleMovement(bool value)
    {
        canMove = value;
    }

    /// <summary>
    /// Applies an external force to the player, such as wind.
    /// </summary>
    /// <param name="force">The vector force to apply.</param>
    public void ApplyExternalForce(Vector3 force)
    {
        externalForce += force;
    }

    #endregion

    #region Input Override (Testing)

    /// <summary>
    /// Overrides the "up" binding in the input system. Used for testing.
    /// </summary>
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

    #endregion

    #region Save/Load

    /// <summary>
    /// Saves the player's current position and Y-axis rotation.
    /// </summary>
    public void SaveData(ref SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(_charecterType);
        psd.Position = transform.position;
        psd.Yaw = transform.rotation.eulerAngles.y;
    }

    /// <summary>
    /// Loads and applies the player's position and rotation from saved data.
    /// </summary>
    public void LoadData(ref SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(_charecterType);
        var controller = GetComponent<CharacterController>();

        controller.enabled = false;
        transform.position = psd.Position;
        transform.rotation = Quaternion.Euler(0, psd.Yaw, 0);
        controller.enabled = true;
    }

    #endregion


    #region Other Movement

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

    #endregion
}