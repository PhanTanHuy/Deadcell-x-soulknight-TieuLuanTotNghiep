using UnityEngine;

public class MeleAttackCar : Vehicle
{
    [Header("Tank")]
    [SerializeField] private Transform sprite;

    [SerializeField] private float rotationSpeed = 180f;

    [Header("Movement")]
    [SerializeField] private float directionLerpSpeed = 5f;

    private Vector2 carDirection;

    protected override void Move()
    {
        Vector2 inputDirection = moveInput;

        // Không cho đi chéo nhanh hơn.
        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        // Lerp hướng tank theo input.
        carDirection = Vector2.Lerp(
            carDirection,
            inputDirection,
            directionLerpSpeed * Time.fixedDeltaTime
        );

        // Khi gần bằng 0 thì dừng hẳn.
        if (carDirection.sqrMagnitude <= 0.001f)
        {
            carDirection = Vector2.zero;
            return;
        }

        // Di chuyển theo carDirection.
        Vector2 movement =
            carDirection * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(rb.position + movement);
        Debug.Log(movement);
        // Xoay theo hướng đang di chuyển.
        RotateTank(carDirection.normalized);
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

        //foreach (var weaponController in GetComponentsInChildren<WeaponController>())
        //{
        //    weaponController.enabled = true;
        //}

        //interactor
        //    .GetComponentInChildren<WeaponController>()
        //    .enabled = false;
    }

    public override void ExitVehicle()
    {
        //if (currentDriver != null) currentDriver.GetComponentInChildren<WeaponController>().enabled = true;

        base.ExitVehicle();

        //foreach (var weaponController in GetComponentsInChildren<WeaponController>())
        //{
        //    weaponController.enabled = false;
        //}
    }
}
