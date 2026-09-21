using UnityEngine;

public class MouseCrazy : MouseAI
{
    [Header("Crazy Attack")]

    private bool isCrazy;

    protected override void UpdateChaseState()
    {
        float distance = Vector2.Distance(
            rb.position,
            target.position
        );

        // B?t ch? ?? ?iên khi phát hi?n player trong bán kính
        isCrazy = distance <= chaseData.crazyDetectionRadius;
        isChasing = isCrazy;
    }

    protected override void ChaseTarget()
    {
        Vector2 toTarget =
            (Vector2)target.position - rb.position;

        float distance = toTarget.magnitude;

        // N?u ? ch? ?? ?iên, hành ??ng khác
        if (isCrazy)
        {
            CrazyChase();
            return;
        }

        // Ch? ?? bình th??ng (n?u quá g?n nh?ng ch?a ?iên)
        if (distance <= chaseData.stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetDirection = toTarget.normalized;

        directionTimer += Time.fixedDeltaTime;

        if (directionTimer >= nextDirectionChangeTime)
        {
            ChooseRandomDirection();
        }

        Vector2 desiredDirection =
            targetDirection +
            randomDirection *
            movementData.randomDirectionStrength;

        desiredDirection.Normalize();

        currentDirection = Vector2.Lerp(
            currentDirection,
            desiredDirection,
            movementData.directionSmooth *
            Time.fixedDeltaTime
        ).normalized;

        Move(currentDirection);
    }

    private void CrazyChase()
    {
        Vector2 toTarget =
            (Vector2)target.position - rb.position;

        if (toTarget.sqrMagnitude <= 0.0001f)
            return;

        // Lao th?ng vào target v?i t?c ?? crazy
        Vector2 crazyDirection = toTarget.normalized;

        currentDirection = crazyDirection;
        RotateToMovementDirection(crazyDirection);
        rb.linearVelocity = crazyDirection *  chaseData.crazyChaseSpeed;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // V? vòng tròn detection cho ch? ?? crazy
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            chaseData.crazyDetectionRadius
        );
    }
}
