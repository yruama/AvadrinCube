using UnityEngine;

[CreateAssetMenu(fileName = "PlatformMotionConfig", menuName = "Cube/Platform Motion Config")]
public class PlatformMotionConfig : ScriptableObject
{
    public float waitTime = 1f;
    public float timeToReachNextPoint = 5f;
}
