using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Vehicle : MonoBehaviour, IInteract
{
    [Header("Vehicle")]
    [SerializeField] protected float moveSpeed = 5f;

    protected Rigidbody2D rb;

    private InputSystem_Actions inputActions;

    protected Vector2 moveInput;

    protected Transform currentDriver;

    private bool isDriving;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        inputActions = new InputSystem_Actions();
    }

    protected virtual void OnEnable()
    {
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Enable();
    }

    protected virtual void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Disable();
    }

    public void Interact(Transform interactor)
    {
        if (isDriving)
        {
            ExitVehicle();
        }
        else
        {
            EnterVehicle(interactor);
        }
    }

    protected virtual void EnterVehicle(Transform interactor)
    {
        if (interactor == null)
            return;

        PlayerController player = interactor.GetComponent<PlayerController>();

        if (player == null)
            return;

        currentDriver = interactor;
        isDriving = true;

        // Tắt điều khiển Player.
        player.SetMovementEnabled(false);

        // Đưa Player vào vị trí xe.
        interactor.SetParent(this.transform);

        Debug.Log("Player entered vehicle: " + gameObject.name);
    }

    protected virtual void ExitVehicle()
    {
        if (currentDriver == null)
            return;

        PlayerController player =
            currentDriver.GetComponent<PlayerController>();

        if (player != null)
        {
            player.SetMovementEnabled(true);

            // Cho Player đứng cạnh xe khi xuống.
            currentDriver.SetParent(null);
            currentDriver.position =
                transform.position + Vector3.right;
        }

        currentDriver = null;
        isDriving = false;
        moveInput = Vector2.zero;

        Debug.Log("Player exited vehicle: " + gameObject.name);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (!isDriving)
            return;

        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        if (!isDriving)
            return;

        moveInput = Vector2.zero;
    }

    protected virtual void FixedUpdate()
    {
        if (!isDriving)
            return;

        Move();
    }

    protected virtual void Move()
    {
        Vector2 direction = moveInput;

        // Không cho đi chéo nhanh hơn.
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        Vector2 movement =
            direction * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + movement);
    }

    public bool IsDriving()
    {
        return isDriving;
    }

    public Transform GetDriver()
    {
        return currentDriver;
    }
}