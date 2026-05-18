using UnityEngine;

/// <summary>
/// Gère le volume d'une source audio en fonction de la distance au joueur.
/// Utilise le <see cref="GameRegistry"/> pour récupérer le joueur si possible.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public float maxHearingDistance = 10f;
    public float minHearingDistance = 5f;

    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("SoundManager nécessite un AudioSource sur le même GameObject. Composant introuvable.");
            enabled = false;
            return;
        }

        if (GameRegistry.Instance != null && GameRegistry.Instance.GetPlayerTransform() != null)
        {
            player = GameRegistry.Instance.GetPlayerTransform();
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
            if (playerObject == null)
            {
                Debug.LogError($"Player with tag '{GameConstants.TAG_PLAYER}' not found. SoundManager will be disabled.");
                enabled = false;
                return;
            }
            player = playerObject.transform;
        }
    }

    /// <summary>
    /// Met à jour le volume en fonction de la distance joueur->source.
    /// </summary>
    void Update()
    {
        if (player == null || audioSource == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float denom = Mathf.Max(0.0001f, (maxHearingDistance - minHearingDistance));
        float normalizedDistance = Mathf.Clamp01((distanceToPlayer - minHearingDistance) / denom);
        float volume = 1f - normalizedDistance;

        audioSource.volume = Mathf.Clamp01(volume);
    }
}