using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

public class UIManager_PoppiCorny : Singleton<UIManager_PoppiCorny>
{
    
    [SerializeField] private List<UICanvas> uiCanvases;
    [SerializeField] private CanvasGroup gameOverPopUp, howToPlay, home, winPopUp, settingPanel, goButton, Loss, gamplayPanel, levelPanel, shopPanel;
    private float time = 0.5f;

    public Transform _effects;
    private bool isPaused = false;
    

    public override void Awake()
    {
        base.Awake();
        InitializeUICanvases();
    }
    #region Extra UI Helpers

/// <summary>
/// Tắt toàn bộ panel trước khi bật panel khác.
/// </summary>
public void HideAllPanels()
{
    foreach (var canvas in uiCanvases)
    {
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
    }
}


/// <summary>
/// Mở panel theo tên class (string).
/// Ví dụ: OpenUIByName("UIhome_PoppiCorny")
/// </summary>
public void OpenUIByName(string uiName)
{
    var ui = uiCanvases.Find(c => c.GetType().Name == uiName);

    if (ui == null)
    {
        Debug.LogError($"UI '{uiName}' not found!");
        return;
    }

    ui.Setup();
    ui.Open();
}


/// <summary>
/// Kiểm tra một UI có đang hiện hay không
/// </summary>
public bool IsUIVisible(string uiName)
{
    var ui = uiCanvases.Find(c => c.GetType().Name == uiName);

    if (ui == null) return false;

    CanvasGroup cg = ui.GetComponent<CanvasGroup>();
    return cg != null && cg.alpha > 0.9f;
}


/// <summary>
/// Reset UI khi restart level, đổi scene...
/// </summary>
public void ResetAllUI()
{
    HideAllPanels();

    // Ví dụ: sau reset đưa về Home
    EnableHome(true);
}


/// <summary>
/// Play sound khi mở UI (nếu thích)
/// </summary>
public void PlayUISound(int id)
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.PlayVFXSound(id);
}

#endregion
    void Start()
    {
        UIManager_PoppiCorny.Instance.OpenUI<UIhome_PoppiCorny>();
    }
    public void EnableGameOverPopUp(bool enable)
    {
        if (enable) gameOverPopUp.DOFade(1f, time).Play();
        else gameOverPopUp.DOFade(0f, time).Play();
        gameOverPopUp.blocksRaycasts = enable;
    }

    public void EnableLoss(bool enable)
    {
        if (enable) Loss.DOFade(1f, time).Play();
        else Loss.DOFade(0f, time).Play();
        Loss.blocksRaycasts = enable;
        Loss.interactable = enable;

    }
    public void EnableHowToPlay(bool enable)
    {
        if (enable) howToPlay.DOFade(1f, time).Play();
        else howToPlay.DOFade(0f, time).Play();
        howToPlay.blocksRaycasts = enable;
        howToPlay.interactable = enable;
    }
    public void EnableHome(bool enable)
    {
        if (enable) home.DOFade(1f, time).Play();
        else home.DOFade(0f, time).Play();
        home.blocksRaycasts = enable;
        home.interactable = enable;
    }

    public void EnableWin(bool enable)
    {
        if (enable) winPopUp.DOFade(1f, time).Play();
        else winPopUp.DOFade(0f, time).Play();
        winPopUp.blocksRaycasts = enable;
        winPopUp.interactable = enable;
    }

    public void EnableSettingPanel(bool enable)
    {
        if (enable) settingPanel.DOFade(1f, time).Play();
        else settingPanel.DOFade(0f, time).Play();
        settingPanel.blocksRaycasts = enable;
        settingPanel.interactable = enable;
    }

    public void EnableGo(bool enable)
    {
        if (enable) goButton.DOFade(1f, time).Play();
        else goButton.DOFade(0f, time).Play();
        goButton.blocksRaycasts = enable;
    }

    public void EnableGameplay(bool enable)
    {
        if (enable) gamplayPanel.DOFade(1f, time).Play();
        else gamplayPanel.DOFade(0f, time).Play();
        gamplayPanel.blocksRaycasts = enable;
        gamplayPanel.interactable = enable;
    }

    public void EnableLevelPanel(bool enable)
    {
        if (enable) levelPanel.DOFade(1f, time).Play();
        else levelPanel.DOFade(0f, time).Play();
        levelPanel.blocksRaycasts = enable;
        levelPanel.interactable = enable;
    }


    private void InitializeUICanvases()
    {
        foreach (var canvas in uiCanvases)
        {

            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }


            canvas.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Setup();
            canvas.Open();
        }
        return canvas;
    }

    public void CloseUI<T>(float time) where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.Close(time);
        }
    }

    public void CloseUIDirectly<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas != null)
        {
            canvas.CloseDirectly();
        }
    }

    public bool IsUIOpened<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        if (canvas == null) return false;

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        return canvasGroup != null && canvasGroup.alpha > 0f;
    }

    public T GetUI<T>() where T : UICanvas
    {
        return uiCanvases.Find(c => c is T) as T;
    }





    public void CloseAll()
    {
        foreach (var canvas in uiCanvases)
        {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null && canvasGroup.alpha > 0f)
            {
                canvas.Close(0);
            }
        }
    }
    public void EnableShop(bool enable)
    {
        if (enable) shopPanel.DOFade(1f, time).Play();
        else shopPanel.DOFade(0f, time).Play();
        shopPanel.blocksRaycasts = enable;
        shopPanel.interactable = enable;
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}