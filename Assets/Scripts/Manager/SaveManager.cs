using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    private static string SAVE_FILE_PATH;
    public DataPlayer dataPlayer;

    void Awake(){
        SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "save-projectcube.json");
    }

    public void Save()
    {
        if (SAVE_FILE_PATH == null) 
            SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "save-projectcube.json");

        Debug.Log("PATH : " + SAVE_FILE_PATH);
        JsonSerializationUtility.SaveToJson(SAVE_FILE_PATH, dataPlayer);
    }

    public void Load()
    {
        if (SAVE_FILE_PATH == null) 
            SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "save-projectcube.json");

        if (File.Exists(SAVE_FILE_PATH))
        {
            dataPlayer = JsonSerializationUtility.LoadFromJson<DataPlayer>(SAVE_FILE_PATH);
            Debug.Log("dataPlayer => " + dataPlayer);
        }
        else
        {
            dataPlayer = new DataPlayer();
            dataPlayer.data = new Dictionary<int, DataLevel>();
            dataPlayer.username = GameConstants.DEFAULT_USERNAME;
            Save();
        }
    }


    public void WriteInConsolePlayerData() {
        Debug.Log("=======================");
        Debug.Log("====== DataLevel ======");
        Debug.Log("username => " + dataPlayer.username);
        for (int i = 0; i < dataPlayer.data.Count; i++) {

            Debug.Log("Level " + (i + 1) + " time : " + dataPlayer.data[i].time);
            Debug.Log("Level " + (i + 1) + " diamond : " + dataPlayer.data[i].nbDiamond);
        }
        Debug.Log("=======================");
    }
}


[Serializable]
public class CustomVector3 {
    public float x;
    public float y;
    public float z;

    public CustomVector3(Vector3 v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 GetVector() {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class CustomVector4 {
    public float x;
    public float y;
    public float z;
    public float w;

    public CustomVector4(Vector4 v) {
        x = v.x;
        y = v.y;
        z = v.z;
        w = v.w;
    }

    public Vector4 GetVector() {
        return new Vector4(x, y, z, w);
    }

    public Quaternion GetQuaternion() {
        return new Quaternion(x, y, z, w);
    }
}

[Serializable]
public class DataPlayer {
    public string username;
    [SerializeField]
    public Dictionary<int, DataLevel> data = new Dictionary<int, DataLevel>();
}

[Serializable]
public class DataLevel {
    public float time;
    public int nbDiamond;
    [SerializeField]
    public List<CustomVector3> position = new List<CustomVector3>();
    [SerializeField]
    public List<CustomVector3> rotation = new List<CustomVector3>();
}