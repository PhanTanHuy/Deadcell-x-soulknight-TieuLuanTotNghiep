using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class BulletLaze : ProjectileMovement
{
    [Header("Laser")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float damageInterval = 0.2f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayer;

    private LineRenderer lineRenderer;
    private Camera mainCamera;

    private float nextDamageTime;

    protected override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        lineRenderer.positionCount = 2;
    }

    private void OnEnable()
    {
        lineRenderer.positionCount = 2;
        nextDamageTime = 0f;
    }

    private void OnDisable()
    {
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        if (shootPos == null || mainCamera == null)
            return;

        Vector3 startPosition = shootPos.position;

        Vector3 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        mouseScreenPosition.z =
            Mathf.Abs(mainCamera.transform.position.z - startPosition.z);

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 direction =
            mouseWorldPosition - startPosition;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(
            startPosition,
            direction,
            maxDistance,
            collisionLayer
        );

        Vector3 endPosition;

        if (hit.collider != null)
        {
            endPosition = hit.point;

            // Chỉ gây damage mỗi 0.2 giây
            if (Time.time >= nextDamageTime)
            {
                HitBox hitBox = hit.collider.GetComponent<HitBox>();

                if (hitBox != null)
                {
                    hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
        else
        {
            endPosition =
                startPosition +
                (Vector3)(direction * maxDistance);
        }

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}