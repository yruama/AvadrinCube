using UnityEngine;

/// <summary>
/// Central registry for important game objects.
/// This eliminates the need for GameObject.Find() calls and provides proper null safety.
/// </summary>
public class GameRegistry : MonoBehaviour
{
    private static GameRegistry _instance;
    public static GameRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameRegistry>();
                if (_instance == null)
                {
                    Debug.LogError("GameRegistry not found in scene. Please add it to the scene.");
                }
            }
            return _instance;
        }
    }

    [Header("Player References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerController playerController;

    [Header("Game Managers")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private DiamondManager diamondManager;
    [SerializeField] private SpeedRunManager speedRunManager;
    [SerializeField] private SoundManager soundManager;

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private CanvasGroup fadeCanvas;

    [Header("Game Objects")]
    [SerializeField] private GameObject objectToActivate;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        ValidateReferences();
    }

    /// <summary>
    /// Validates that all necessary references are assigned.
    /// </summary>
    private void ValidateReferences()
    {
        if (playerTransform == null)
            Debug.LogWarning("Player Transform not assigned in GameRegistry");
        if (playerController == null)
            Debug.LogWarning("Player Controller not assigned in GameRegistry");
        if (gameManager == null)
            Debug.LogWarning("Game Manager not assigned in GameRegistry");
        if (blackScreen == null)
            Debug.LogWarning("Black Screen not assigned in GameRegistry");
    }

    // Player accessors
    public Transform GetPlayerTransform() => playerTransform;
    public PlayerController GetPlayerController() => playerController;

    // Manager accessors
    public GameManager GetGameManager() => gameManager;
    public SaveManager GetSaveManager() => saveManager;
    public DiamondManager GetDiamondManager() => diamondManager;
    public SpeedRunManager GetSpeedRunManager() => speedRunManager;
    public SoundManager GetSoundManager() => soundManager;

    // UI accessors
    public CanvasGroup GetBlackScreen() => blackScreen;
    public CanvasGroup GetFadeCanvas() => fadeCanvas;

    // Game object accessors
    public GameObject GetObjectToActivate() => objectToActivate;
}
