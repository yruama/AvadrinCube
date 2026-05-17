using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// Utility class for JSON serialization/deserialization operations.
/// </summary>
public static class JsonSerializationUtility
{
    /// <summary>
    /// Saves object to JSON file using JsonUtility.
    /// </summary>
    public static void SaveToJson<T>(string filePath, T data) where T : class
    {
        try
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json, Encoding.UTF8);
            Debug.Log($"Data saved to {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving JSON to {filePath}: {e.Message}");
        }
    }

    /// <summary>
    /// Loads object from JSON file using JsonUtility.
    /// </summary>
    public static T LoadFromJson<T>(string filePath) where T : class, new()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"File not found: {filePath}");
                return new T();
            }

            string json = File.ReadAllText(filePath, Encoding.UTF8);
            T data = JsonUtility.FromJson<T>(json);
            Debug.Log($"Data loaded from {filePath}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading JSON from {filePath}: {e.Message}");
            return new T();
        }
    }

    /// <summary>
    /// Converts Vector3 to CustomVector3 for serialization.
    /// </summary>
    public static CustomVector3 Vector3ToCustom(Vector3 v)
    {
        return new CustomVector3(v);
    }

    /// <summary>
    /// Converts Quaternion to CustomVector4 for serialization.
    /// </summary>
    public static CustomVector4 QuaternionToCustom(Quaternion q)
    {
        return new CustomVector4(new Vector4(q.x, q.y, q.z, q.w));
    }
}
