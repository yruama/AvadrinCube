using UnityEngine;

/// <summary>Marqueur sur le collider du dessus d'une plateforme.</summary>
public class MovingPlatformSurface : MonoBehaviour
{
    [SerializeField] private PlatformeController _platform;

    public PlatformeController Platform
    {
        get
        {
            if (_platform == null)
                _platform = GetComponentInParent<PlatformeController>();
            return _platform;
        }
    }

    void Awake()
    {
        if (_platform == null)
            _platform = GetComponentInParent<PlatformeController>();
    }
}
