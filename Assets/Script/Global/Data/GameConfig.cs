using System;
using System.IO;
using Assets.Script.Global.Data;
using Global.Data.Entity;
using Global.Data.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Global.Data
{
    public class GameConfig : MonoBehaviour
    {
        public static GameConfig Instance { get; private set; }
        public CommonEntityDataCollection CommonEDC;
        public EntityScriptDataCollection EntitySDC;
        public LevelDataCollection LevtlDC;
        public UserConfigData UserCD;
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
            Debug.Log("查看" + EntitySDC.entityScriptList.Count);
        }

        private void LoadAllConfig()
        {
            CommonEDC = LoadConfigData<CommonEntityDataCollection>("CommonEntityData.json");
            EntitySDC = LoadConfigData<EntityScriptDataCollection>("EntityScriptData.json");
            LevtlDC = LoadConfigData<LevelDataCollection>("LevelData.json");
            UserCD = JsonTool.LoadByJson<UserConfigData>(Path.Combine(Application.streamingAssetsPath, "UserConfigData.json"));
            if (UserCD == null)
            {
                UserCD = new UserConfigData();
                SaveUserConfig(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen);
            }
            Screen.SetResolution(UserCD.ResolutionX, UserCD.ResolutionY, UserCD.isFullScreen);
        }
        public void SaveUserConfig(int x,int y,bool fulls)
        {
            UserCD.ResolutionX =x ;
            UserCD.ResolutionY = y;
            UserCD.isFullScreen = fulls;
            JsonTool.SaveByJson(Path.Combine(Application.streamingAssetsPath, "UserConfigData.json"), UserCD);
        }
        public void SetResolution()
        {
            Screen.SetResolution(UserCD.ResolutionX, UserCD.ResolutionY, UserCD.isFullScreen);
        }
        private void ApplyUserResolution()
        {
            // 确保分辨率应用
            if (UserCD != null)
            {
                Screen.SetResolution(UserCD.ResolutionX, UserCD.ResolutionY, UserCD.isFullScreen);
                Debug.Log($"已应用分辨率: {UserCD.ResolutionX}x{UserCD.ResolutionY}, 全屏:{UserCD.isFullScreen}");
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplyUserResolution(); // 确保每次场景切换后都应用分辨率
        }


        private T LoadConfigData<T>(string relativePath)
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
