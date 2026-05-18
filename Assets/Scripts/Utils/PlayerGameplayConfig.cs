using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGameplayConfig", menuName = "Cube/Player Gameplay Config")]
public class PlayerGameplayConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Jump")]
    public float maxJumpVelocity = 6.25f;
    public float minJumpVelocity = 3.5f;
    public float jumpFeedbackDuration = 0.12f;

    [Header("Physics")]
    public float groundAcceleration = 80f;
    public float groundDeceleration = 90f;
    public float airAcceleration = 40f;
    public float airDeceleration = 30f;
    public float gravityMultiplier = 25f;
    public float groundStickForce = 2f;
    public float platformCheckDistance = 0.8f;
}
