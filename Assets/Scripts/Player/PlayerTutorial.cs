using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorial : MonoBehaviour
{
    public string fileName;

    private bool _canMove;
    private readonly List<Vector3> _positions = new List<Vector3>();
    private int _currentIndex;

    void Start()
    {
        TextAsset json = Resources.Load<TextAsset>("GhostTutorial/" + fileName);
        if (json == null)
        {
            Debug.LogError($"GhostTutorial/{fileName} introuvable dans Resources.");
            return;
        }

        PositionData data = JsonUtility.FromJson<PositionData>(json.text);
        if (data?.positions != null)
            _positions.AddRange(data.positions);
    }

    void FixedUpdate()
    {
        if (!_canMove || _positions.Count == 0)
            return;

        transform.position = _positions[_currentIndex];
        _currentIndex = (_currentIndex + 1) % _positions.Count;
    }

    public void StartMovement() => _canMove = true;
}
