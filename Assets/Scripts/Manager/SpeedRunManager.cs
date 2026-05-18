using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SpeedRunManager : MonoBehaviour
{
    private DataLevel _previousData;

    [SerializeField] GameObject _playerGhostPrefab;
    [SerializeField] GameObject _timer;
    GameManager _gameManager;

    private GameObject _player;
    private GameObject _playerGhost;

    private bool _replay;
    private int _replayId;

    private bool _isActive;

    private float _currentTime;
    private float _time;

    void Awake(){
        _gameManager = GetComponent<GameManager>();
    }

    void Start() {
        _isActive = false;
        if (GameRegistry.Instance == null || GameRegistry.Instance.GetPlayerTransform() == null)
        {
            Debug.LogError("GameRegistry ou PlayerTransform introuvable. SpeedRunManager désactivé.");
            enabled = false;
            return;
        }

        _player = GameRegistry.Instance.GetPlayerTransform().gameObject;

        if (PlayerPrefs.GetInt("SpeedRun") == 1) {
            if (_gameManager.saveManager.dataPlayer == null) {
                _gameManager.saveManager.Load();
            }

            if (_gameManager.saveManager.dataPlayer.data.Count > 0 && _gameManager.saveManager.dataPlayer.data.ContainsKey(_gameManager.currentLevel)) {
                _previousData = _gameManager.saveManager.dataPlayer.data[_gameManager.currentLevel];
                _replay = true;
            } else {
                _previousData = null;
                _replay = false;
            }
        }
    }

    /// <summary>
    /// Démarre l'enregistrement / replay du speedrun pour ce niveau.
    /// Instancie le ghost si des données précédentes existent.
    /// </summary>
    public void LevelStart()
    {
        if (_previousData != null && _playerGhostPrefab != null)
            _playerGhost = Instantiate(_playerGhostPrefab);

        _isActive = true;
        _currentTime = Time.time;
    }

   void FixedUpdate()
    {
        if (_isActive)
        {
            if (_player != null && _gameManager != null && _gameManager.dataLevel != null)
            {
                _gameManager.dataLevel.position.Add(new CustomVector3(_player.transform.position));
            }

            _time = Time.time - _currentTime;
            if (_timer != null)
            {
                var text = _timer.GetComponent<TextMeshProUGUI>();
                if (text != null) text.text = Utils.Functions.FormatTime(_time);
            }
        }

        if (_replay && _previousData != null && _replayId < _previousData.position.Count && _isActive && _playerGhost != null)
        {
            _playerGhost.transform.position = _previousData.position[_replayId].GetVector();
            _replayId += 1;
        }
    }
}