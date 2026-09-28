using UnityEngine;

public class ChangeBullet : Item
{
    public GameObject bulletPrefab; // The new bullet prefab to switch to
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speedTime = 0.5f; // The time it takes to change the bullet prefab
    public bool isHoldShoot = false;
    public int poolSize = 10; // The size of the projectile pool
    public int bulletPerShot = 1; // The number of bullets to shoot per shot
    public float shakeDuration;
    public float shakeMag;
    public float recoilDistance = 0.15f;
    public override void Pick(GameObject player)
    {
        ProjectileAttack p = GetComponent<ProjectileAttack>();
        p.ChangeProjectilePool(0, bulletPrefab, poolSize, bulletPerShot, shakeDuration, shakeMag);
        p.spriteWeapon.sprite = ItemSprite;
        UpgradeWeapon up = player.GetComponent<UpgradeWeapon>();
        up.ChangeShootSpeed(speedTime);
        up.ChangeFireHold(isHoldShoot);
        up.ChangeRecoilDistance(recoilDistance);

    }
}
