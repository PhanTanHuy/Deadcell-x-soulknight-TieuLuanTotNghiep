using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : WeaponController
{
    [Header("Player")]

    private InputSystem_Actions inputActions;
    private Camera mainCamera;

    private bool isAttacking;

    protected override void Awake()
    {
        base.Awake();

        inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (inputActions == null)
            return;

        inputActions.Enable();

        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Attack.canceled += OnAttackCanceled;

        isAttacking = false;
    }

    protected override void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Attack.performed -= OnAttackPerformed;
            inputActions.Player.Attack.canceled -= OnAttackCanceled;

            inputActions.Disable();
        }

        base.OnDisable();

        isAttacking = false;
    }

    protected override void Update()
    {
        RotateWeaponsTowardsMouse();
        RotateHolderToWeapon();

        HandleFire();
    }

    //========================================================
    // ROTATION
    //========================================================

    private void RotateWeaponsTowardsMouse()
    {
        if (mainCamera == null)
            return;

        Vector3 mousePosition = Mouse.current.position.ReadValue();

        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        AimAtPosition(worldMousePosition);
    }

    

    //========================================================
    // FIRE
    //========================================================

    protected override void HandleFire()
    {
        fireTimer += Time.deltaTime;

        if (!isAttacking)
            return;

        if (fireTimer < fireInterval)
            return;

        fireTimer = 0f;

        if (!fireHold)
            Fire(directionToTarget);
    }

    //========================================================
    // INPUT
    //========================================================

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        isAttacking = true;

        if (fireHold && projectileAttack != null && !projectileAttack.IsProjectileActive)
            ActiveAllProjectileHolds();
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;

        if (fireHold && projectileAttack != null && projectileAttack.IsProjectileActive)
            DeactiveAllProjectileHolds();
    }
}