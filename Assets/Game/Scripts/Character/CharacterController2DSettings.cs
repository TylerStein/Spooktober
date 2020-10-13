using UnityEngine;

[CreateAssetMenu(fileName = "CharacterController2D Settings", menuName = "Custom/CC2D Settings", order = 0)]
public class CharacterController2DSettings : ScriptableObject
{
    [Header("Abilities")]
    public bool canJump = true;

    [Header("Layers")]
    public LayerMask collisionLayerMask = 0x0;
    public float skinWidth = 0.5f;

    [Header("Ground Movement")]
    public float maxGroundVelocityX = 6f;
    public float groundAcceleration = 60f;
    public float groundBraking = 120f;
    public float groundFriction = 80f;

    [Header("Stairs Movement")]
    public float maxStairsVelocityX = 2f;
    public float stairsAcceleration = 60f;
    public float stairsBraking = 120f;
    public float stairsFriction = 100f;

    [Header("Air Movement")]
    public float maxAirVelocityX = 6f;
    public float airAccelerationX = 60f;
    public float airBraking = 80f;
    public float airFriction = 40f;

    public float fallAcceleration = 120f;
    public float maxFallVelocity = 26f;

    public float jumpVelocity = 26f;
    public float jumpRiseTime = 0.12f;
    public float maxJumpRiseTime = 0.16f;
}
