using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    // TODO Maryam: should add require Animator to the script 
    private static readonly int Vertical = Animator.StringToHash("vertical");
    private static readonly int Horizontal = Animator.StringToHash("horizontal");

    private CharacterType _charecterType = CharacterType.Robot; // the type of character that is holding the script


    // #region New input system
    //
    // private void Start()
    // {
    //     DebugController.Instance.AddDebugCommand(new DebugCommand("disable_w", "disable w key", "", prnt));
    // }
    //
    // private float moveSpeed = 5f;
    // private float rotationSpeed = 60f; // Degrees per second (smaller number -> slower)
    //
    // private Vector2 movementInput;
    //
    // public void OnMove(InputValue context)
    // {
    //     movementInput = context.Get<Vector2>();
    // }
    //
    // void Update()
    // {
    //     HandleRotation(); 
    //     HandleMovement();
    // }
    //
    // void HandleRotation()
    // {
    //     float horizontal = movementInput.x;
    //
    //     if (!(Mathf.Abs(horizontal) > 0.1f)) return;
    //     float rotationAmount = horizontal * rotationSpeed * Time.deltaTime;
    //     transform.Rotate(0f, rotationAmount, 0f);
    // }
    //
    // void HandleMovement()
    // {
    //     float vertical = movementInput.y;
    //
    //     if (!(vertical > 0.1f)) return;
    //     
    //     Vector3 move = transform.forward * moveSpeed * Time.deltaTime;
    //     transform.position += move;
    // }
    //
    // #endregion


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

    #region rotate then move

    private Rigidbody _rb;
    private Animator _animator;
    [SerializeField] private float speed = 5;
    [SerializeField] [Range(0f, 360f)] private float turnSpeed = 90;
    private Vector2 _moveInput;
    private Vector3 _input;

    // need to be unserialized, this is just for testing
    [SerializeField] private int hits = 10;
    [SerializeField] private int lives = 10;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        DebugController.Instance?.AddDebugCommand(new DebugCommand("disable_w", "disable w key", "", prnt));
    }


    private void OnMove(InputValue context)
    {
        _moveInput = context.Get<Vector2>();
        _animator.SetFloat(Vertical, _moveInput.x);
        _animator.SetFloat(Horizontal, _moveInput.y);
    }

    private void Update()
    {
        GatherInput();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GatherInput()
    {
        _input = new Vector3(_moveInput.x, 0, _moveInput.y);
    }

    // private void Look()
    // {
    //     if (_input == Vector3.zero) return;
    //
    //     var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    // }
    //
    // private void Move()
    // {
    //     _rb.MovePosition(transform.position +
    //                      transform.forward * (_input.normalized.magnitude * speed * Time.deltaTime));
    // }

    private void Move()
    {
        if (_input == Vector3.zero) return;

        var targetRot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);

        var angleDiff = Quaternion.Angle(transform.rotation, targetRot);

        if (angleDiff < 1f)
            _rb.MovePosition(transform.position +
                             transform.forward * (_input.normalized.magnitude * speed * Time.deltaTime));
    }
    // private void Look()
    // {
    //     if (_input == Vector3.zero) return;
    //
    //     var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
    //     transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    // }

    private void Look()
    {
        // If focusing, rotate to look at closest enemy
        if (CameraManager.Instance != null && CameraManager.Instance.IsFocusing)
        {
            var closestEnemy = GetClosestEnemy();
            if (closestEnemy != null)
            {
                var direction = closestEnemy.transform.position - transform.position;
                direction.y = 0f; // Flatten to prevent tilting

                if (direction.sqrMagnitude > 0.01f)
                {
                    var targetRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    transform.rotation =
                        Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
                }

                return;
            }
        }

        // Default movement-based look direction
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
    }

    private GameObject GetClosestEnemy()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        var minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            var dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    #endregion

    public void SaveData(SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(CharacterType.Robot);
        psd.Position = transform.position;
        psd.Yaw = transform.rotation.eulerAngles.y;
        psd.HitsRemaining = hits;
        psd.LivesRemaining = lives;
        // Debug.Log($"save data {psd.HitsRemaining} & {psd.LivesRemaining} & {data.Meta.SaveName}");
    }

    public void LoadData(SaveData data)
    {
        var psd = SaveManager.Instance.GetCharacterData(CharacterType.Robot);
        transform.position = psd.Position;
        var newRot = Quaternion.Euler(0, psd.Yaw, 0);
        transform.rotation = newRot;
        hits = psd.HitsRemaining;
        lives = psd.LivesRemaining;
        // Debug.Log($"load obj {psd.HitsRemaining}");
    }
}