using System;
using System.IO;
using UnityEngine;

public class JsonTool
{
    public static void SaveByJson(string path, object data)
    {
        var json = JsonUtility.ToJson(data);
        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"成功存储在{path}");
        }
        catch (System.Exception exception)
        {
            Debug.Log($"在{path}存储失败\n {exception}");
        }
    }
    public static T LoadByJson<T>(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);
            Debug.Log($"成功在{path}读取");
            return data;
        }
        catch(Exception ex)
        {
            Debug.Log($"在{path}读取失败"+ex.Message);
        }
        return default(T);
    }
    public static void DeleteFile(string saveFilename)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFilename);
        try { File.Delete(path); }
        catch (System.Exception exception)
        {
            Debug.Log($"在{path}删除失败\n {exception}");
        }
    }
}
