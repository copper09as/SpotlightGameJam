using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Global.Data.BattleConfig
{
    [Serializable]
    public class UserData
    {
        public int unLockLevel;
    }

    public class BattleConfig:MonoBehaviour
    {
        public int levelId;
        public static BattleConfig Instance;
        public UserData userData;
        public bool FlyMode = false;
        public bool EverythingMoveMode = false;
        public bool DragMode = false;
        public bool PoisionMode = false;
        public bool DontDeadMode = false;
        public bool DeadMode = false;
        public int SpecialSfx = 0;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            if (Screen.width < 1366 || Screen.height < 768)
            {
               
#if UNITY_EDITOR
#else
        Application.Quit();
#endif
                return;
            }
            Debug.Log("您的分辨率通过检测！可以正常进行游戏");
           userData = JsonTool.LoadByJson<UserData>(Path.Combine(Application.persistentDataPath, "UserData.json"));
            if (userData == null)
            {
                userData = new UserData();
                userData.unLockLevel = 0;
            }
        }
        public void Win(float delay)
        {
            StartCoroutine(WinWithDelayCoroutine(delay));
        }
        public void UnLockLevel()
        {
            userData.unLockLevel = 7600;
            JsonTool.SaveByJson(Path.Combine(Application.persistentDataPath, "UserData.json"), userData);
        }
        private IEnumerator WinWithDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            //9->10,count ->10,levelId=count,so return StartScene
            if (levelId == userData.unLockLevel)
            {
                userData.unLockLevel += 1;
                JsonTool.SaveByJson(Path.Combine(Application.persistentDataPath, "UserData.json"), userData);
            }
            levelId += 1;
            
            if (levelId < GameConfig.Instance.LevtlDC.levelDataList.Count - 1)
            {
                SceneChangeManager.Instance.LoadScene
               (GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == levelId).ScenePath);
            }
            
            else
            {
                if(DeadMode)
                {
                    NotificationManager.Instance.ShowNotification("恭喜您通关死亡模式！", "恭喜您通关死亡模式！");
                    SceneChangeManager.Instance.LoadScene("StartScene");
                }
                else
                {
                    SceneChangeManager.Instance.LoadScene("StartScene");
                }
              
            }
                
        }

        internal void ClearLevel()
        {
            userData.unLockLevel = 0;
            JsonTool.SaveByJson(Path.Combine(Application.persistentDataPath, "UserData.json"), userData);
        }
    }
}
