using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private GameObject unlockImage;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject completedTick;
    [SerializeField] private Text levelText;

    private int levelNumber;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Setup(int levelNum)
    {
        levelNumber = levelNum;
        UpdateButtonState();
    }

    // ← HÀM MỚI: REFRESH LẠI TRẠNG THÁI BUTTON
    public void RefreshState()
    {
        UpdateButtonState();
    }

    // ← TÁCH LOGIC CẬP NHẬT RA HÀM RIÊNG
    private void UpdateButtonState()
    {
        if (LevelManager.Instance == null) return;

        if (levelText != null)
        {
            levelText.text = levelNumber.ToString();
        }

        bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(levelNumber);
        bool isCompleted = LevelManager.Instance.IsLevelCompleted(levelNumber);

        if (unlockImage != null)
            unlockImage.SetActive(isUnlocked);

        if (lockImage != null)
            lockImage.SetActive(!isUnlocked);

        if (completedTick != null)
            completedTick.SetActive(isCompleted);

        if (button != null)
        {
            button.interactable = isUnlocked;
            button.onClick.RemoveAllListeners();

            if (isUnlocked)
            {
                button.onClick.AddListener(OnLevelButtonClicked);
            }
        }

        Debug.Log($"[LevelButton] Level {levelNumber}: Unlocked={isUnlocked}, Completed={isCompleted}");
    }

    private void OnLevelButtonClicked()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayVFXSound(0);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(levelNumber);
        }

        if (UIManager_PoppiCorny.Instance != null)
        {
            UIManager_PoppiCorny.Instance.EnableLevelPanel(false);
            UIManager_PoppiCorny.Instance.EnableGameplay(true);
        }

        Debug.Log($"Bắt đầu Level {levelNumber}");
    }
}