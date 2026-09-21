using UnityEngine;

public class BulletWave : ProjectileMovement
{
    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable()
    {
        RotateTransformToDirection();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HitBox hitBox = collision.GetComponent<HitBox>();
            hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
            hitBox.KnockBack(MoveDirection, 10f);
        }
    }
}
