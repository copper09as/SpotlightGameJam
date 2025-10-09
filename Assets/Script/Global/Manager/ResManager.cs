using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ResManager
{
    private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
    public static T LoadDataByAsset<T>(string path)
    {
        var handle = Addressables.LoadAssetAsync<T>(path);

        handle.WaitForCompletion();

        var obj = handle.Result;

        return obj;
    }
    /// <summary>
    /// ���ڼ���sprite��Դ
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(string path)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, path);
        if (cache.TryGetValue(fullPath, out Sprite cachedObj))
        {
            return cachedObj;
        }

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
