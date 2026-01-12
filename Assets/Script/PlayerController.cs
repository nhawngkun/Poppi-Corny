using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Player Models")]
    [SerializeField] public GameObject normalPlayer;
    [SerializeField] public GameObject winPlayer;
    [SerializeField] public GameObject vfx;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tweenDuration = 0.2f;
    [SerializeField] private float bloomMoveDistance = 5f;
    [SerializeField] private float bloomRotationDuration = 0.3f;

    [Header("Detection Settings")]
    [SerializeField] private LayerMask bloomZoneLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask dieLayer;
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private float sideBloomCheckDistance = 1.0f;
    [SerializeField] private float raycastWidthOffset = 0.3f; // [MỚI] Khoảng cách lệch trái/phải để check mép

    [Header("Die Settings")]
    [SerializeField] private float deathYThreshold = -100f;
    [SerializeField] private float resetDelayAfterDeath = 1f;

    private Rigidbody rb;
    private Animator animator;
    private Collider playerCollider;
    private UIGameplay uiGameplay;

    // TRẠNG THÁI
    private bool isBlooming = false;
    private bool waitingBloomInput = false;
    private bool isTweening = false;
    private bool hasRotatedInBloom = false;
    private bool isDead = false;
    private bool hasWon = false;

    private Collider currentStandingBloom = null;
    private BloomController currentBloomController = null;
    private Vector3 bloomCenter;
    private int lastFacingDir = 1;

    // Sound
    private bool isPlayingFootstepSound = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        animator = GetComponentInChildren<Animator>();

        if (vfx != null)
            vfx.SetActive(false);

        uiGameplay = FindObjectOfType<UIGameplay>();

        ResetToNormalPlayer();
    }

    private void Update()
    {
        if (hasWon) return;

        // Kiểm tra chết theo vị trí Y
        if (!isDead && transform.position.y < deathYThreshold)
        {
            OnPlayerDie("Rơi xuống vực");
            return;
        }

        if (isDead || isTweening) return;

        if (isBlooming)
        {
            if (waitingBloomInput)
            {
                HandleBloomJumpInput();
            }
            return;
        }

        NormalMove();

        if (CheckInputToEnterBloomFromSide())
        {
            return;
        }

        CheckStandingOnBloom();
        CheckInputToEnterBloom();
    }

    // --- LOGIC BLOOM ---

    // [CẬP NHẬT] Hàm check ground mới với 3 tia Raycast
    private void CheckStandingOnBloom()
    {
        RaycastHit hit;

        // 1. Kiểm tra tâm (Ưu tiên nhất)
        bool hitCenter = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, bloomZoneLayer);

        // 2. Kiểm tra bên trái
        Vector3 leftOrigin = transform.position + Vector3.left * raycastWidthOffset;
        bool hitLeft = Physics.Raycast(leftOrigin, Vector3.down, out RaycastHit hitL, groundCheckDistance, bloomZoneLayer);

        // 3. Kiểm tra bên phải
        Vector3 rightOrigin = transform.position + Vector3.right * raycastWidthOffset;
        bool hitRight = Physics.Raycast(rightOrigin, Vector3.down, out RaycastHit hitR, groundCheckDistance, bloomZoneLayer);

        if (hitCenter)
        {
            currentStandingBloom = hit.collider;
        }
        else if (hitLeft)
        {
            currentStandingBloom = hitL.collider;
        }
        else if (hitRight)
        {
            currentStandingBloom = hitR.collider;
        }
        else
        {
            currentStandingBloom = null;
        }
    }

    private void CheckInputToEnterBloom()
    {
        if (currentStandingBloom != null)
        {
            float v = GetVerticalInput();
            if (v < -0.5f)
            {
                BloomController bloomCtrl = currentStandingBloom.GetComponentInParent<BloomController>();
                EnterBloomState(currentStandingBloom.bounds.center, bloomCtrl);
                currentStandingBloom = null;
            }
        }
    }

    private bool CheckInputToEnterBloomFromSide()
    {
        float h = GetHorizontalInput();

        if (Mathf.Abs(h) < 0.5f)
        {
            return false;
        }

        Vector3 checkDir = new Vector3(Mathf.Sign(h), 0, 0);

        if (Physics.Raycast(transform.position, checkDir, out RaycastHit hit, sideBloomCheckDistance, bloomZoneLayer))
        {
            BloomController bloomCtrl = hit.collider.GetComponentInParent<BloomController>();
            EnterBloomState(hit.collider.bounds.center, bloomCtrl);
            return true;
        }

        return false;
    }

    private void EnterBloomState(Vector3 center, BloomController bloomCtrl)
    {
        isBlooming = true;
        waitingBloomInput = false;
        isTweening = true;
        hasRotatedInBloom = false;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        playerCollider.isTrigger = true;

        currentBloomController = bloomCtrl;

        if (currentBloomController != null)
        {
            currentBloomController.OnPlayerEnter();
        }

        transform.DOKill();

        if (animator != null)
        {
            animator.SetBool("run", false);
        }

        Sequence enterSequence = DOTween.Sequence();
        enterSequence.Append(transform.DOMove(center, tweenDuration).SetEase(Ease.OutQuad));
        enterSequence.Join(transform.DORotate(new Vector3(0, 90, 0), tweenDuration).SetEase(Ease.OutQuad));
        enterSequence.OnComplete(() =>
        {
            bloomCenter = center;
            hasRotatedInBloom = true;
            isTweening = false;
            waitingBloomInput = true;
        });
    }

    private void JumpToFreeAir(Vector3 targetPos, Vector3 jumpDirection)
    {
        isTweening = true;
        waitingBloomInput = false;

        BloomController exitingBloom = currentBloomController;

        if (animator != null)
        {
            animator.SetTrigger("jump");
        }

        transform.DOKill();

        Sequence exitSequence = DOTween.Sequence();
        exitSequence.Append(transform.DOMove(targetPos, tweenDuration).SetEase(Ease.OutQuad));
        exitSequence.Join(transform.DORotate(new Vector3(0, 0, 0), tweenDuration).SetEase(Ease.OutQuad));
        exitSequence.OnComplete(() =>
        {
            CompleteJumpToFreeAir(exitingBloom, jumpDirection);
        });
    }

    private void CompleteJumpToFreeAir(BloomController exitingBloom, Vector3 jumpDirection)
    {
        rb.isKinematic = false;
        playerCollider.isTrigger = false;

        isBlooming = false;
        isTweening = false;
        waitingBloomInput = false;
        hasRotatedInBloom = false;

        if (exitingBloom != null)
        {
            exitingBloom.OnPlayerExit();
        }

        currentBloomController = null;

        if (animator != null)
        {
            animator.SetBool("run", false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckWinConditionAfterJump(jumpDirection);
        }
    }

    private void JumpToNextBloom(Vector3 targetCenter, BloomController bloomCtrl)
    {
        isTweening = true;
        waitingBloomInput = false;

        BloomController previousBloom = currentBloomController;
        currentBloomController = bloomCtrl;

        if (animator != null)
        {
            animator.SetTrigger("jump");
        }

        transform.DOKill();

        transform.DOMove(targetCenter, tweenDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (previousBloom != null)
            {
                previousBloom.OnPlayerExit();
            }

            if (currentBloomController != null)
            {
                currentBloomController.OnPlayerEnter();
            }

            bloomCenter = targetCenter;
            isTweening = false;
            waitingBloomInput = true;
            rb.isKinematic = true;
        });
    }

    private void NormalMove()
    {
        Vector3 dir = GetInputDirection();
        Vector3 move = new Vector3(dir.x, 0, 0) * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        if (dir.x != 0)
        {
            lastFacingDir = dir.x < 0 ? -1 : 1;
            transform.localScale = new Vector3(lastFacingDir, 1, 1);
            SoundManager.Instance.PlayVFXSound(2);
        }

        if (animator != null)
        {
            bool isRunning = Mathf.Abs(dir.x) > 0.1f;
            animator.SetBool("run", isRunning);
        }
    }

    private float GetHorizontalInput()
    {
        if (uiGameplay != null)
        {
            return uiGameplay.GetHorizontalInput();
        }
        return Input.GetAxisRaw("Horizontal");
    }

    private float GetVerticalInput()
    {
        if (uiGameplay != null)
        {
            return uiGameplay.GetVerticalInput();
        }
        return Input.GetAxisRaw("Vertical");
    }

    private Vector3 GetInputDirection()
    {
        float h = GetHorizontalInput();
        float v = GetVerticalInput();
        return new Vector3(h, v, 0).normalized;
    }

    private void HandleBloomJumpInput()
    {
        Vector3 dir = GetInputDirection();
        if (dir != Vector3.zero)
        {
            if (IsPathBlockedByWall(dir)) return;

            if (TryGetNextBloomTarget(dir, out Vector3 foundCenter, out BloomController foundBloomCtrl))
            {
                JumpToNextBloom(foundCenter, foundBloomCtrl);
            }
            else
            {
                Vector3 targetPos = bloomCenter + (dir * bloomMoveDistance);
                JumpToFreeAir(targetPos, dir);
            }
        }
    }

    private bool IsPathBlockedByWall(Vector3 dir) => Physics.Raycast(bloomCenter, dir, bloomMoveDistance, wallLayer);

    private bool TryGetNextBloomTarget(Vector3 dir, out Vector3 center, out BloomController bloomCtrl)
    {
        center = Vector3.zero;
        bloomCtrl = null;

        if (Physics.SphereCast(bloomCenter, 0.5f, dir, out RaycastHit hit, bloomMoveDistance, bloomZoneLayer))
        {
            center = hit.collider.bounds.center;
            bloomCtrl = hit.collider.GetComponentInParent<BloomController>();
            return true;
        }
        return false;
    }

    // --- PLAYER MODEL MANAGEMENT ---

    public void SwitchToWinPlayer()
    {
        hasWon = true;
        Debug.Log($"🎉 SwitchToWinPlayer được gọi! hasWon={hasWon}, isDead={isDead}");

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Sequence winSequence = DOTween.Sequence();

        if (normalPlayer != null)
        {
            normalPlayer.transform.DOKill();
            winSequence.Append(
                normalPlayer.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            );
            winSequence.AppendCallback(() => normalPlayer.SetActive(false));
        }

        if (vfx != null)
        {
            winSequence.AppendCallback(() =>
            {
                vfx.SetActive(true);
            });
        }

        if (winPlayer != null)
        {
            winSequence.AppendCallback(() =>
            {
                winPlayer.SetActive(true);
                winPlayer.transform.localScale = Vector3.zero;
            });

            winSequence.Append(
                winPlayer.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
            );
        }

        Debug.Log("Chuyển sang trạng thái Win với hiệu ứng");
    }

    public void ResetToNormalPlayer()
    {
        isDead = false;
        hasWon = false;

        if (normalPlayer != null)
        {
            normalPlayer.transform.DOKill();
            normalPlayer.SetActive(true);
            normalPlayer.transform.localScale = Vector3.one;
        }

        if (winPlayer != null)
        {
            winPlayer.transform.DOKill();
            winPlayer.SetActive(false);
            winPlayer.transform.localScale = Vector3.one;
        }

        if (vfx != null)
            vfx.SetActive(false);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }

        isBlooming = false;
        waitingBloomInput = false;
        isTweening = false;
        hasRotatedInBloom = false;
    }

    // --- LOGIC XỬ LÝ CHẾT ---

    private void OnPlayerDie(string reason)
    {
        if (isDead || hasWon)
        {
            Debug.Log($"🚫 Bỏ qua chết vì: isDead={isDead}, hasWon={hasWon}");
            return;
        }

        isDead = true;
        Debug.Log($"💀 PLAYER CHẾT: {reason}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopAllCoroutines();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (animator != null)
        {
            animator.SetBool("run", false);
        }

        if (normalPlayer != null)
        {
            normalPlayer.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        }

        StartCoroutine(ResetLevelAfterDelay());
    }

    private IEnumerator ResetLevelAfterDelay()
    {
        yield return new WaitForSeconds(resetDelayAfterDeath);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ReloadCurrentLevel();
        }
    }

    // --- VA CHẠM VỚI DIE LAYER ---

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead || hasWon)
        {
            Debug.Log($"🚫 Bỏ qua collision vì: isDead={isDead}, hasWon={hasWon}");
            return;
        }

        if ((dieLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log($"⚠️ Collision với die layer: {collision.gameObject.name}");
            OnPlayerDie($"Va chạm với {collision.gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead || hasWon)
        {
            Debug.Log($"🚫 Bỏ qua trigger vì: isDead={isDead}, hasWon={hasWon}");
            return;
        }

        if ((dieLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log($"⚠️ Trigger với die layer: {other.gameObject.name}");
            OnPlayerDie($"Chạm vào {other.gameObject.name}");
        }
    }

    // --- PUBLIC METHODS ---

    public bool IsDead() => isDead;

    // --- VẼ DEBUG GIZMOS ---
    private void OnDrawGizmos()
    {
        // [CẬP NHẬT] Vẽ cả 3 tia trong Gizmos để dễ debug
        Gizmos.color = Color.green;

        // Tia giữa
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);

        // Tia trái
        Vector3 leftOrigin = transform.position + Vector3.left * raycastWidthOffset;
        Gizmos.DrawRay(leftOrigin, Vector3.down * groundCheckDistance);

        // Tia phải
        Vector3 rightOrigin = transform.position + Vector3.right * raycastWidthOffset;
        Gizmos.DrawRay(rightOrigin, Vector3.down * groundCheckDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.right * sideBloomCheckDistance);
        Gizmos.DrawRay(transform.position, Vector3.left * sideBloomCheckDistance);

        if (isBlooming)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, bloomMoveDistance);
        }

        Gizmos.color = Color.red;
        Vector3 leftPoint = transform.position + Vector3.left * 100f;
        Vector3 rightPoint = transform.position + Vector3.right * 100f;
        leftPoint.y = deathYThreshold;
        rightPoint.y = deathYThreshold;
        Gizmos.DrawLine(leftPoint, rightPoint);
    }
}