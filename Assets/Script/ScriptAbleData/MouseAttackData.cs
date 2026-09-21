using UnityEngine;

public enum MouseAttackType
{
    Normal,
    Pattern
}

[CreateAssetMenu(
    fileName = "MouseAttackData",
    menuName = "Enemy/Mouse Attack Data"
)]
public class MouseAttackData : ScriptableObject
{
    [Header("Attack Type")]
    public MouseAttackType attackType = MouseAttackType.Normal;

    [Header("Projectile")]
    public int projectileIndex = 0;

    [Header("Normal Attack")]
    [Min(0f)]
    public float fireCooldown = 1f;

    [Min(0f)]
    public float aimThreshold = 10f;

    [Header("Hold Projectile")]
    [Min(0f)]
    public float holdDelay = 0f;

    [Header("Bullet Pattern")]
    public BulletPatternData patternData;
}