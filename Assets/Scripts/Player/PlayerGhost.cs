using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Outil debug : enregistre une trajectoire via manette.</summary>
public class PlayerGhost : MonoBehaviour
{
    public bool isTestMode;

    private bool _isRecording;
    private readonly List<Vector3> _recordedPositions = new List<Vector3>();
    private Gamepad _gamepad;

    void Start()
    {
        _gamepad = Gamepad.current;
    }

    void Update()
    {
        if (_gamepad == null)
            return;

        if (_gamepad.leftShoulder.isPressed)
            _isRecording = true;

        if (_gamepad.leftShoulder.wasReleasedThisFrame)
        {
            _isRecording = false;
            SavePositionsToJson();
        }
    }

    void FixedUpdate()
    {
        if (!isTestMode || !_isRecording)
            return;

        _recordedPositions.Add(transform.position);
    }

    private void SavePositionsToJson()
    {
        string jsonPath = Path.Combine(Application.dataPath, "PositionsEnregistrees.json");
        JsonSerializationUtility.SaveToJson(jsonPath, new PositionData(_recordedPositions));
    }
}
