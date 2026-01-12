using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIhome_PoppiCorny : UICanvas
{
    [Header("References")]
    [SerializeField] private GameObject buttonGroup; // Kéo cái object chứa các nút Play, Setting... vào đây
    [SerializeField] private Slider loadingSlider;   // Kéo thanh Slider vào đây
    [SerializeField] private float loadingDuration = 2.0f; // Thời gian load

    // Biến static để nhớ là đã load xong chưa (giữ giá trị qua các scene)
    private static bool hasLoaded = false;

    private void Start()
    {
        if (!hasLoaded)
        {
            // Nếu là lần đầu mở game -> Chạy Loading
            StartCoroutine(ProcessLoading());
        }
        else
        {
            // Nếu đã load rồi (quay lại từ level) -> Hiện nút luôn, ẩn slider
            if (loadingSlider != null) loadingSlider.gameObject.SetActive(false);
            if (buttonGroup != null) buttonGroup.SetActive(true);
        }
    }

    private IEnumerator ProcessLoading()
    {
        // 1. Ẩn nút, hiện slider
        if (buttonGroup != null) buttonGroup.SetActive(false);
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0;
        }

        // 2. Chạy thanh slider
        float timer = 0;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            if (loadingSlider != null)
            {
                loadingSlider.value = timer / loadingDuration;
            }
            yield return null;
        }

        // 3. Load xong
        if (loadingSlider != null) loadingSlider.value = 1;
        yield return new WaitForSeconds(0.2f); // Đợi 1 xíu cho đẹp

        // 4. Ẩn slider, hiện nút
        if (loadingSlider != null) loadingSlider.gameObject.SetActive(false);
        if (buttonGroup != null) buttonGroup.SetActive(true);

        // Đánh dấu là đã load xong
        hasLoaded = true;
    }

    // --- CÁC HÀM CŨ ---
    public void play()
    {
        UIManager_PoppiCorny.Instance.EnableHome(false);
        UIManager_PoppiCorny.Instance.EnableLevelPanel(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayVFXSound(1);
    }

    public void settings()
    {
        UIManager_PoppiCorny.Instance.EnableHome(false);
        UIManager_PoppiCorny.Instance.EnableSettingPanel(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayVFXSound(1);
    }

    public void howtoplay()
    {
        UIManager_PoppiCorny.Instance.EnableHome(false);
        UIManager_PoppiCorny.Instance.EnableHowToPlay(true);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayVFXSound(1);
    }
}