using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform weaponPos;
    [SerializeField] private float fireInterval = 1f;
    [SerializeField] private ProjectileAttack projectileAttack;
    [SerializeField] private Transform spriteWeapon, spritePlayer;
    private InputSystem_Actions inputActions;
    private Camera mainCamera;

    private bool isAttacking;
    private float fireTimer;
    private Vector2 directionToMouse;
    private bool fireHold = false;
    private static readonly Vector3 NormalScale = Vector3.one;
    private static readonly Vector3 FlipScale = new Vector3(-1f, 1f, 1f);
    [SerializeField] private float recoilDistance = 0.15f;
    [SerializeField] private float recoilDuration = 0.2f;

    private Vector3 spriteOriginalLocalPosition;
    private Coroutine recoilCoroutine;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        spriteOriginalLocalPosition = spriteWeapon.localPosition;
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Subscribe input events
        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Attack.canceled += OnAttackCanceled;
    }

    private void OnDisable()
    {
        // Unsubscribe input events
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Attack.canceled -= OnAttackCanceled;

        inputActions.Disable();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        RotateWeaponTowardsLook();
        RotatePlayerToWeapon();
        HandleFire();
    }
    private void RotatePlayerToWeapon()
    {
        spritePlayer.localScale =
            directionToMouse.x < 0f
                ? FlipScale
                : NormalScale;
    }
    private void PlayRecoil()
    {
        if (spriteWeapon == null)
            return;

        if (recoilCoroutine != null)
        {
            StopCoroutine(recoilCoroutine);
        }

        recoilCoroutine = StartCoroutine(Recoil());
    }

    private IEnumerator Recoil()
    {
        Vector3 startPosition = spriteOriginalLocalPosition;

        Vector3 recoilPosition =
            startPosition - Vector3.up * recoilDistance;

        float halfDuration = recoilDuration * 0.5f;

        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;

            float t = timer / halfDuration;

            spriteWeapon.localPosition =
                Vector3.Lerp(
                    startPosition,
                    recoilPosition,
                    t
                );

            yield return null;
        }

        timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;

            float t = timer / halfDuration;

            spriteWeapon.localPosition =
                Vector3.Lerp(
                    recoilPosition,
                    startPosition,
                    t
                );

            yield return null;
        }

        spriteWeapon.localPosition = startPosition;

        recoilCoroutine = null;
    }
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        isAttacking = true;
        if (fireHold && !projectileAttack.IsProjectileActive) projectileAttack.ActiveProjectHold(0, directionToMouse);

        // Bắn ngay viên đầu tiên
        //Fire();

        // Reset timer
        //fireTimer = 0f;
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
        //fireTimer = 0f;
        if (fireHold && projectileAttack.IsProjectileActive) projectileAttack.DeactiveProjectHold(0);
    }

    private void HandleFire()
    {
        fireTimer += Time.deltaTime;

        if (!isAttacking)
            return;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;

            if (!fireHold) Fire();
        }
    }

    private void Fire()
    {
        projectileAttack.ShootProjectile(0, directionToMouse);
        PlayRecoil();
    }

    private void RotateWeaponTowardsLook()
    {
        if (weaponPos == null)
            return;

        if (mainCamera == null)
            return;

        Vector3 mousePosition = Mouse.current.position.ReadValue();

        mousePosition.z = Mathf.Abs(
            mainCamera.transform.position.z
        );

        Vector3 worldMousePosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        directionToMouse =
            worldMousePosition - weaponPos.position;

        float angle =
            Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg - 90f;
        weaponPos.rotation =
            Quaternion.AngleAxis(angle, Vector3.forward);

        spriteWeapon.localScale =
            directionToMouse.x < 0f
                ? FlipScale
                : NormalScale;
    }
    public void IncreaseShootSpeed(float percent)
    {
        if (percent <= 0f)
            return;

        fireInterval /= (1f + percent / 100f);
    }
    public void ChangeShootSpeed(float time)
    {
        fireInterval = time;
    }
    public void ChangeHoldShoot(bool t)
    {
        fireHold = t;
    }
}