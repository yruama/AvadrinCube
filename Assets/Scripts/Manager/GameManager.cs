using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Threading.Tasks;

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
        saveManager.Load();
        //saveManager.WriteInConsolePlayerData();

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
        speedRunManager.LevelStart();
    }


    public void LevelStart()
    {
        dataLevel = new DataLevel();
    }

    public void LevelEnd()
    {
        /*_isActive = false;
         //int diamond = _diamondManager.nbDiamond;

         if (_previousData != null) {
             _data = _previousData;
             if (_previousData.time > _data.time)
                 _data.time = _gameManager.TimerLevel;
             if (_previousData.nbDiamond < diamond)
                  _data.nbDiamond = diamond;
         } else {
             _data.time = _gameManager.TimerLevel;
             //_data.nbDiamond = diamond;
         }

         if (SaveManager._data.data.ContainsKey(_gameManager.currentLevel)) {
             SaveManager._data.data[_gameManager.currentLevel] = _data;
         } else {
             SaveManager._data.data.Add(_gameManager.currentLevel, _data);
         }

         SaveManager.Save();*/
    }
}
