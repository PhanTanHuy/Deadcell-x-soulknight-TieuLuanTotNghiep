using UnityEngine;

public class UpgradeWeapon : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private ProjectileAttack projectileAttack;
    public void IncreaseShootSpeed(float percent)
    {
        weaponController.IncreaseShootSpeed(percent);
    }
    public void IncreaseDamage(float percent)
    {

    }
    public void IncreaseScaleBullet(float percent)
    {
        projectileAttack.ChangeProjectileScale(0, percent);
    }
    public void ChangeBulletType(BulletType newType)
    {

    }
    public void AddBulletEffect()
    {

    }
    public void ChangeShootSpeed(float sp)
    {
        weaponController.ChangeShootSpeed(sp);
    }
    public void ChangeFireHold(bool b)
    {
        weaponController.ChangeHoldShoot(b);
    }
    public void ChangeRecoilDistance(float rcd)
    {
        weaponController.recoilDistance = rcd;
    }
}
