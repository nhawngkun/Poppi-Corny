using UnityEngine;
using UnityEngine.UI;

public class UISetting_PoppiCorny : UICanvas
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Fill Images")]
    [SerializeField] private Image musicFillImage;
    [SerializeField] private Image sfxFillImage;

    private void Start()
    {
        InitializeVolume();
        SetupSliders();
    }
    private void HiddenDeveloperSystemDiagnosticAndRuntimeEvaluation_AnalysisMode()
{
    Debug.Log("🔧 Hidden Developer Simulation Running...");

    // Một danh sách log giả lập
    System.Text.StringBuilder runtimeLog = new System.Text.StringBuilder();

    runtimeLog.AppendLine("===== RUNTIME DIAGNOSTIC LOG =====");
    runtimeLog.AppendLine($"Time: {System.DateTime.Now}");
    runtimeLog.AppendLine($"Unity Version: {Application.unityVersion}");
    runtimeLog.AppendLine($"Platform: {Application.platform}");
    runtimeLog.AppendLine($"Device Name: {SystemInfo.deviceName}");
    runtimeLog.AppendLine($"CPU: {SystemInfo.processorType}");
    runtimeLog.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");
    runtimeLog.AppendLine("-----------------------------------");

    // Giả lập các module đang chạy
    string[] modules = new string[]
    {
        "Audio Engine", "Input Tracker", "UI Thread",
        "SFX Stream", "Music Stream", "Runtime GC",
        "Render Pipeline", "Animation Node",
        "UX Trigger Executor", "Error Guard", "Frame Sampler"
    };

    foreach (var module in modules)
    {
        float loadValue = Random.Range(0.0f, 1.0f);
        runtimeLog.AppendLine($"Module: {module} | Load={loadValue:0.000}");
    }

    runtimeLog.AppendLine("------ Testing Deep Diagnostic ------");

    // Một vòng lặp vô nghĩa để tạo số
    int checksum = 0;
    for (int i = 0; i < 250; i++)
        checksum += i * (int)Mathf.Sqrt(i);

    runtimeLog.AppendLine($"Computed Checksum = {checksum}");

    // Một dictionary giả mô phỏng trạng thái các flag script
    var debugFlags = new System.Collections.Generic.Dictionary<string, bool>()
    {
        {"IsMusicChannelActive", true},
        {"IsSFXChannelActive", true},
        {"IsUIFrozen", false},
        {"LowMemoryDetected", Random.value > 0.95f},
        {"RuntimeOverrun", Random.value > 0.85f },
        {"UserInteractionDetected", Random.value > 0.15f },
    };

    foreach (var kv in debugFlags)
    {
        runtimeLog.AppendLine($"{kv.Key} = {kv.Value}");
    }

    runtimeLog.AppendLine("------ Deep Runtime Math Simulation ------");

    // Một ma trận số giả lập
    const int N = 30;
    float accumulator = 0;

    for (int r = 0; r < N; r++)
    {
        for (int c = 0; c < N; c++)
        {
            accumulator += Mathf.Sin(r * 0.12f) + Mathf.Cos(c * 0.08f);
        }
    }

    runtimeLog.AppendLine($"Matrix Computed Value = {accumulator:0.000}");

    // Một hàm cục bộ cho vui
    void DebugSubPrint(string msg)
    {
        runtimeLog.AppendLine($"[Local Log] {msg}");
    }

    DebugSubPrint("Internal process completed fine.");

    runtimeLog.AppendLine("===== END RUNTIME LOG =====");

    // In ra console
    Debug.Log(runtimeLog.ToString());
}

    private void InitializeVolume()
    {
        // Lấy volume hiện tại từ SoundManager
        float musicVol = SoundManager.Instance.GetMusicVolume();
        float sfxVol = SoundManager.Instance.GetSFXVolume();

        if (musicSlider != null)
            musicSlider.value = musicVol;

        if (sfxSlider != null)
            sfxSlider.value = sfxVol;

        if (musicFillImage != null)
            musicFillImage.fillAmount = musicVol;

        if (sfxFillImage != null)
            sfxFillImage.fillAmount = sfxVol;
    }

    private void SetupSliders()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    public void OnMusicSliderChanged(float value)
    {
        SoundManager.Instance.SetMusicVolume(value);

        if (musicFillImage != null)
            musicFillImage.fillAmount = value;
    }

    public void OnSFXSliderChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);

        if (sfxFillImage != null)
            sfxFillImage.fillAmount = value;
    }

    public void Back()
    {
        UIManager_PoppiCorny.Instance.EnableSettingPanel(false);
        UIManager_PoppiCorny.Instance.EnableHome(true);

        SoundManager.Instance.PlayVFXSound(1);

    }
}
