using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

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
        public void Win()
        {
            if (levelId == userData.unLockLevel)
            {
                userData.unLockLevel += 1;
                JsonTool.SaveByJson(Path.Combine(Application.persistentDataPath, "UserData.json"), userData);
            }
                
        }
    }
}
