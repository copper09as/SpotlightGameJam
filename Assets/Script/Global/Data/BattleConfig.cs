using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Global.Data.BattleConfig
{
    public class BattleConfig:MonoBehaviour
    {
        public int levelId;
        public static BattleConfig Instance;
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

        }
    }
}
