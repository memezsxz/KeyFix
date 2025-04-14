using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    // TODO Maryam: should add require Animator to the script 
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private static readonly int Horizontal = Animator.StringToHash("horizontal");

    private CharacterType _charecterType = CharacterType.Robot; // the type of character that is holding the script


    #region Testing Other Input system

    // public GameManagement GameManagement;
    // public GameObject LevelSuccessParticles;
    // public ParticleSystem LevelFailureParticles;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 8.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private InputAction moveAction, jumpAction;
    public Animator anim;
    public float allowPlayerRotation = 0.1f;
    [Range(0, 1f)] public float StartAnimTime = 0.3f;
    [Range(0, 1f)] public float StopAnimTime = 0.15f;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        anim = this.GetComponent<Animator>();

        var actions = gameObject.GetComponent<PlayerInput>().actions;
        moveAction = actions.FindAction("Move");
        jumpAction = actions.FindAction("Jump");
    }

    void Update()
    {
        // Check if grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        
        // Read input from new Input System

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float inputX = moveValue.x;
        float inputZ = moveValue.y;

        // Move vector and motion

        Vector3 move = new Vector3(moveValue.x, 0.0f, moveValue.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Rotate to direction of movement
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Jumping logic
        if (jumpAction.IsPressed() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
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
    }

    #endregion

    private void prnt()
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

    public void SaveData(SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(CharacterType.Robot);
        psd.Position = transform.position;
        psd.Yaw = transform.rotation.eulerAngles.y;
        // psd.HitsRemaining = hits;
        // psd.LivesRemaining = lives;
        // Debug.Log($"save data {psd.HitsRemaining} & {psd.LivesRemaining} & {data.Meta.SaveName}");
    }

    public void LoadData(SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(CharacterType.Robot);
        transform.position = psd.Position;
        var newRot = Quaternion.Euler(0, psd.Yaw, 0);
        transform.rotation = newRot;
        // hits = psd.HitsRemaining;
        // lives = psd.LivesRemaining;
        // Debug.Log($"load obj {psd.HitsRemaining}");
    }
}