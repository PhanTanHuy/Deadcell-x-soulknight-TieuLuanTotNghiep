using UnityEngine;

public class FanObject : RotateChildrenAround
{
    [Header("Detection")]
    [SerializeField] private float radiusAttack = 3f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Attack")]
    [SerializeField] private float damageInterval = 0.25f;
    [SerializeField] private int damage = 1;

    private float damageTimer;

    protected override void OnEnable()
    {
        base.OnEnable();

        damageTimer = 0f;
    }

    protected override void Update()
    {
        base.Update();

        HandleDamage();
    }

    private void HandleDamage()
    {
        damageTimer += Time.deltaTime;

        if (damageTimer < damageInterval)
            return;

        damageTimer = 0f;

        DetectEnemies();
    }

    private void DetectEnemies()
    {
        Transform center = GetOrbitCenter();

        if (center == null)
            return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            center.position,
            radiusAttack,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            HitBox hitBox = enemy.GetComponent<HitBox>();

            if (hitBox != null)
            {
                hitBox.GetDame(damage);
            }
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
            radiusAttack
        );
    }
}