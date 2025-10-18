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
    [Serializable]
    public class LevelPreData
    {
        public int Id;
        public List<string> DecoratePath;
        public List<String> CanvaEntitiesPath;
        public List<string> WorldEntitiesPath;
        public string TileMapPath;
    }
}
