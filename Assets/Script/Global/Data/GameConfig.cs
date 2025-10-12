using System;
using Global.Data.Entity;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
namespace Global.Data
{
    public class GameConfig : MonoBehaviour
    {
        public static GameConfig Instance { get; private set; }
        public CharacterData CharacterDT;
        public EntityScriptDataCollection EntitySDC { get; private set; }
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllConfig();

        }

        private void LoadAllConfig()
        {
            CharacterDT = LoadConfigData<CharacterData>("CharacterData.json");
            EntitySDC = LoadConfigData<EntityScriptDataCollection>("EntityScriptData.json");
            /*EntitySDC = new EntityScriptDataCollection();
            for (int i = 0; i < 10; i++)
            {
                EntityScriptData data = new EntityScriptData();
                data.id = 10000 + i;
                data.InitPath.Add("1");
                data.InitPath.Add("2");
                data.UpdatePath.Add("3");
                data.UpdatePath.Add("4");
                data.OnDestroyPath.Add("4");
                data.OnDestroyPath.Add("4");
                data.OnMouseDownPath.Add("4");
                data.OnMouseDownPath.Add("4");
                data.OnCollisionPath.Add("123");
                data.OnCollisionPath.Add("123");
                EntitySDC.entityScriptList.Add(data);
            }
            JsonTool.SaveByJson(Path.Combine(Application.streamingAssetsPath, "EntityScriptData.json"),EntitySDC);
            */
            
        }
        private static T LoadConfigData<T>(string relativePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
            Debug.Log(fullPath);
            if (File.Exists(fullPath))
            {
                T data = JsonTool.LoadByJson<T>(fullPath);
                return data;
            }
            else
            {
                Debug.LogWarning($"StreamingAssets 文件不存在: {fullPath}, 尝试从 Resources 加载...");
            }
            string resourcePath = Path.ChangeExtension(relativePath, null); // 去掉扩展名
            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset != null)
            {
                try
                {
                    T data = JsonUtility.FromJson<T>(textAsset.text);
                    Debug.Log($"成功在 Resources 读取: {resourcePath}");
                    return data;
                }
                catch (Exception e)
                {
                    Debug.LogError($"在 Resources 解析 JSON 失败: {resourcePath}\n{e}");
                }
            }
            else
            {
                Debug.LogError($"文件在 StreamingAssets 和 Resources 中都不存在: {relativePath}");
            }

            return default;
        }
    }
}
