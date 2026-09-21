using UnityEngine;

public class TulenObject : RotateChildrenAround
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Attack")]
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float projectileSpeed = 10f;

    private float fireTimer;

    private Transform currentProjectile;
    private Collider2D currentTarget;

    protected override void OnEnable()
    {
        base.OnEnable();

        fireTimer = fireCooldown;
        currentProjectile = null;
        currentTarget = null;
    }

    protected override void Update()
    {
        base.Update();

        fireTimer += Time.deltaTime;

        if (currentProjectile != null)
        {
            MoveProjectile();
            return;
        }

        if (fireTimer < fireCooldown)
            return;

        TryShoot();
    }

    private void TryShoot()
    {
        fireTimer = 0f;

        Collider2D target = FindEnemy();

        if (target == null)
            return;

        Transform projectile = PopFirstActiveChild();

        if (projectile == null)
        {
            DisableWhenNoChildren();
            return;
        }

        currentProjectile = projectile;
        currentTarget = target;


        currentProjectile.SetParent(null);
    }

    private Collider2D FindEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            GetOrbitCenter().position,
            detectionRadius,
            enemyLayer
        );

        if (enemies.Length == 0)
        {
            Debug.Log("No enemies found within detection radius.");
            return null;

        }
        Debug.Log(enemies[0].gameObject.name + " is the first enemy found within detection radius.");
        return enemies[0];
    }

    private void MoveProjectile()
    {
        if (currentProjectile == null)
            return;

        if (currentTarget == null)
        {
            DestroyProjectile();
            return;
        }

        Vector3 currentPosition = currentProjectile.position;
        Vector3 targetPosition = currentTarget.transform.position;

        Vector3 direction =
            (targetPosition - currentPosition).normalized;

        currentProjectile.position += direction * projectileSpeed * Time.deltaTime;
        
        if (Vector3.Distance(currentProjectile.position, targetPosition) < 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (currentTarget != null)
        {
            HitBox hitBox =
                currentTarget.GetComponent<HitBox>();

            if (hitBox != null)
            {
                hitBox.GetDame(1);
            }
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (currentProjectile != null)
        {
            currentProjectile.SetParent(transform);
            currentProjectile.gameObject.SetActive(false);
        }

        currentProjectile = null;
        currentTarget = null;

        DisableWhenNoChildren();
    }

    private void DisableWhenNoChildren()
    {
        if (!HasActiveChildren())
        {
            gameObject.SetActive(false);
            PoolObject.instance.ReturnToPool(gameObject, PoolObject.VFXType.TulenObject);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Transform center = GetOrbitCenter();

        if (center == null)
            return;

        Gizmos.DrawWireSphere(
            center.position,
            detectionRadius
        );
    }
}