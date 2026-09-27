using UnityEngine;

public class Tank : Vehicle
{
    [Header("Tank")]
    [SerializeField] private Transform sprite;

    [SerializeField] private float rotationSpeed = 180f;

    [Header("Movement")]
    [SerializeField] private float directionLerpSpeed = 5f;

    private Vector2 tankDirection;

    protected override void Move()
    {
        Vector2 inputDirection = moveInput;

        // Không cho đi chéo nhanh hơn.
        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        // Lerp hướng tank theo input.
        tankDirection = Vector2.Lerp(
            tankDirection,
            inputDirection,
            directionLerpSpeed * Time.fixedDeltaTime
        );

        // Khi gần bằng 0 thì dừng hẳn.
        if (tankDirection.sqrMagnitude <= 0.001f)
        {
            tankDirection = Vector2.zero;
            return;
        }

        // Di chuyển theo tankDirection.
        Vector2 movement =
            tankDirection * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + movement);
        Debug.Log(movement);
        // Xoay theo hướng đang di chuyển.
        RotateTank(tankDirection.normalized);
    }

    private void RotateTank(Vector2 direction)
    {
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);

        sprite.rotation = Quaternion.RotateTowards(
            sprite.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    protected override void EnterVehicle(Transform interactor)
    {
        base.EnterVehicle(interactor);

        foreach (var weaponController in GetComponentsInChildren<WeaponController>())
        {
            weaponController.enabled = true;
        }

        interactor
            .GetComponentInChildren<WeaponController>()
            .enabled = false;
    }

    public override void ExitVehicle()
    {
        if (currentDriver != null) currentDriver.GetComponentInChildren<WeaponController>().enabled = true;

        base.ExitVehicle();

        foreach (var weaponController in GetComponentsInChildren<WeaponController>())
        {
            weaponController.enabled = false;
        }
    }
}