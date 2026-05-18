using UnityEngine;

[CreateAssetMenu(fileName = "PlatformMotionConfig", menuName = "Cube/Platform Motion Config")]
/// <summary>
/// Configuration réutilisable pour le mouvement des plateformes (durée, attente).
/// </summary>
public class PlatformMotionConfig : ScriptableObject
{
    public float waitTime = 1f;
    public float timeToReachNextPoint = 5f;
}
