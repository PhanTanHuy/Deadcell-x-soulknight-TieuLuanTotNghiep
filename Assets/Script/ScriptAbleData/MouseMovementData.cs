using UnityEngine;

[CreateAssetMenu(
    fileName = "MouseMovementData",
    menuName = "AI/Mouse/Movement Data"
)]
public class MouseMovementData : ScriptableObject
{
    [Header("Movement")]
    [Min(0f)]
    public float moveSpeed = 2f;

    [Header("Random Movement")]
    [Min(0f)]
    public float randomDirectionStrength = 1.5f;

    [Min(0.01f)]
    public float directionChangeMinTime = 0.3f;

    [Min(0.01f)]
    public float directionChangeMaxTime = 1.2f;

    [Header("Movement Smoothing")]
    [Min(0f)]
    public float directionSmooth = 5f;
}