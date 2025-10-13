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
            Debug.Log($"�ɹ��洢��{path}");
        }
        catch (System.Exception exception)
        {
            Debug.Log($"��{path}�洢ʧ��\n {exception}");
        }
    }
    public static T LoadByJson<T>(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);
            Debug.Log($"�ɹ���{path}��ȡ");
            return data;
        }
        catch(Exception ex)
        {
            Debug.Log($"��{path}��ȡʧ��"+ex.Message);
        }
        return default(T);
    }
    public static void DeleteFile(string saveFilename)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFilename);
        try { File.Delete(path); }
        catch (System.Exception exception)
        {
            Debug.Log($"��{path}ɾ��ʧ��\n {exception}");
        }
    }
}
