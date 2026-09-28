using System.Collections;
using UnityEngine;

public class BulletMinato : ProjectileMovement
{
    private CharacterBuff playerBuff;

    private Coroutine stopCoroutine;
    [SerializeField] private float stopDuration = 0.15f, dashDuration = 0.35f;
    protected override void Awake()
    {
        base.Awake();

        playerBuff = projectileAttack.attacker.GetComponent<CharacterBuff>();
    }

    private void OnEnable()
    {
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
        }

        stopCoroutine = StartCoroutine(StopAndDash());
    }

    private IEnumerator StopAndDash()
    {
        yield return new WaitForSeconds(stopDuration);

        StopMovement();
        if (playerBuff != null)
        {
            playerBuff.DashBuff(transform.position, dashDuration);
            PoolObject.instance.SpawnDamageAbleObject(PoolObject.VFXType.FanObject, playerBuff.transform, dashDuration);
        }

        stopCoroutine = null;
    }

    private void OnDisable()
    {
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }
    }
}