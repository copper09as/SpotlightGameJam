using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ResManager
{
    private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static IEnumerator LoadPrefabAsync(string path, Transform parent, System.Action<GameObject> callback)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(path);
        yield return handle; // �ȴ��첽�������

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            var obj = Object.Instantiate(handle.Result, parent);
            callback?.Invoke(obj);
        }
        else
        {
            Debug.LogError("Failed to load prefab: " + path);
            callback?.Invoke(null);
        }
    }
    public static GameObject LoadPrefab(string path)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(path);

        handle.WaitForCompletion();

        var obj = handle.Result;

        return obj;
    }
    public static T LoadDataByAsset<T>(string path)
    {
        var handle = Addressables.LoadAssetAsync<T>(path);

        handle.WaitForCompletion();

        var obj = handle.Result;

        return obj;
    }
    
    public static Sprite LoadSprite(string path)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, path);
        // �ȼ�黺��
        if (cache.TryGetValue(fullPath, out Sprite cachedObj))
        {
            return cachedObj;
        }

        // ���� StreamingAssets
        if (File.Exists(fullPath))
        {
                byte[] fileData = File.ReadAllBytes(fullPath);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.LoadImage(fileData);

                Sprite sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );

                cache[fullPath] = sprite;
                return sprite;
        }
        else
        {
            Debug.LogWarning($"StreamingAssets �ļ�������: {fullPath}, ���Դ� Resources ����...");
        }

        // ���� Resources
        string resourcePath = Path.ChangeExtension(path, null);
        var resource = Resources.Load<Sprite>(resourcePath);
        if (resource != null)
        {
            cache[fullPath] = resource; 
            return resource;
        }
        Debug.LogError($"�ļ��� StreamingAssets �� Resources �ж�������: {path}");
        return default;
    }
}
