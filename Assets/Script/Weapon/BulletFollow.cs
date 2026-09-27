using UnityEngine;

public class BulletFollow : ProjectileMovement
{
    [Header("Homing")]
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
            25,
            enemyLayer
        );

        if (hit == null)
            return null;
        PoolObject.instance.SpawnObject(PoolObject.VFXType.IdentityTarget, hit.transform, default, 2f, true);
        return hit.transform;
    }

    protected override void FixedUpdate()
    {
        if (target != null && target.gameObject.activeInHierarchy)
        {
            Vector2 targetDirection = ((Vector2)target.position - (Vector2)transform.position);

            MoveDirection = Vector2.Lerp(MoveDirection, targetDirection.normalized, turnSpeed * Time.fixedDeltaTime).normalized;
            RotateTransformToDirection();
            if (targetDirection.magnitude < 0.5f)
            {
                HitBox hitBox = target.GetComponent<HitBox>();
                hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
                gameObject.SetActive(false);
            }
        }

        base.FixedUpdate();
    }
}