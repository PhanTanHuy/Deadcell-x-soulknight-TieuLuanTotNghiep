using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Vehicle : MonoBehaviour, IInteract
{
    [Header("Vehicle")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected Transform driverSeat;
    [SerializeField] protected Transform exitPoint;
    protected Rigidbody2D rb;

    private Rigidbody2D driverRb;
    private InputSystem_Actions inputActions;

    protected Vector2 moveInput;
    protected Transform currentDriver;
    private HitBox hitBox;
    private BoxInteract boxinteract;
    private bool isDriving;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxinteract = GetComponentInChildren<BoxInteract>();
        inputActions = new InputSystem_Actions();
        hitBox = GetComponent<HitBox>();
        hitBox.enabled = false;
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
        if (hitBox.CanNotLifeAnymore()) return;
        if (interactor == null)
            return;
        interactor.SetParent(transform);

        PlayerController player =
            interactor.GetComponent<PlayerController>();

        if (player == null)
            return;

        driverRb = interactor.GetComponent<Rigidbody2D>();

        if (driverRb == null)
            return;

        currentDriver = interactor;
        isDriving = true;

        // Tắt PlayerController.
        player.enabled = false;
        player.GetComponent<HitBox>().enabled = false;
        player.GetComponent<Collider2D>().enabled = false;
        hitBox.enabled = true;

        // Tắt physics của Player.
        driverRb.bodyType = RigidbodyType2D.Kinematic;
        // Cho Player làm con của xe.

        // Đặt Player vào ghế.
        if (driverSeat != null)
        {
            currentDriver.position = driverSeat.position;
        }
        else
        {
            currentDriver.localPosition = Vector3.zero;
        }

        Debug.Log("Player entered vehicle: " + gameObject.name);
        rb.linearVelocity = Vector3.zero;
    }

    public virtual void ExitVehicle()
    {
        if (currentDriver == null)
            return;
        PlayerController player =
            currentDriver.GetComponent<PlayerController>();

        // Trước tiên lấy vị trí xuống xe.
        Vector3 exitPosition;

        if (exitPoint != null)
        {
            exitPosition = exitPoint.position;
        }
        else
        {
            exitPosition = transform.position + Vector3.right;
        }

        // Bỏ Player khỏi xe.
        currentDriver.SetParent(null);

        // Đặt vị trí xuống xe.
        currentDriver.position = exitPosition;

        // Bật lại physics.
        if (driverRb != null)
        {
            driverRb.bodyType = RigidbodyType2D.Dynamic;
        }

        // Bật lại PlayerController.
        if (player != null)
        {
            player.enabled = true;
            player.GetComponent<HitBox>().enabled = true;
            player.GetComponent<Collider2D>().enabled = true;
        }
        hitBox.enabled = false;

        currentDriver = null;
        driverRb = null;

        isDriving = false;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;

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
    public void DisableInteract()
    {
        boxinteract.enabled = false;
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