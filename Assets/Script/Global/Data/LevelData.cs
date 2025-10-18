using System;
using System.Collections.Generic;

namespace Global.Data.Level
{
    [Serializable]
    public class LevelData
    {
        public int Id;
        public string SceneName;
        public string SpritePath;
        public string ScenePath;
    }
    [Serializable]
    public class LevelDataCollection
    {
        public List<LevelData> levelDataList = new List<LevelData>();
    }

}
