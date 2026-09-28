using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterMovement
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform sprite;

    private InputSystem_Actions inputActions;
    private Animator animator;

    private Vector2 moveInput;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Enable();

        moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Disable();

        moveInput = Vector2.zero;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (!IsMovementEnabled())
            return;

        moveInput = context.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 0f)
        {
            animator.Play("Run");
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        if (!IsMovementEnabled())
            return;

        moveInput = Vector2.zero;

        animator.Play("Idle");
    }

    protected override void Move()
    {
        Vector2 direction = moveInput;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        Vector2 movement = direction * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + movement);
    }

    public override void SetMovementEnabled(bool enabled)
    {
        base.SetMovementEnabled(enabled);

        if (!enabled)
        {
            moveInput = Vector2.zero;

            if (animator != null)
            {
                animator.Play("Idle");
            }
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}