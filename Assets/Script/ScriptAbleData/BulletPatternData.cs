using UnityEngine;

public enum BulletPatternType
{
    Circle,
    Spiral,
    DoubleSpiral
}

[CreateAssetMenu(
    fileName = "BulletPattern",
    menuName = "Enemy/Bullet Pattern"
)]
public class BulletPatternData : ScriptableObject
{
    [Header("Pattern")]
    public BulletPatternType patternType;

    [Header("Timing")]
    [Min(0.01f)]
    public float fireInterval = 0.2f;

    [Header("Bullets")]
    [Min(1)]
    public int bulletCount = 8;

    [Header("Rotation")]
    public float angleStep = 10f;

    public float startAngle = 0f;
}