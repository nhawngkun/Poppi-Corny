using UnityEngine;

public class uihowtoplay_PoppiCorny : UICanvas
{
    public void back()
    {
        UIManager_PoppiCorny.Instance.EnableHowToPlay(false);
        UIManager_PoppiCorny.Instance.EnableHome(true);
        SoundManager.Instance.PlayVFXSound(1);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
