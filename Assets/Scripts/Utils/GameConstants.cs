using UnityEngine;

/// <summary>
/// Centralized game constants to avoid magic strings and hardcoded values.
/// </summary>
public static class GameConstants
{
    // Tags
    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "ennemy";
    public const string TAG_COLLECTIBLE = "Collectible";
    public const string TAG_PROJECTILE = "Projectile";
    public const string TAG_PROJECTILE_PARRY = "ProjectileParry";
    public const string TAG_WALL = "Wall";

    // Object Names
    public const string OBJECT_GAME_MANAGER = "GameManager";
    public const string OBJECT_TO_ACTIVATE = "OBJECT_TO_ACTIVATE";
    public const string OBJECT_BLACK_SCREEN = "BlackScreen";

    // Layers (voir TagManager)
    public const int LAYER_PLAYER = 3;
    public const int LAYER_GROUND = 7;
    public const int LAYER_MOVING_PLATFORM = 10;

    // Physics
    public const float GRAVITY_MULTIPLIER = 9.81f;

    /// <summary>Layer Ground : sol statique + surface du dessus des plateformes.</summary>
    public static readonly LayerMask GroundAndPlatformMask = 1 << LAYER_GROUND;

    // UI/Animation
    public const float CANVAS_FADE_DURATION = 0.75f;

    // Default values
    public const string DEFAULT_USERNAME = "Yruama";
}
