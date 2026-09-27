using UnityEngine;

public class BulletEnemy : ProjectileMovement
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Vehicle"))
        {
            HitBox hitBox = collision.GetComponent<HitBox>();
            hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
            gameObject.SetActive(false);
        }
    }
}
