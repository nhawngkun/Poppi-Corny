using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIChooseLevel_PoppiCorny : UICanvas
{
    [Header("Panel References")]
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;

    [Header("Panel Images")]
    [SerializeField] private GameObject panel1Image;
    [SerializeField] private GameObject panel2Image;

    [Header("Navigation Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button backButton;

    [Header("Manual Button Setup (Kéo các nút Level vào đây)")]
    // Bạn kéo các nút trong Panel 1 vào List này
    [SerializeField] private List<LevelButton> panel1Buttons = new List<LevelButton>();

    // Bạn kéo các nút trong Panel 2 vào List này
    [SerializeField] private List<LevelButton> panel2Buttons = new List<LevelButton>();

    private int currentPanelIndex = 0;

    // List tổng hợp để quản lý chung
    private List<LevelButton> allLevelButtons = new List<LevelButton>();

    private void Start()
    {
        SetupButtons();
        InitializeButtons();
        ShowPanel(0);
    }

    private void OnEnable()
    {
        // Refresh lại trạng thái mỗi khi bật UI lên
        if (allLevelButtons.Count > 0)
        {
            RefreshLevelButtons();
        }
    }

    private void SetupButtons()
    {
        if (leftButton != null)
        {
            leftButton.onClick.RemoveAllListeners();
            leftButton.onClick.AddListener(OnLeftButtonClicked);
        }

        if (rightButton != null)
        {
            rightButton.onClick.RemoveAllListeners();
            rightButton.onClick.AddListener(OnRightButtonClicked);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void InitializeButtons()
    {
        allLevelButtons.Clear();
        int currentLevelCount = 1;

        // --- XỬ LÝ PANEL 1 ---
        // Nếu bạn chưa kéo gì vào List (List trống), code sẽ tự tìm trong Panel1
        if (panel1Buttons.Count == 0 && panel1 != null)
        {
            foreach (Transform child in panel1.transform)
            {
                LevelButton btn = child.GetComponent<LevelButton>();
                if (btn != null) panel1Buttons.Add(btn);
            }
        }

        // Setup các nút trong Panel 1
        foreach (var btn in panel1Buttons)
        {
            if (btn != null)
            {
                btn.Setup(currentLevelCount);
                allLevelButtons.Add(btn);
                currentLevelCount++;
            }
        }

        // --- XỬ LÝ PANEL 2 ---
        // Nếu bạn chưa kéo gì vào List (List trống), code sẽ tự tìm trong Panel2
        if (panel2Buttons.Count == 0 && panel2 != null)
        {
            foreach (Transform child in panel2.transform)
            {
                LevelButton btn = child.GetComponent<LevelButton>();
                if (btn != null) panel2Buttons.Add(btn);
            }
        }

        // Setup các nút trong Panel 2
        foreach (var btn in panel2Buttons)
        {
            if (btn != null)
            {
                btn.Setup(currentLevelCount);
                allLevelButtons.Add(btn);
                currentLevelCount++;
            }
        }

        Debug.Log($"Đã setup tổng cộng {allLevelButtons.Count} level buttons.");
    }

    private void RefreshLevelButtons()
    {
        foreach (var btn in allLevelButtons)
        {
            if (btn != null)
            {
                btn.RefreshState();
            }
        }
    }

    private void ShowPanel(int panelIndex)
    {
        currentPanelIndex = panelIndex;

        if (panel1 != null) panel1.SetActive(panelIndex == 0);
        if (panel2 != null) panel2.SetActive(panelIndex == 1);

        if (panel1Image != null) panel1Image.SetActive(panelIndex == 0);
        if (panel2Image != null) panel2Image.SetActive(panelIndex == 1);

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        // Kiểm tra xem Panel 2 có nút nào không
        bool hasButtonsInPanel2 = panel2Buttons.Count > 0;

        if (leftButton != null)
            leftButton.interactable = currentPanelIndex > 0;

        if (rightButton != null)
            rightButton.interactable = currentPanelIndex < 1 && hasButtonsInPanel2;
    }

    private void OnLeftButtonClicked()
    {
        if (currentPanelIndex > 0)
        {
            ShowPanel(currentPanelIndex - 1);
            if (SoundManager.Instance != null) SoundManager.Instance.PlayVFXSound(1);
        }
    }

    private void OnRightButtonClicked()
    {
        if (currentPanelIndex < 1)
        {
            ShowPanel(currentPanelIndex + 1);
            if (SoundManager.Instance != null) SoundManager.Instance.PlayVFXSound(1);
        }
    }

    private void OnBackButtonClicked()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayVFXSound(1);

        if (UIManager_PoppiCorny.Instance != null)
        {
            UIManager_PoppiCorny.Instance.EnableLevelPanel(false);
            UIManager_PoppiCorny.Instance.EnableHome(true);
        }
    }

    public override void Setup()
    {
        base.Setup();
        InitializeButtons();
        ShowPanel(0);
    }

    public void ForceRefresh()
    {
        RefreshLevelButtons();
    }
}