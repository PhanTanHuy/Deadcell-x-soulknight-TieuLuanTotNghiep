using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    public enum WeaponRotationMode
    {
        SharedRotation,
        IndividualRotation
    }

    [Header("Weapon")]
    [SerializeField] private Transform weaponPos;
    [SerializeField] private Transform[] spriteWeapons;
    [SerializeField] private Transform[] shootPos;
    [SerializeField] private ProjectileAttack projectileAttack;
    [SerializeField] private float fireInterval = 1f;
    [SerializeField] private WeaponRotationMode rotationMode = WeaponRotationMode.SharedRotation;

    [Header("Player")]
    [SerializeField] private Transform spritePlayer;

    [Header("Recoil")]
    public float recoilDistance = 0.15f;
    [SerializeField] private float recoilDuration = 0.2f;

    private InputSystem_Actions inputActions;
    private Camera mainCamera;
    private bool isAttacking;
    private float fireTimer;
    private Vector2 directionToMouse;
    [SerializeField] private bool fireHold = false;

    private static readonly Vector3 NormalScale = Vector3.one;
    private static readonly Vector3 FlipScale = new Vector3(-1f, 1f, 1f);

    private Vector3[] spriteOriginalLocalPositions;
    private Coroutine[] recoilCoroutines;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        InitializeWeapons();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Attack.canceled += OnAttackCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Attack.canceled -= OnAttackCanceled;
        inputActions.Disable();

        if (fireHold)
            DeactiveAllProjectileHolds();
    }

    private void InitializeWeapons()
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

    private void Update()
    {
        RotateWeaponsTowardsLook();
        RotatePlayerToWeapon();
        HandleFire();
    }

    //========================================================
    // ROTATION
    //========================================================

    private void RotateWeaponsTowardsLook()
    {
        if (weaponPos == null)
            return;

        if (mainCamera == null)
            return;

        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        directionToMouse = worldMousePosition - weaponPos.position;

        if (directionToMouse.sqrMagnitude <= 0.001f)
            return;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (rotationMode == WeaponRotationMode.SharedRotation)
            RotateAllWeaponsTogether(targetRotation);
        else
            RotateWeaponsIndividually(worldMousePosition);
    }

    //========================================================
    // MODE 1
    // Tất cả súng xoay cùng nhau
    //========================================================

    private void RotateAllWeaponsTogether(Quaternion targetRotation)
    {
        weaponPos.rotation = targetRotation;

        for (int i = 0; i < spriteWeapons.Length; i++)
        {
            if (spriteWeapons[i] == null)
                continue;

            spriteWeapons[i].localScale = directionToMouse.x < 0f ? FlipScale : NormalScale;
        }
    }

    //========================================================
    // MODE 2
    // Mỗi súng tự hướng về chuột
    //========================================================

    private void RotateWeaponsIndividually(Vector3 worldMousePosition)
    {
        for (int i = 0; i < spriteWeapons.Length; i++)
        {
            Transform weapon = spriteWeapons[i];

            if (weapon == null)
                continue;

            Vector3 direction = worldMousePosition - weapon.position;

            if (direction.sqrMagnitude <= 0.001f)
                continue;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            weapon.localScale = direction.x < 0f ? FlipScale : NormalScale;
        }
    }

    private void RotatePlayerToWeapon()
    {
        if (spritePlayer == null)
            return;

        spritePlayer.localScale = directionToMouse.x < 0f ? FlipScale : NormalScale;
    }

    //========================================================
    // FIRE
    //========================================================

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        isAttacking = true;

        if (fireHold && !projectileAttack.IsProjectileActive)
            ActiveAllProjectileHolds();
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;

        if (fireHold && projectileAttack.IsProjectileActive)
            DeactiveAllProjectileHolds();
    }

    private void HandleFire()
    {
        fireTimer += Time.deltaTime;

        if (!isAttacking)
            return;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;

            if (!fireHold)
                Fire();
        }
    }

    private void Fire()
    {
        if (projectileAttack == null)
            return;

        int count = Mathf.Min(spriteWeapons.Length, shootPos.Length);

        for (int i = 0; i < count; i++)
        {
            if (shootPos[i] == null)
                continue;

            bool fired = projectileAttack.ShootProjectile(0, directionToMouse, shootPos[i]);

            if (fired)
                PlayRecoil(i);
        }
    }

    //========================================================
    // HOLD PROJECTILE
    //========================================================

    private void ActiveAllProjectileHolds()
    {
        int count = Mathf.Min(spriteWeapons.Length, shootPos.Length);

        for (int i = 0; i < count; i++)
        {
            if (shootPos[i] == null)
                continue;

            projectileAttack.ActiveProjectHold(0, directionToMouse, shootPos[i]);
        }
    }

    private void DeactiveAllProjectileHolds()
    {
        int count = Mathf.Min(spriteWeapons.Length, shootPos.Length);

        for (int i = 0; i < count; i++)
            projectileAttack.DeactiveProjectHold(0);
    }

    //========================================================
    // RECOIL
    //========================================================

    private void PlayRecoil(int index)
    {
        if (index < 0 || index >= spriteWeapons.Length)
            return;

        if (spriteWeapons[index] == null)
            return;

        if (recoilCoroutines[index] != null)
            StopCoroutine(recoilCoroutines[index]);

        recoilCoroutines[index] = StartCoroutine(Recoil(index));
    }

    private IEnumerator Recoil(int index)
    {
        Transform weapon = spriteWeapons[index];
        Vector3 startPosition = spriteOriginalLocalPositions[index];
        Vector3 recoilPosition = startPosition - Vector3.up * recoilDistance;
        float halfDuration = recoilDuration * 0.5f;
        float timer = 0f;

        // Đi lùi.
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            weapon.localPosition = Vector3.Lerp(startPosition, recoilPosition, t);
            yield return null;
        }

        timer = 0f;

        // Trở lại.
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
}