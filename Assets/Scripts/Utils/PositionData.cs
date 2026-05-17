using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PositionData
{
    public List<Vector3> positions = new List<Vector3>();

    public PositionData() { }

    public PositionData(List<Vector3> source)
    {
        positions = source ?? new List<Vector3>();
    }
}
