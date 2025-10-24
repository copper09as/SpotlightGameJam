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
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
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
            levelId += 1;
            //9->10,count ->10,levelId=count,so return StartScene
            if (levelId == userData.unLockLevel)
            {
                userData.unLockLevel += 1;
                JsonTool.SaveByJson(Path.Combine(Application.persistentDataPath, "UserData.json"), userData);
            }
            if (levelId < GameConfig.Instance.LevtlDC.levelDataList.Count - 1)
            {
                SceneChangeManager.Instance.LoadScene
               (GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == levelId).ScenePath);
            }
            else
                SceneChangeManager.Instance.LoadScene("StartScene");
        }
    }
}
