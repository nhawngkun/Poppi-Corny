using UnityEngine;
using System.Collections;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Level Data")]
    [SerializeField] private LevelData levelData;
    
    [Header("Level Container")]
    [SerializeField] private Transform levelContainer;
    
    private GameObject currentLevelInstance;
    private int currentLevelNumber = 1;

    public int CurrentLevelNumber => currentLevelNumber;

    public override void Awake()
    {
        base.Awake();
        // Lấy level đã lưu, nếu chưa có thì mặc định là 1
        currentLevelNumber = PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    public LevelData GetLevelData()
    {
        return levelData;
    }

    public void LoadLevel(int levelNumber)
    {
        // 1. Xóa level cũ
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        // 2. Chạy Coroutine đợi 1 frame để xóa hẳn rồi mới tạo cái mới
        StartCoroutine(LoadLevelRoutine(levelNumber));
    }

    private IEnumerator LoadLevelRoutine(int levelNumber)
    {
        // Đợi 1 nhịp để Unity dọn dẹp rác
        yield return null;

        LevelData.LevelInfo levelInfo = levelData.GetLevel(levelNumber);
        
        if (levelInfo == null || levelInfo.levelPrefab == null)
        {
            Debug.LogError($"Level {levelNumber} chưa được thiết lập trong LevelData!");
            yield break;
        }

        // 3. Tạo Level mới
        currentLevelInstance = Instantiate(levelInfo.levelPrefab, levelContainer);
        currentLevelNumber = levelNumber;

        // Lưu lại level hiện tại
        PlayerPrefs.SetInt("CurrentLevel", currentLevelNumber);
        PlayerPrefs.Save();

        // Đợi thêm 1 nhịp để các script trong Level mới khởi động
        yield return null;

        // 4. Báo GameManager quét lại dữ liệu
        // --- ĐÂY LÀ DÒNG ĐÃ SỬA LỖI ---
        if (GameManager.Instance != null)
        {
            // Gọi đúng tên hàm RefreshBloomList (không gọi RefreshGameData nữa)
            GameManager.Instance.RefreshBloomList();
        }
        // -----------------------------

        Debug.Log($"Đã load Level {levelNumber}: {levelInfo.levelName}");
    }

    public void LoadNextLevel()
    {
        int nextLevel = currentLevelNumber + 1;
        
        // Kiểm tra xem đã hết màn chưa
        if (nextLevel > levelData.GetTotalLevels())
        {
            Debug.Log("Đã phá đảo toàn bộ game!");
            return;
        }

        LoadLevel(nextLevel);
    }

    public void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int nextLevel = currentLevelNumber + 1;
        
        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
            Debug.Log($"Đã mở khóa Level {nextLevel}");
        }
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(currentLevelNumber);
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        return levelNumber <= unlockedLevel;
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        return levelNumber < unlockedLevel;
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();
        Debug.Log("Reset progress thành công");
    }
}