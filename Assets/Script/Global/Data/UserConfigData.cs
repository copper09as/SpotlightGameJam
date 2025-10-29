using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Global.Data
{
    [System.Serializable]
    public class UserConfigData
    {
        public int ResolutionX;
        public int ResolutionY;
        public bool isFullScreen;
        public int TargetFrameRate;
        public bool vSync;
    }
}
