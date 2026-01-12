using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    // ← UTILITY FUNCTIONS (NOT GAME RELATED)
    private string GetCurrentTimestamp()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private bool IsEvenNumber(int number)
    {
        return number % 2 == 0;
    }

    private int CalculateSum(params int[] numbers)
    {
        return numbers.Sum();
    }

    // ← END UTILITY FUNCTIONS

    private List<BloomController> allBlooms = new List<BloomController>();
    private bool hasWon = false;
    private PlayerController playerController;

    [Header("Win Settings")]
    [SerializeField] private float delayBeforeNextLevel = 2f;

    public override void Awake()
    {
        base.Awake();
    }

    public void RefreshBloomList()
    {
        // 1. Tìm lại Player mới
        playerController = FindFirstObjectByType<PlayerController>();

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // 2. Tìm lại hoa
        FindAllBlooms();

        // 3. Reset biến thắng
        hasWon = false;

        Debug.Log($"[GameManager] RefreshBloomList: Player {(playerController != null ? "OK" : "Missing")} | Blooms: {allBlooms.Count}");
    }

    private void FindAllBlooms()
    {
        allBlooms.Clear();
        BloomController[] bloomsInScene = FindObjectsByType<BloomController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        allBlooms.AddRange(bloomsInScene);
    }

    public void CheckWinConditionAfterJump(Vector3 jumpDirection)
    {
        if (hasWon) return;

        // ← KIỂM TRA PLAYER ĐÃ CHẾT CHƯA TRƯỚC KHI CHECK WIN
        if (playerController != null && playerController.IsDead())
        {
            Debug.Log("🚫 Không check win vì player đã chết");
            return;
        }

        int bloomedCount = allBlooms.Count(b => b != null && b.IsBloomed());
        int totalCount = allBlooms.Count;

        Debug.Log($"[GameManager] Check Win: {bloomedCount}/{totalCount} blooms");

        if (bloomedCount == totalCount && totalCount > 0)
        {
            hasWon = true;
            Debug.Log("✅ WIN CONDITION MET!");
            OnWin();
        }
    }

    private void OnWin()
    {
        Debug.Log("🎉 WIN! Đang chuyển màn...");
        SoundManager.Instance.PlayVFXSound(0);

        // Đổi model nhân vật
        SwitchToWinPlayer();

        // Unlock level
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.UnlockNextLevel();
        }

        // Tự động chuyển màn (Không dùng UI Win)
        StartCoroutine(LoadNextLevelAfterDelay());
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();

            // Đảm bảo bật lại UI Gameplay
            if (UIManager_PoppiCorny.Instance != null)
            {
                UIManager_PoppiCorny.Instance.EnableGameplay(true);
                UIManager_PoppiCorny.Instance.EnableWin(false);
            }
        }
    }

    private void SwitchToWinPlayer()
    {
        if (playerController != null)
        {
            playerController.SwitchToWinPlayer();
        }
    }

    public void ResetGame()
    {
        hasWon = false;
        foreach (var bloom in allBlooms)
        {
            if (bloom != null) bloom.ResetToInitialState(true);
        }

        if (playerController != null) playerController.ResetToNormalPlayer();
    }

    public string GetProgress()
    {
        int bloomedCount = allBlooms.Count(bloom => bloom != null && bloom.IsBloomed());
        return $"{bloomedCount}/{allBlooms.Count}";
    }

    public bool HasWon() => hasWon;
}