using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float maxHearingDistance = 10f;
    public float minHearingDistance = 5f;

    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject playerObject = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
        if (playerObject == null)
        {
            Debug.LogError($"Player with tag '{GameConstants.TAG_PLAYER}' not found. SoundManager will be disabled.");
            enabled = false;
            return;
        }
        player = playerObject.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Mesurer la distance entre le joueur et la source du son
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Utiliser l'interpolation linéaire pour calculer le volume en fonction de la distance
        float normalizedDistance = Mathf.Clamp01((distanceToPlayer - minHearingDistance) / (maxHearingDistance - minHearingDistance));
        float volume = 1f - normalizedDistance; // Inverser la distance pour que le volume soit plus fort quand le joueur est plus proche

        // Ajuster le volume de l'AudioSource
        audioSource.volume = Mathf.Clamp01(volume);
    }
}