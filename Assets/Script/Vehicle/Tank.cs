using UnityEngine;

public class Tank : Vehicle
{
    [Header("Tank")]
    [SerializeField] private Transform sprite;

    [SerializeField] private float rotationSpeed = 180f;

    protected override void Move()
    {
        Vector2 direction = moveInput;

        // Không cho đi chéo nhanh hơn.
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        // Không có input thì không di chuyển / không xoay.
        if (direction.sqrMagnitude <= 0.001f)
            return;

        // Di chuyển tank.
        Vector2 movement =
            direction * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + movement);

        // Xoay thân tank theo hướng di chuyển.
        RotateTank(direction);
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
        interactor.GetComponentInChildren<WeaponController>().enabled = false;
        GetComponentInChildren<WeaponController>().enabled = true;
    }
    protected override void ExitVehicle()
    {
        currentDriver.GetComponentInChildren<WeaponController>().enabled = true;
        base.ExitVehicle();
        GetComponentInChildren<WeaponController>().enabled = false;
    }
}