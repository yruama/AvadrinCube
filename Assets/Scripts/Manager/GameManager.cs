using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Threading.Tasks;

/// <summary>
/// Coordonne des fonctionnalités globales du jeu (chargement, niveau, gestion d'UI liée au démarrage).
/// Les dépendances (SaveManager, SpeedRunManager) sont récupérées depuis l'inspecteur ou le <see cref="GameRegistry"/> si possible.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] CanvasGroup _fade;

    public CanvasGroup canvas;

    public int currentLevel;

    public SaveManager saveManager;
    public SpeedRunManager speedRunManager;

    [HideInInspector]
    public DataLevel dataLevel;

    private bool _levelRunning;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (saveManager == null && GameRegistry.Instance != null)
            saveManager = GameRegistry.Instance.GetSaveManager();

        if (saveManager != null)
        {
            saveManager.Load();
        }
        else
        {
            Debug.LogWarning("SaveManager non assigné et introuvable dans GameRegistry. Chargement ignoré.");
        }

        if (speedRunManager == null && GameRegistry.Instance != null)
            speedRunManager = GameRegistry.Instance.GetSpeedRunManager();

        Control _control = new Control();
        InputAction moveAction = _control.Player.Move;
        moveAction.Enable();
        moveAction.performed += OnActionTriggered;
    }

    void Update()
    {

    }

    async void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (_levelRunning) return;

        _levelRunning = true;
        await Utils.Functions.HideCanvasGroup(canvas);
        if (speedRunManager != null)
            speedRunManager.LevelStart();
        else
            Debug.LogWarning("SpeedRunManager introuvable lors du démarrage du niveau.");
    }

    /// <summary>
    /// Initialise l'enregistrement de la session de niveau.
    /// </summary>
    public void LevelStart()
    {
        dataLevel = new DataLevel();
    }

    public void LevelEnd()
    {
    }
}
