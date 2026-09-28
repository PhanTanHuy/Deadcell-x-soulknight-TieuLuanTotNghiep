using System.Collections;
using UnityEngine;

public class CharacterBuff : BuffOnSelf
{
    private CharacterMovement characterMovement;
    private Rigidbody2D rb;

    private Coroutine dashCoroutine;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void DashBuff(Vector2 targetPosition, float time)
    {
        if (characterMovement == null)
            return;

        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }

        dashCoroutine = StartCoroutine(
            DashToPosition(targetPosition, time)
        );
    }

    private IEnumerator DashToPosition(
        Vector2 targetPosition,
        float time
    )
    {
        if (time <= 0f)
        {
            rb.MovePosition(targetPosition);
            yield break;
        }

        characterMovement.SetTimeForBuff(time);

        rb.linearVelocity = Vector2.zero;

        Vector2 startPosition = rb.position;

        float timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;

            float t = timer / time;

            rb.MovePosition(
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    t
                )
            );

            yield return null;
        }

        rb.MovePosition(targetPosition);

        rb.linearVelocity = Vector2.zero;

        dashCoroutine = null;
    }

    public override void TeleportBuff(Vector2 position)
    {
        if (rb == null)
            return;

        rb.position = position;
        rb.linearVelocity = Vector2.zero;

        Debug.Log(
            $"{gameObject.name} Teleport Buff: Position = {position}"
        );
    }

    private void OnDisable()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }
    }
}