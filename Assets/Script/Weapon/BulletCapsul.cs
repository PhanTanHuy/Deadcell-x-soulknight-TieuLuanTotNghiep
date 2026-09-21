using UnityEngine;

public class BulletCapsul : ProjectileMovement
{
    private void OnEnable()
    {
        RotateTransformToDirection();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBox hitBox = collision.GetComponent<HitBox>();
        hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
    }
}
