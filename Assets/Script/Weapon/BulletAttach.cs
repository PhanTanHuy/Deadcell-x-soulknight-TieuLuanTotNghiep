using UnityEngine;

public class BulletAttach : ProjectileMovement
{
    private void OnEnable()
    {
        RotateTransformToDirection();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitBox hitBox = collision.GetComponent<HitBox>();
        hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
        SwitchToLatchMode(hitBox.transform);
    }
}
