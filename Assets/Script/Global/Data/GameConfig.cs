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
        public CommonEntityDataCollection CommonEDC { get; private set; }
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
            /*CommonEDC = new CommonEntityDataCollection();
            for(int i = 0;i<10;i++)
            {
                CommonEntityData commonEntityData = new CommonEntityData();
                CharacterEntityData characterEntityData = new CharacterEntityData();
                characterEntityData.signId = -1;
                commonEntityData.id = 20000 + i;
                commonEntityData.EffectId = 10000;
                commonEntityData.ScaleX = 1.0f;
                commonEntityData.ScaleY = 1.0f;
                commonEntityData.CharacterData = characterEntityData.Copy();
                CommonEDC.CommonEntityList.Add(commonEntityData);
            }
            JsonTool.SaveByJson(Path.Combine(Application.streamingAssetsPath, "CommonEntityData.json"),CommonEDC);*/
            CommonEDC = LoadConfigData<CommonEntityDataCollection>("CommonEntityData.json");
            EntitySDC = LoadConfigData<EntityScriptDataCollection>("EntityScriptData.json");
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
