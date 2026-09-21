using UnityEngine;

public class BulletGojoBlue : ProjectileMovement
{
    [Header("Visual")]
    [SerializeField] private GameObject blue;
    [SerializeField] private GameObject puple;

    [Header("Pull")]
    [SerializeField] private float pullRadius = 5f;
    [SerializeField] private float pullSpeed = 10f;
    [SerializeField] private LayerMask enemyLayer;
    private float pullSpeedPlus = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BulletEnemy"))
        {
            collision.gameObject.SetActive(false);
            if (!puple.activeInHierarchy)
            {
                blue.SetActive(false);
                puple.SetActive(true);

                PlusSpeed(10);

                ShakeCam.Instance.Shake(0.35f, 0.55f);
            }
            
        }

        if (collision.CompareTag("Enemy") && puple.activeInHierarchy)
        {
            HitBox hitBox = collision.GetComponentInParent<HitBox>();

            if (hitBox != null)
            {
                hitBox.GetDame(
                    damageAmountPlus + projectileAttack.DamageAmount
                );
                hitBox.KnockBack(
                    (collision.transform.position - transform.position).normalized,
                    30f
                );
            }
        }
    }

    private void OnEnable()
    {
        ResetSpeed();
        pullSpeedPlus = 0f;
        blue.SetActive(true);
        puple.SetActive(false);
    }

    private void Update()
    {
        PullEnemies();
    }

    private void PullEnemies()
    {
        pullSpeedPlus += pullSpeed * Time.deltaTime;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            pullRadius,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            Transform enemyRoot = enemy.transform;

            Vector2 currentPosition = enemyRoot.position;

            Vector2 direction =
                (Vector2)transform.position - currentPosition;

            float distance = direction.magnitude;

            if (distance <= 0.01f)
                continue;

            float moveDistance = pullSpeed * Time.deltaTime;

            if (distance <= moveDistance)
            {
                enemyRoot.position = transform.position;
            }
            else
            {
                enemyRoot.position +=
                    (Vector3)(direction.normalized * moveDistance);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            pullRadius
        );
    }
}