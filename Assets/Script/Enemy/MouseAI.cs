using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class MouseAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] protected Transform target;

    [Header("Data")]
    [SerializeField] protected MouseMovementData movementData;
    [SerializeField] protected MouseChaseData chaseData;

    protected Rigidbody2D rb;

    protected Vector2 currentDirection;
    protected Vector2 randomDirection;

    protected float directionTimer;
    protected float nextDirectionChangeTime;
    protected float timeCanNotMove;

    protected bool isChasing;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ChooseRandomDirection();
    }

    protected virtual void FixedUpdate()
    {
        if (timeCanNotMove > 0f)
        {
            timeCanNotMove -= Time.fixedDeltaTime;
            return;
        }

        if (movementData == null || chaseData == null)
            return;

        if (target == null)
        {
            MoveRandomly();
            return;
        }

        UpdateChaseState();

       

        if (isChasing)
        {
            ChaseTarget();
        }
        else
        {
            MoveRandomly();
        }
    }

    protected virtual void UpdateChaseState()
    {
        float distance = Vector2.Distance(
            rb.position,
            target.position
        );

        isChasing = distance <= chaseData.chaseRadius;
    }

    protected virtual void ChaseTarget()
    {
        Vector2 toTarget =
            (Vector2)target.position - rb.position;

        float distance = toTarget.magnitude;

        if (distance <= chaseData.stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetDirection =
            toTarget.normalized;

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

    protected virtual void MoveRandomly()
    {
        directionTimer += Time.fixedDeltaTime;

        if (directionTimer >= nextDirectionChangeTime)
        {
            ChooseRandomDirection();
        }

        currentDirection = Vector2.Lerp(
            currentDirection,
            randomDirection,
            movementData.directionSmooth *
            Time.fixedDeltaTime
        ).normalized;

        Move(currentDirection);
    }

    protected virtual void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.001f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        RotateToMovementDirection(direction);

        rb.linearVelocity =
            direction * movementData.moveSpeed;
    }

    protected virtual void RotateToMovementDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    protected virtual void ChooseRandomDirection()
    {
        directionTimer = 0f;

        nextDirectionChangeTime = Random.Range(
            movementData.directionChangeMinTime,
            movementData.directionChangeMaxTime
        );

        randomDirection =
            Random.insideUnitCircle.normalized;
    }

    public void SetTimeCanNotMove(float time)
    {
        timeCanNotMove = time;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    protected virtual void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (chaseData == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            chaseData.chaseRadius
        );
    }
}