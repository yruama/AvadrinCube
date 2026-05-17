using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGameplayConfig", menuName = "Cube/Player Gameplay Config")]
public class PlayerGameplayConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 15f;

    [Header("Jump")]
    public float maxJumpVelocity = 20f;
    public float minJumpVelocity = 5f;
    public float jumpFeedbackDuration = 0.12f;

    [Header("Physics")]
    public float groundAcceleration = 80f;
    public float groundDeceleration = 90f;
    public float airAcceleration = 40f;
    public float airDeceleration = 30f;
    public float gravityMultiplier = 10f;
    public float groundStickForce = 2f;
    public float platformCheckDistance = 0.8f;
}
