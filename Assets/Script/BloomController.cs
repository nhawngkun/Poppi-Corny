using UnityEngine;
using DG.Tweening;

public class BloomController : MonoBehaviour
{
    [Header("Bloom States")]
    [SerializeField] private GameObject normalState;
    [SerializeField] private GameObject bloomedState;

    [Header("Settings")]
    [SerializeField] private bool startAsBloomed = false;

    [Header("Animation Settings")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    private bool isBloomed = false;
    private bool isTransitioning = false;

    private void Start()
    {
        if (startAsBloomed)
        {
            SetBloomStateImmediate(true);

        }
        else
        {
            SetBloomStateImmediate(false);
        }
    }

    /// <summary>
    /// Được gọi từ PlayerController khi player vào bloom
    /// </summary>
    public void OnPlayerEnter()
    {
        Debug.Log($"Player entered bloom {gameObject.name}");
    }

    /// <summary>
    /// Được gọi từ PlayerController khi player rời bloom - CHỈ ĐỔI TRẠNG THÁI
    /// </summary>
    public void OnPlayerExit()
    {
        if (!isTransitioning)
        {
            ToggleBloomStateWithAnimation();
        }
    }

    /// <summary>
    /// Chuyển đổi trạng thái hoa với animation
    /// </summary>
    private void ToggleBloomStateWithAnimation()
    {
        isBloomed = !isBloomed;
        SoundManager.Instance.PlayVFXSound(3);
        StartTransition(isBloomed);
    }

    /// <summary>
    /// Bắt đầu quá trình chuyển đổi với animation
    /// </summary>
    private void StartTransition(bool toBloomed)
    {
        isTransitioning = true;

        GameObject currentActive = toBloomed ? normalState : bloomedState;
        GameObject targetActive = toBloomed ? bloomedState : normalState;

        if (currentActive != null) currentActive.SetActive(true);
        if (targetActive != null) targetActive.SetActive(true);

        Sequence transitionSequence = DOTween.Sequence();

        if (currentActive != null)
        {
            currentActive.transform.DOKill();
            transitionSequence.Append(
                currentActive.transform.DOScale(Vector3.zero, transitionDuration * 0.5f)
                    .SetEase(Ease.InBack)
            );
        }

        transitionSequence.AppendCallback(() =>
        {
            if (currentActive != null)
            {
                currentActive.SetActive(false);
                currentActive.transform.localScale = Vector3.one;
            }

            if (targetActive != null)
            {
                targetActive.transform.localScale = Vector3.zero;
            }
        });

        if (targetActive != null)
        {
            targetActive.transform.DOKill();
            transitionSequence.Append(
                targetActive.transform.DOScale(Vector3.one, transitionDuration * 0.5f)
                    .SetEase(scaleEase)
            );
        }

        transitionSequence.OnComplete(() =>
        {
            isTransitioning = false;
            Debug.Log($"Bloom {gameObject.name}: {(toBloomed ? "Nở hoa" : "Trạng thái thường")}");

            // *** KHÔNG GỌI CheckWinCondition Ở ĐÂY NỮA ***
            // Win sẽ được kiểm tra từ PlayerController
        });
    }

    private void SetBloomStateImmediate(bool bloomed)
    {
        isBloomed = bloomed;

        if (normalState != null)
        {
            normalState.SetActive(!bloomed);
            normalState.transform.localScale = Vector3.one;
        }

        if (bloomedState != null)
        {
            bloomedState.SetActive(bloomed);
            bloomedState.transform.localScale = Vector3.one;
        }
    }

    public bool IsBloomed()
    {
        return isBloomed;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    public void ResetToInitialState(bool withAnimation = false)
    {
        if (withAnimation && isBloomed != startAsBloomed)
        {
            ToggleBloomStateWithAnimation();
        }
        else
        {
            SetBloomStateImmediate(startAsBloomed);
        }
    }

    private void OnDestroy()
    {
        if (normalState != null)
        {
            normalState.transform.DOKill();
        }
        if (bloomedState != null)
        {
            bloomedState.transform.DOKill();
        }
    }
}