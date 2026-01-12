using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class LevelInfo
    {
        public int levelNumber;
        public string levelName;
        public GameObject levelPrefab;
        public Sprite levelThumbnail;
    }

    [Header("All Levels")]
    public LevelInfo[] levels;

    public LevelInfo GetLevel(int levelNumber)
    {
        foreach (var level in levels)
        {
            if (level.levelNumber == levelNumber)
                return level;
        }
        return null;
    }

    public int GetTotalLevels()
    {
        return levels.Length;
    }
}