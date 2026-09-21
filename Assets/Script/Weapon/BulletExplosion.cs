using UnityEngine;

public class BulletExplosion : ProjectileMovement
{
    [Header("Explosion")]
    [SerializeField] private float radiusExplosion = 3f;
    [SerializeField] private LayerMask enemyLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode(transform.position);
    }

    private void Explode(Vector2 explosionPosition)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            explosionPosition,
            radiusExplosion,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            HitBox hitBox = enemy.GetComponentInParent<HitBox>();

            if (hitBox != null)
            {
                hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
            }
        }
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            radiusExplosion
        );
    }
}