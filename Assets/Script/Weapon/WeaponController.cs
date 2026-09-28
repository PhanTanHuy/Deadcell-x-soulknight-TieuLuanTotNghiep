using System.Collections;
using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    public enum WeaponRotationMode
    {
        SharedRotation,
        IndividualRotation
    }

    [Header("Weapon")]
    [SerializeField] protected Transform[] spriteWeapons;
    [SerializeField] protected Transform[] shootPos;
    [SerializeField] protected ProjectileAttack projectileAttack;
    [SerializeField] protected float fireInterval = 1f;
    [SerializeField] protected WeaponRotationMode rotationMode = WeaponRotationMode.SharedRotation;

    [Header("Recoil")]
    public float recoilDistance = 0.15f;
    [SerializeField] protected float recoilDuration = 0.2f;

    [Header("Fire")]
    [SerializeField] protected bool fireHold;

    protected float fireTimer;
    protected Vector2 directionToTarget;

    protected Vector3[] spriteOriginalLocalPositions;
    protected Coroutine[] recoilCoroutines;

    protected static readonly Vector3 NormalScale = Vector3.one;
    protected static readonly Vector3 FlipScale = new Vector3(-1f, 1f, 1f);

    protected virtual void Awake()
    {
        InitializeWeapons();
    }

    protected virtual void OnDisable()
    {
        if (fireHold)
            DeactiveAllProjectileHolds();
    }

    protected virtual void Update()
    {
        HandleFire();
    }

    //========================================================
    // INITIALIZE
    //========================================================

    protected virtual void InitializeWeapons()
    {
        int weaponCount = spriteWeapons != null ? spriteWeapons.Length : 0;

        spriteOriginalLocalPositions = new Vector3[weaponCount];
        recoilCoroutines = new Coroutine[weaponCount];

        for (int i = 0; i < weaponCount; i++)
        {
            if (spriteWeapons[i] != null)
                spriteOriginalLocalPositions[i] = spriteWeapons[i].localPosition;
        }
    }

    //========================================================
    // FIRE
    //========================================================

    protected virtual void HandleFire()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer < fireInterval)
            return;

        fireTimer = 0f;

        if (!fireHold)
            Fire(directionToTarget);
    }

    protected virtual void Fire(Vector2 direction)
    {
        if (projectileAttack == null)
            return;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        int count = Mathf.Min(spriteWeapons.Length, shootPos.Length);

        for (int i = 0; i < count; i++)
        {
            if (shootPos[i] == null)
                continue;

            bool fired = projectileAttack.ShootProjectile(0, direction, shootPos[i]);

            if (fired)
                PlayRecoil(i);
        }
    }

    //========================================================
    // AIM
    //========================================================

    protected virtual void AimAtPosition(Vector3 targetPosition)
    {
        directionToTarget = targetPosition - transform.position;

        if (directionToTarget.sqrMagnitude <= 0.001f)
            return;

        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (rotationMode == WeaponRotationMode.SharedRotation)
            RotateAllWeaponsTogether(targetRotation);
        else
            RotateWeaponsIndividually(targetPosition);
    }

    protected virtual void RotateAllWeaponsTogether(Quaternion targetRotation)
    {
        transform.rotation = targetRotation;

        bool flip = directionToTarget.x < 0f;

        for (int i = 0; i < spriteWeapons.Length; i++)
        {
            if (spriteWeapons[i] == null)
                continue;

            spriteWeapons[i].localScale = flip ? FlipScale : NormalScale;
        }
    }

    protected virtual void RotateWeaponsIndividually(Vector3 targetPosition)
    {
        bool flip = directionToTarget.x < 0f;

        for (int i = 0; i < spriteWeapons.Length; i++)
        {
            Transform weapon = spriteWeapons[i];

            if (weapon == null)
                continue;

            Vector3 direction = targetPosition - weapon.position;

            if (direction.sqrMagnitude <= 0.001f)
                continue;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            weapon.localScale = flip ? FlipScale : NormalScale;
        }
    }

    //========================================================
    // HOLD PROJECTILE
    //========================================================

    protected virtual void ActiveAllProjectileHolds()
    {
        if (projectileAttack == null)
            return;

        int count = Mathf.Min(spriteWeapons.Length, shootPos.Length);

        for (int i = 0; i < count; i++)
        {
            if (shootPos[i] == null)
                continue;

            projectileAttack.ActiveProjectHold(0, directionToTarget, shootPos[i]);
        }
    }

    protected virtual void DeactiveAllProjectileHolds()
    {
        if (projectileAttack == null)
            return;

        int count = Mathf.Min(spriteWeapons.Length, shootPos.Length);

        for (int i = 0; i < count; i++)
            projectileAttack.DeactiveProjectHold(0);
    }

    //========================================================
    // RECOIL
    //========================================================

    protected virtual void PlayRecoil(int index)
    {
        if (index < 0 || index >= spriteWeapons.Length)
            return;

        if (spriteWeapons[index] == null)
            return;

        if (recoilCoroutines[index] != null)
            StopCoroutine(recoilCoroutines[index]);

        recoilCoroutines[index] = StartCoroutine(Recoil(index));
    }

    protected virtual IEnumerator Recoil(int index)
    {
        Transform weapon = spriteWeapons[index];

        Vector3 startPosition = spriteOriginalLocalPositions[index];
        Vector3 recoilPosition = startPosition - Vector3.up * recoilDistance;

        float halfDuration = recoilDuration * 0.5f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;

            float t = timer / halfDuration;

            weapon.localPosition = Vector3.Lerp(startPosition, recoilPosition, t);

            yield return null;
        }

        timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;

            float t = timer / halfDuration;

            weapon.localPosition = Vector3.Lerp(recoilPosition, startPosition, t);

            yield return null;
        }

        weapon.localPosition = startPosition;
        recoilCoroutines[index] = null;
    }

    //========================================================
    // PUBLIC
    //========================================================

    public void IncreaseShootSpeed(float percent)
    {
        if (percent <= 0f)
            return;

        fireInterval /= 1f + percent / 100f;
    }

    public void ChangeShootSpeed(float time)
    {
        fireInterval = time;
    }

    public void ChangeHoldShoot(bool value)
    {
        fireHold = value;
    }

    public void ChangeRotationMode(WeaponRotationMode mode)
    {
        rotationMode = mode;
    }

    public float GetFireInterval()
    {
        return fireInterval;
    }

    public ProjectileAttack GetProjectileAttack()
    {
        return projectileAttack;
    }
}