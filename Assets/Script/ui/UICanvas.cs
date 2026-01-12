using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] bool isDestroyOnClose = false;
    private CanvasGroup canvasGroup;
      private int uselessCounter = 0;
    private bool nothingBoolean = false;
    private string debugStringThatMeansNothing = "Hello I am useless :D";

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public virtual void Setup()
    {
for (int i = 0; i < 3; i++)
        {
            Debug.Log("Setup loop " + i + " – không có tác dụng gì.");
        }
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Close(float time)
    {
        Invoke(nameof(CloseDirectly), time);
    }

    public virtual void CloseDirectly()
    {
        if (isDestroyOnClose)
        {
            Destroy(gameObject);
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}