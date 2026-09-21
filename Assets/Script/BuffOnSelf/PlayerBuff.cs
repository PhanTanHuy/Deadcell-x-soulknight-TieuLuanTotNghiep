using System.Collections;
using UnityEngine;

public class PlayerBuff : BuffOnSelf
{
    private Rigidbody2D rb;
    private PlayerController playerController;

    private Coroutine dashCoroutine;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void DashBuff(Vector2 targetPosition, float time)
    {
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
        playerController.SetTimeForBuff(time);

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
        Debug.Log(
            $"Player Teleport Buff: Position = {position}"
        );
    }
}