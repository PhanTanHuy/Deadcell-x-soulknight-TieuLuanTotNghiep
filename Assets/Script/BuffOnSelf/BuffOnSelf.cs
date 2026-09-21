using UnityEngine;

public abstract class BuffOnSelf : MonoBehaviour
{
    public virtual void DashBuff(Vector2 direction, float velocity)
    {
        // Implement the logic for the dash buff here
    }
    public virtual void TeleportBuff(Vector2 position)
    {
        // Implement the logic for the teleport buff here
    }
}
