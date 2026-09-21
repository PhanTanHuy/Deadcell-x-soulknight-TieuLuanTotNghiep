using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletElectric : ProjectileMovement
{
    private LineRenderer lineRenderer;

    [Header("Cấu hình Lan truyền")]
    [Tooltip("Bán kính quét tìm mục tiêu tiếp theo")]
    public float radius = 3f;

    [SerializeField] private int maxChainTargets = 5;
    [SerializeField] private float chainDelay = 0.1f;

    [Tooltip("LayerMask của Enemy")]
    public LayerMask enemyLayer;

    [Header("Ưu tiên hướng Projectile")]
    [Tooltip("Enemy phải nằm trong góc này so với hướng bay của projectile")]
    [SerializeField] private float forwardAngle = 90f;

    [Header("Cấu hình Tia sét")]
    [Tooltip("Thời gian tia sét hiển thị")]
    public float visualDuration = 0.25f;

    [Tooltip("Số lượng đoạn của tia sét")]
    public int segments = 8;

    [Tooltip("Độ gập ghềnh của tia sét")]
    public float noiseMagnitude = 0.4f;

    [Tooltip("Tốc độ đổi hình dáng tia sét")]
    public float flickerInterval = 0.04f;

    [Header("Tia sét khi không có Enemy")]
    [SerializeField] private float minFallbackDistance = 1.5f;
    [SerializeField] private float maxFallbackDistance = 2.5f;

    private Coroutine shockRoutine;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = Random.Range(0.05f, 0.1f);

        shockRoutine = StartCoroutine(DoShockChain());
    }

    private void OnDisable()
    {
        if (shockRoutine != null)
        {
            StopCoroutine(shockRoutine);
            shockRoutine = null;
        }

        lineRenderer.positionCount = 0;
    }

    private IEnumerator DoShockChain()
    {
        // Đợi 1 frame để đảm bảo ProjectileMovement đã Initialize
        yield return null;


        Vector2 moveDirection = MoveDirection;

        if (moveDirection.sqrMagnitude <= 0.001f)
            yield break;

        moveDirection.Normalize();

        HashSet<Transform> visitedEnemies = new HashSet<Transform>();

        Vector3 currentPosition = transform.position;

        for (int i = 0; i < maxChainTargets; i++)
        {
            Transform nextEnemy = FindBestEnemy(
                currentPosition,
                moveDirection,
                visitedEnemies
            );

            // Không có enemy
            if (nextEnemy == null)
            {
                // Nếu đây là lần đầu tiên tìm enemy
                // thì tạo tia sét theo hướng projectile
                if (i == 0)
                {
                    float distance = Random.Range(
                        minFallbackDistance,
                        maxFallbackDistance
                    );

                    Vector3 fallbackEnd =
                        currentPosition +
                        (Vector3)(moveDirection * distance);

                    StartCoroutine(
                        ShowLightning(
                            currentPosition,
                            fallbackEnd
                        )
                    );
                }

                break;
            }

            // Gây damage
            nextEnemy.GetComponent<HitBox>()?.GetDame(damageAmountPlus + projectileAttack.DamageAmount);

            visitedEnemies.Add(nextEnemy);

            // Tạo tia sét đến enemy
            StartCoroutine(
                ShowLightning(
                    currentPosition,
                    nextEnemy
                )
            );

            // Vị trí hiện tại chuyển sang enemy
            currentPosition = nextEnemy.position;

            // Sau khi chain đến enemy,
            // tiếp tục tìm enemy tiếp theo theo hướng từ vị trí mới
            yield return new WaitForSeconds(chainDelay);
        }
    }

    private Transform FindBestEnemy(
        Vector3 searchPosition,
        Vector2 moveDirection,
        HashSet<Transform> visitedEnemies)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            searchPosition,
            radius,
            enemyLayer
        );

        Transform bestEnemy = null;

        float bestScore = float.MinValue;

        foreach (Collider2D col in colliders)
        {
            Transform enemy = col.transform;

            if (visitedEnemies.Contains(enemy))
                continue;

            Vector2 directionToEnemy =
                (enemy.position - searchPosition);

            if (directionToEnemy.sqrMagnitude <= 0.001f)
                continue;

            directionToEnemy.Normalize();

            // Dot:
            //  1  = cùng hướng
            //  0  = vuông góc
            // -1  = ngược hướng
            float dot = Vector2.Dot(
                moveDirection,
                directionToEnemy
            );

            float angle = Vector2.Angle(
                moveDirection,
                directionToEnemy
            );

            // Chỉ ưu tiên enemy nằm phía trước
            if (angle > forwardAngle)
                continue;

            /*
             * Score:
             *
             * dot càng lớn -> enemy càng nằm đúng hướng
             * khoảng cách càng gần -> score càng cao
             *
             * Có thể chỉnh trọng số tùy gameplay.
             */
            float distance = Vector2.Distance(
                searchPosition,
                enemy.position
            );

            float distanceScore = 1f / (distance + 0.1f);

            float score =
                dot * 2f +
                distanceScore;

            if (score > bestScore)
            {
                bestScore = score;
                bestEnemy = enemy;
            }
        }

        return bestEnemy;
    }

    private IEnumerator ShowLightning(
        Vector3 start,
        Transform enemy)
    {
        float elapsed = 0f;
        float flickerTimer = 0f;

        while (elapsed < visualDuration)
        {
            if (enemy == null)
                yield break;

            elapsed += Time.deltaTime;
            flickerTimer -= Time.deltaTime;

            if (flickerTimer <= 0f)
            {
                DrawLightning(
                    start,
                    enemy.position
                );

                flickerTimer = flickerInterval;
            }

            yield return null;
        }

        lineRenderer.positionCount = 0;
    }

    private IEnumerator ShowLightning(
        Vector3 start,
        Vector3 end)
    {
        float elapsed = 0f;
        float flickerTimer = 0f;

        while (elapsed < visualDuration)
        {
            elapsed += Time.deltaTime;
            flickerTimer -= Time.deltaTime;

            if (flickerTimer <= 0f)
            {
                DrawLightning(
                    start,
                    end
                );

                flickerTimer = flickerInterval;
            }

            yield return null;
        }

        lineRenderer.positionCount = 0;
    }

    private void DrawLightning(
        Vector3 start,
        Vector3 end)
    {
        lineRenderer.positionCount = segments;

        lineRenderer.SetPosition(
            0,
            start
        );

        lineRenderer.SetPosition(
            segments - 1,
            end
        );

        Vector3 direction =
            (end - start).normalized;

        Vector3 perpendicular =
            new Vector3(
                -direction.y,
                direction.x,
                0f
            );

        for (int i = 1; i < segments - 1; i++)
        {
            float fraction =
                (float)i / (segments - 1);

            Vector3 midPoint =
                Vector3.Lerp(
                    start,
                    end,
                    fraction
                );

            float noise =
                Random.Range(
                    -noiseMagnitude,
                    noiseMagnitude
                );

            float envelope =
                Mathf.Sin(
                    fraction * Mathf.PI
                );

            midPoint +=
                perpendicular *
                noise *
                envelope;

            lineRenderer.SetPosition(
                i,
                midPoint
            );
        }
    }
}