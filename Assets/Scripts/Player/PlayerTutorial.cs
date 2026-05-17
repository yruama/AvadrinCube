using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PlayerTutorial : MonoBehaviour
{
    private bool _canMove = false;
    public string fileName;
    private List<Vector3> positionsEnregistrees = new List<Vector3>();
    private int currentPosition = 0;
    // Start is called before the first frame update
    void Start()
    {
        // Utiliser Resources.Load pour charger le fichier JSON
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("GhostTutorial/" + fileName);

        // Vérifier si le fichier JSON existe
        if (jsonTextAsset != null)
        {
            // Lire le contenu du fichier JSON
            PositionData positionData = JsonUtility.FromJson<PositionData>(jsonTextAsset.text);
            positionsEnregistrees = positionData.positions;
        }
        else
        {
            Debug.LogError("Le fichier JSON n'existe pas.");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (positionsEnregistrees.Count == 0 || !_canMove) return;

        transform.position = positionsEnregistrees[currentPosition];
        currentPosition += 1;
        if (currentPosition == positionsEnregistrees.Count) currentPosition = 0;
    }

    public void StartMovement() {
        _canMove = true;
    }
}
