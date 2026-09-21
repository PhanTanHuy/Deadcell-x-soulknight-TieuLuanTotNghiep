using UnityEngine;

[CreateAssetMenu(
    fileName = "MouseChaseData",
    menuName = "AI/Mouse/Chase Data"
)]
public class MouseChaseData : ScriptableObject
{
    [Header("Chase")]
    [Min(0f)]
    public float chaseRadius = 5f;

    [Min(0f)]
    public float stopDistance = 0.5f;
    public float crazyDetectionRadius = 5f;
    public float crazyChaseSpeed = 5f;

}