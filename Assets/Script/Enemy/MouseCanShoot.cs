using UnityEngine;

public class MouseCanShoot : MouseAI
{
    [Header("Attack")]
    [SerializeField] private Transform gunTransform;
    [SerializeField] private ProjectileAttack projectileAttack;
    [SerializeField] private MouseAttackData attackData;

    // Runtime state
    private float fireTimer;
    private float holdTimer;

    private bool isHolding;

    // Pattern runtime
    private float patternTimer;
    private float currentPatternAngle;

    protected override void Awake()
    {
        base.Awake();

        if (projectileAttack == null)
        {
            projectileAttack =
                GetComponent<ProjectileAttack>()
                ?? GetComponentInChildren<ProjectileAttack>();
        }

        ResetPattern();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void Update()
    {
        if (target == null ||
            attackData == null ||
            !isChasing)
        {
            ResetAttack();
            return;
        }

        if (gunTransform == null ||
            projectileAttack == null)
        {
            ResetAttack();
            return;
        }

        AimAtTarget();

        switch (attackData.attackType)
        {
            case MouseAttackType.Normal:
                HandleNormalAttack();
                break;

            case MouseAttackType.Pattern:
                HandlePatternAttack();
                break;
        }
    }

    // =========================================================
    // AIM
    // =========================================================

    private Vector2 AimAtTarget()
    {
        Vector2 dir =
            (Vector2)target.position -
            (Vector2)gunTransform.position;

        if (dir.sqrMagnitude <= 0.0001f)
            return Vector2.zero;

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        gunTransform.rotation =
            Quaternion.AngleAxis(
                angle,
                Vector3.forward
            );

        return dir.normalized;
    }

    private bool IsAimed(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
            return false;

        float targetAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

        float gunAngle =
            gunTransform.eulerAngles.z;

        float angleDiff =
            Mathf.Abs(
                Mathf.DeltaAngle(
                    gunAngle,
                    targetAngle
                )
            );

        return angleDiff <= attackData.aimThreshold;
    }

    // =========================================================
    // NORMAL ATTACK
    // =========================================================

    private void HandleNormalAttack()
    {
        Vector2 direction =
            ((Vector2)target.position -
             (Vector2)gunTransform.position).normalized;

        if (!IsAimed(direction))
        {
            fireTimer = 0f;
            HandleHold(false);
            return;
        }

        // -----------------------------
        // Bắn 1 viên thẳng
        // -----------------------------

        fireTimer += Time.deltaTime;

        if (fireTimer >= attackData.fireCooldown)
        {
            projectileAttack.ShootProjectile(
                attackData.projectileIndex,
                direction
            );

            fireTimer = 0f;
        }

        // -----------------------------
        // Hold projectile
        // -----------------------------

        HandleHold(true, direction);
    }

    // =========================================================
    // HOLD PROJECTILE
    // =========================================================

    private void HandleHold(
        bool canHold,
        Vector2 direction = default)
    {
        if (!canHold ||
            attackData.holdDelay <= 0f)
        {
            if (isHolding)
            {
                projectileAttack.DeactiveProjectHold(
                    attackData.projectileIndex
                );

                isHolding = false;
            }

            holdTimer = 0f;
            return;
        }

        holdTimer += Time.deltaTime;

        if (!isHolding &&
            holdTimer >= attackData.holdDelay)
        {
            projectileAttack.ActiveProjectHold(
                attackData.projectileIndex,
                direction
            );

            isHolding = true;
        }
    }

    // =========================================================
    // BULLET PATTERN
    // =========================================================

    private void HandlePatternAttack()
    {
        BulletPatternData pattern =
            attackData.patternData;

        if (pattern == null)
            return;

        patternTimer += Time.deltaTime;

        if (patternTimer < pattern.fireInterval)
            return;

        patternTimer = 0f;

        FirePattern(pattern);
    }

    private void FirePattern(BulletPatternData pattern)
    {
        switch (pattern.patternType)
        {
            case BulletPatternType.Circle:
                FireCircle(pattern);
                break;

            case BulletPatternType.Spiral:
                FireSpiral(pattern);
                break;

            case BulletPatternType.DoubleSpiral:
                FireDoubleSpiral(pattern);
                break;
        }
    }

    // =========================================================
    // CIRCLE
    // =========================================================

    private void FireCircle(BulletPatternData pattern)
    {
        float angleStep =
            360f / pattern.bulletCount;

        for (int i = 0; i < pattern.bulletCount; i++)
        {
            float angle =
                currentPatternAngle +
                pattern.startAngle +
                angleStep * i;

            FireBulletAtAngle(angle);
        }

        currentPatternAngle += pattern.angleStep;
    }

    // =========================================================
    // SPIRAL
    // =========================================================

    private void FireSpiral(BulletPatternData pattern)
    {
        float angle =
            currentPatternAngle +
            pattern.startAngle;

        FireBulletAtAngle(angle);

        currentPatternAngle +=
            pattern.angleStep;
    }

    // =========================================================
    // DOUBLE SPIRAL
    // =========================================================

    private void FireDoubleSpiral(
        BulletPatternData pattern)
    {
        float angle =
            currentPatternAngle +
            pattern.startAngle;

        FireBulletAtAngle(angle);
        FireBulletAtAngle(angle + 180f);

        currentPatternAngle +=
            pattern.angleStep;
    }

    // =========================================================
    // FIRE BULLET
    // =========================================================

    private void FireBulletAtAngle(float angle)
    {
        float radians =
            angle * Mathf.Deg2Rad;

        Vector2 direction =
            new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

        projectileAttack.ShootProjectile(
            attackData.projectileIndex,
            direction
        );
    }

    // =========================================================
    // RESET
    // =========================================================

    private void ResetAttack()
    {
        fireTimer = 0f;
        holdTimer = 0f;

        patternTimer = 0f;

        if (isHolding &&
            projectileAttack != null &&
            attackData != null)
        {
            projectileAttack.DeactiveProjectHold(
                attackData.projectileIndex
            );

            isHolding = false;
        }
    }

    private void ResetPattern()
    {
        patternTimer = 0f;

        if (attackData != null &&
            attackData.patternData != null)
        {
            currentPatternAngle =
                attackData.patternData.startAngle;
        }
        else
        {
            currentPatternAngle = 0f;
        }
    }

    protected override void OnDisable()
    {
        ResetAttack();
        base.OnDisable();
    }
}