using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; 

    [Header("Settings")]
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private Vector3 offset; 

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // 1. Nếu không có Target (do mới vào game hoặc vừa qua màn mới)
        if (target == null)
        {
            // Tự động đi tìm Player mới
            FindPlayer();
            return; // Tạm dừng frame này, đợi frame sau có target rồi chạy tiếp
        }

        // 2. Logic bám theo Player (Giữ nguyên như cũ)
        Vector3 desiredPosition = target.position + offset;
        
        // Xử lý mượt mà
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );
    }

    // Hàm tự động tìm kiếm Player trong Scene
    private void FindPlayer()
    {
        // Tìm object có script PlayerController
        PlayerController player = FindFirstObjectByType<PlayerController>();

        // Nếu phiên bản Unity cũ không có FindFirstObjectByType thì dùng dòng dưới (bỏ comment):
        // if (player == null) player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            target = player.transform;
            // Tùy chọn: Cập nhật vị trí ngay lập tức để không bị hiệu ứng "trượt" camera từ xa tới
            // transform.position = target.position + offset; 
        }
    }
    
    // Hàm hỗ trợ: Nếu muốn set thủ công từ GameManager (không bắt buộc)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}