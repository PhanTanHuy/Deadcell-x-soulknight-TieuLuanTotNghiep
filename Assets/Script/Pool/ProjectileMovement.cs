using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    // Định nghĩa 3 chế độ hoạt động theo yêu cầu của bạn
    public enum MoveMode
    {
        None,
        StraightDirection, // Đi thẳng theo hướng cố định
        FollowTarget,      // Đuổi theo mục tiêu, dừng lại ở khoảng cách quy định
        LatchTarget        // Bám dính cứng vào mục tiêu (không dùng SetParent)
    }

    [Header("--- SETTINGS ---")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float stoppingDistance = 1.5f;

    private MoveMode currentMode;
    private Vector2 moveDirection;
    public Vector2 MoveDirection => moveDirection;
    private Transform target;
    protected Transform shootPos;
    [SerializeField] protected int damageAmountPlus = 1;
    [HideInInspector] public ProjectileAttack projectileAttack;

    // Các biến phục vụ tối ưu hiệu suất
    private Vector2 latchOffset;
    private float sqrStoppingDistance;
    private float defaultSpeed;

    protected virtual void Awake()
    {
        // Tính trước bình phương khoảng cách dừng để dùng cho so sánh SqrMagnitude
        sqrStoppingDistance = stoppingDistance * stoppingDistance;
        defaultSpeed = speed;
    }

    /// <summary>
    /// Hàm khởi tạo tổng hợp - Gọi ngay sau khi lấy Object ra khỏi Pool
    /// </summary>
    public void InitializeProjectile(MoveMode mode, Vector2 spawnPos, Transform targetTransform = null, Vector2 direction = default)
    {
        currentMode = mode;
        transform.position = spawnPos;
        target = targetTransform;

        switch (mode)
        {
            case MoveMode.StraightDirection:
                moveDirection = direction.normalized;
                break;

            case MoveMode.FollowTarget:
                // Nếu chế độ follow mà không truyền target thì tự hủy hoặc chuyển về đi thẳng
                if (target == null) currentMode = MoveMode.StraightDirection;
                break;

            case MoveMode.LatchTarget:
                if (target != null)
                {
                    // Tính khoảng cách lệch giữa đạn và tâm của mục tiêu ngay tại thời điểm bám
                    latchOffset = spawnPos - (Vector2)target.position;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }
    public void RotateTransformToDirection()
    {
        Vector2 direction = MoveDirection;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    protected virtual void FixedUpdate()
    {
        if (speed <= 0f) return; // Không di chuyển nếu tốc độ <= 0
        switch (currentMode)
        {
            case MoveMode.None:
                // Không làm gì cả
                break;
            case MoveMode.StraightDirection:
                // Tối ưu hơn Translate: Gán thẳng vector vị trí
                transform.position += (Vector3)(moveDirection * speed * Time.fixedDeltaTime);
                break;

            case MoveMode.FollowTarget:
                if (target == null)
                {
                    gameObject.SetActive(false);
                    return;
                }

                Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;

                // TỐI ƯU: So sánh bình phương khoảng cách để né tính toán căn bậc hai (Mathf.Sqrt)
                if (toTarget.sqrMagnitude > sqrStoppingDistance)
                {
                    transform.position += (Vector3)(toTarget.normalized * speed * Time.fixedDeltaTime);
                }
                break;

            case MoveMode.LatchTarget:
                if (target == null || !target.gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(false); // Target chết hoặc biến mất thì đạn tự tắt
                    return;
                }

                // KHÔNG DÙNG SETPARENT: Đạn liên tục di chuyển theo vị trí của Target dựa trên offset
                transform.position = (Vector2)target.position + latchOffset;
                break;
        }
    }

    /// <summary>
    /// Hàm bổ trợ: Cho phép chuyển trạng thái sang bám dính (Latch) ngay khi va chạm từ code bên ngoài
    /// </summary>
    public void SwitchToLatchMode(Transform targetAttach)
    {
        target = targetAttach;
        currentMode = MoveMode.LatchTarget;
        latchOffset = (Vector2)transform.position - (Vector2)target.position;
    }
    public void SetShootPos(Transform tfShootPos)
    {
        shootPos = tfShootPos;
    }
    public void StopMovement()
    {
        currentMode = MoveMode.None;
    }
    public void PlusSpeed(float amount)
    {
        speed += amount;
    }
    public void ResetSpeed()
    {
        speed = defaultSpeed;
    }
}