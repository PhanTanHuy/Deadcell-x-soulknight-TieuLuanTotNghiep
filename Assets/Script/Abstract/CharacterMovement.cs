using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class CharacterMovement : MonoBehaviour
{
    protected Rigidbody2D rb;

    private float timeForBuff;
    private bool movementEnabled = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (!movementEnabled)
            return;

        if (timeForBuff > 0f)
        {
            timeForBuff -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Move();
    }

    protected abstract void Move();

    public virtual void SetTimeForBuff(float time)
    {
        timeForBuff = Mathf.Max(timeForBuff, time);
    }

    public virtual void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
            rb.linearVelocity = Vector2.zero;
    }

    public bool IsMovementEnabled()
    {
        return movementEnabled;
    }

    public bool IsBuffLocked()
    {
        return timeForBuff > 0f;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}