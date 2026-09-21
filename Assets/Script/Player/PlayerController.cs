using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform sprite;

    private InputSystem_Actions inputActions;
    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Animator animator;
    private float timeForBuff = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Disable();
    }
    public void SetTimeForBuff(float t)
    {
        timeForBuff = t;
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        animator.Play("Run");
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        animator.Play("Idle");
    }

    private void FixedUpdate()
    {
        if (timeForBuff > 0f)
        {
            timeForBuff -= Time.fixedDeltaTime;
            return;
        }
        Move();
    }

    private void Move()
    {
        Vector2 direction = moveInput;

        // Prevent diagonal movement from being faster.
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        Vector2 movement =
            direction * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + movement);
    }
}