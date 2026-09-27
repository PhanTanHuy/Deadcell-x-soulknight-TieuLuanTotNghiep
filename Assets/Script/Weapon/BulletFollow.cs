using UnityEngine;

public class BulletFollow : ProjectileMovement
{
    [Header("Homing")]
    [SerializeField] private float searchRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float turnSpeed = 5f;

    private void OnEnable()
    {

        target = FindTarget();
    }

    private Transform FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            searchRadius,
            enemyLayer
        );

        if (hit == null)
            return null;
        PoolObject.instance.SpawnObject(PoolObject.VFXType.IdentityTarget, hit.transform.position, default, 2f);
        return hit.transform;
    }

    protected override void FixedUpdate()
    {
        if (target != null && target.gameObject.activeInHierarchy)
        {
            Vector2 targetDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;

            MoveDirection = Vector2.Lerp(MoveDirection, targetDirection, turnSpeed * Time.fixedDeltaTime).normalized;
            RotateTransformToDirection();

        }

        base.FixedUpdate();
    }
}