using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIGameplay : UICanvas
{
    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backToLevelSelectButton;

    [Header("Progress Display")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI levelNumberText;

    [Header("Mobile Controls")]
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private GameObject mobileControlsPanel;

    // Trạng thái nút đang được nhấn
    private bool isUpPressed = false;
    private bool isDownPressed = false;
    private bool isLeftPressed = false;
    private bool isRightPressed = false;

    private void Start()
    {
        SetupButtons();
        SetupMobileControls();
        UpdateLevelInfo();

        // Tự động ẩn/hiện mobile controls dựa trên platform
        if (mobileControlsPanel != null)
        {
#if UNITY_ANDROID || UNITY_IOS
            mobileControlsPanel.SetActive(true);
#else
            mobileControlsPanel.SetActive(false);
#endif
        }
    }

    private void Update()
    {
        UpdateProgressDisplay();
    }

    private void SetupButtons()
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(OnResetButtonClicked);
        }

        if (backToLevelSelectButton != null)
        {
            backToLevelSelectButton.onClick.RemoveAllListeners();
            backToLevelSelectButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void SetupMobileControls()
    {
        // Setup nút UP
        if (upButton != null)
        {
            AddEventTrigger(upButton.gameObject, EventTriggerType.PointerDown, (data) => { isUpPressed = true; });
            AddEventTrigger(upButton.gameObject, EventTriggerType.PointerUp, (data) => { isUpPressed = false; });
        }

        // Setup nút DOWN
        if (downButton != null)
        {
            AddEventTrigger(downButton.gameObject, EventTriggerType.PointerDown, (data) => { isDownPressed = true; });
            AddEventTrigger(downButton.gameObject, EventTriggerType.PointerUp, (data) => { isDownPressed = false; });
        }

        // Setup nút LEFT
        if (leftButton != null)
        {
            AddEventTrigger(leftButton.gameObject, EventTriggerType.PointerDown, (data) => { isLeftPressed = true; });
            AddEventTrigger(leftButton.gameObject, EventTriggerType.PointerUp, (data) => { isLeftPressed = false; });
        }

        // Setup nút RIGHT
        if (rightButton != null)
        {
            AddEventTrigger(rightButton.gameObject, EventTriggerType.PointerDown, (data) => { isRightPressed = true; });
            AddEventTrigger(rightButton.gameObject, EventTriggerType.PointerUp, (data) => { isRightPressed = false; });
        }
    }

    private void AddEventTrigger(GameObject target, EventTriggerType eventType, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }

    private void OnResetButtonClicked()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayVFXSound(0);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ReloadCurrentLevel();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayVFXSound(1);
        }

        Debug.Log("Đã reset level");
    }

    private void OnBackButtonClicked()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayVFXSound(1);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayVFXSound(0);
        }

        if (UIManager_PoppiCorny.Instance != null)
        {
            UIManager_PoppiCorny.Instance.EnableGameplay(false);
            UIManager_PoppiCorny.Instance.EnableLevelPanel(true);

            // ← THÊM: REFRESH LEVEL BUTTONS SAU KHI BẬT LẠI LEVEL PANEL
            UIChooseLevel_PoppiCorny levelPanel = FindObjectOfType<UIChooseLevel_PoppiCorny>();
            if (levelPanel != null)
            {
                levelPanel.ForceRefresh();
                Debug.Log("🔄 Đã refresh UIChooseLevel sau khi back");
            }
        }

        Debug.Log("Quay về màn chọn level");
    }

    private void UpdateProgressDisplay()
    {
        if (progressText != null && GameManager.Instance != null)
        {
            progressText.text = GameManager.Instance.GetProgress();
        }
    }

    private void UpdateLevelInfo()
    {
        if (levelNumberText != null && LevelManager.Instance != null)
        {
            levelNumberText.text = $"Level {LevelManager.Instance.CurrentLevelNumber}";
        }
    }

    public override void Setup()
    {
        base.Setup();
        UpdateLevelInfo();
    }

    // Public methods để PlayerController có thể lấy input từ mobile controls
    public float GetHorizontalInput()
    {
        if (isRightPressed) return 1f;
        if (isLeftPressed) return -1f;
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetVerticalInput()
    {
        if (isUpPressed) return 1f;
        if (isDownPressed) return -1f;
        return Input.GetAxisRaw("Vertical");
    }

    // Toggle mobile controls visibility (useful for testing on PC)
    public void ToggleMobileControls(bool show)
    {
        if (mobileControlsPanel != null)
        {
            mobileControlsPanel.SetActive(show);
        }
    }
}