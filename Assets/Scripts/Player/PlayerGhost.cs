using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem;

public class PlayerGhost : MonoBehaviour
{
    public bool isTestMode = false;

    private bool savePosition = false;

    private List<Vector3> positionsEnregistrees = new List<Vector3>();
    private Gamepad _gamepad;

    void Start() {
        _gamepad = Gamepad.current;
    }

    void Update() {

        if (_gamepad.leftShoulder.IsPressed()) {
            savePosition = true;
        }

        if (_gamepad.leftShoulder.wasReleasedThisFrame) {
            savePosition = false;
            // Sauvegardez les positions dans un fichier JSON
            SauvegarderPositionsEnJSON();
        }
    }

    void FixedUpdate()
    {
        Debug.Log("Coucou");
        if (isTestMode) {
            if (savePosition) {
                positionsEnregistrees.Add(transform.position);
            }
        }
        
    }

    private void SauvegarderPositionsEnJSON()
    {
        Debug.Log("oui !");
        string jsonPath = "Assets/PositionsEnregistrees.json"; // Chemin du fichier JSON, ajustez selon vos besoins

        // Convertissez la liste de positions en JSON
        string positionsJSON = JsonUtility.ToJson(new PositionData(positionsEnregistrees));

        // Écrivez le JSON dans un fichier
        File.WriteAllText(jsonPath, positionsJSON);
    }
}

[System.Serializable]
public class PositionData
{
    public List<Vector3> positions;

    public PositionData(List<Vector3> positions)
    {
        this.positions = positions;
    }
}