using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PartnerMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float catchUpSpeed = 5.5f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float catchUpDistance = 3f;
    [SerializeField] private float directionSmooth = 5f;

    [Header("Natural Movement")]
    [SerializeField] private float randomOffsetRadius = 0.7f;
    [SerializeField] private float offsetChangeMinTime = 1.5f;
    [SerializeField] private float offsetChangeMaxTime = 3f;

    [Header("Rotation")]
    [SerializeField] private Transform sprite;

    private Rigidbody2D rb;

    private Vector2 currentDirection;
    private Vector2 randomOffset;

    private float offsetTimer;
    private float nextOffsetChangeTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        ChooseRandomOffset();
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        UpdateRandomOffset();

        MoveTowardsPlayer();
    }

    private void UpdateRandomOffset()
    {
        offsetTimer += Time.fixedDeltaTime;

        if (offsetTimer >= nextOffsetChangeTime)
        {
            ChooseRandomOffset();
        }
    }

    private void ChooseRandomOffset()
    {
        offsetTimer = 0f;

        nextOffsetChangeTime = Random.Range(
            offsetChangeMinTime,
            offsetChangeMaxTime
        );

        randomOffset = Random.insideUnitCircle * randomOffsetRadius;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 targetPosition = (Vector2)player.position + randomOffset;
        Vector2 toTarget = targetPosition - rb.position;

        float distance = toTarget.magnitude;

        if (distance <= stopDistance)
        {
            currentDirection = Vector2.Lerp(
                currentDirection,
                Vector2.zero,
                directionSmooth * Time.fixedDeltaTime
            );

            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetDirection = toTarget.normalized;

        currentDirection = Vector2.Lerp(
            currentDirection,
            targetDirection,
            directionSmooth * Time.fixedDeltaTime
        );

        float currentSpeed = moveSpeed;

        if (distance >= catchUpDistance)
        {
            currentSpeed = catchUpSpeed;
        }

        Vector2 movement = currentDirection * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movement);

    }

    

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    public Transform GetPlayer()
    {
        return player;
    }
}