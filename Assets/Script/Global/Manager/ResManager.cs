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
        yield return handle; // 等待异步加载完成

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
        // 先检查缓存
        if (cache.TryGetValue(fullPath, out Sprite cachedObj))
        {
            return cachedObj;
        }

        // 尝试 StreamingAssets
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
            Debug.LogWarning($"StreamingAssets 文件不存在: {fullPath}, 尝试从 Resources 加载...");
        }

        // 尝试 Resources
        string resourcePath = Path.ChangeExtension(path, null);
        var resource = Resources.Load<Sprite>(resourcePath);
        if (resource != null)
        {
            cache[fullPath] = resource; 
            return resource;
        }
        Debug.LogError($"文件在 StreamingAssets 和 Resources 中都不存在: {path}");
        return default;
    }
}
