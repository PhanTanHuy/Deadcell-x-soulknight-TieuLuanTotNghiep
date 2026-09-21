using UnityEngine;

public class BulletCircle : ProjectileMovement
{
    public bool hitEnemy = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitEnemy)
        if (collision.CompareTag("Enemy"))
        {
            HitBox hitBox = collision.GetComponent<HitBox>();
            hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
            gameObject.SetActive(false);
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                HitBox hitBox = collision.GetComponent<HitBox>();
                hitBox.GetDame(damageAmountPlus + projectileAttack.DamageAmount);
                gameObject.SetActive(false);
            }
        }
    }
}
