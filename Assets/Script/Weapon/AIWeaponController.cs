using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AIWeaponController : WeaponController
{
    [Header("Target Detection")]
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask targetLayer;

    private readonly HashSet<Transform> targets = new();

    private Transform currentTarget;

    private CircleCollider2D detectionCollider;

    protected override void Awake()
    {
        base.Awake();

        SetupDetection();
    }

    protected override void Update()
    {
        if (currentTarget == null)
            return;

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            RemoveCurrentTarget();
            return;
        }

        AimAtTarget();
        RotateHolderToWeapon();

        HandleFire();
    }

    //========================================================
    // DETECTION
    //========================================================

    private void SetupDetection()
    {
        detectionCollider = GetComponent<CircleCollider2D>();

        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsTargetLayer(collision.gameObject))
            return;

        Transform target = GetTargetTransform(collision);

        if (target == null)
            return;

        if (targets.Add(target))
            FindNearestTarget();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsTargetLayer(collision.gameObject))
            return;

        Transform target = GetTargetTransform(collision);

        if (target == null)
            return;

        if (targets.Remove(target))
        {
            if (currentTarget == target)
                FindNearestTarget();
        }
    }

    private Transform GetTargetTransform(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
            return collision.attachedRigidbody.transform;

        return collision.transform;
    }

    private bool IsTargetLayer(GameObject target)
    {
        return (targetLayer.value & (1 << target.layer)) != 0;
    }

    //========================================================
    // TARGET
    //========================================================

    private void FindNearestTarget()
    {
        Transform nearestTarget = null;

        float nearestDistanceSqr = float.MaxValue;

        Vector2 currentPosition = transform.position;

        foreach (Transform target in targets)
        {
            if (target == null)
                continue;

            if (!target.gameObject.activeInHierarchy)
                continue;

            Vector2 offset = (Vector2)target.position - currentPosition;

            float distanceSqr = offset.sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestTarget = target;
            }
        }

        currentTarget = nearestTarget;
    }

    private void RemoveCurrentTarget()
    {
        if (currentTarget == null)
            return;

        targets.Remove(currentTarget);

        FindNearestTarget();
    }

    //========================================================
    // AIM
    //========================================================

    private void AimAtTarget()
    {
        if (currentTarget == null)
            return;

        AimAtPosition(currentTarget.position);
    }

    //========================================================
    // FIRE
    //========================================================

    protected override void HandleFire()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer < fireInterval)
            return;

        fireTimer = 0f;

        if (currentTarget == null)
            return;

        Fire(directionToTarget);
    }

    //========================================================
    // GIZMOS
    //========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}