using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public Transform player;  // 따라갈 플레이어
    public Vector2 minBounds; // 맵의 최소 경계
    public Vector2 maxBounds; // 맵의 최대 경계

    private Camera cam;
    private float camHeight;
    private float camWidth;

    void Start()
    {
        cam = Camera.main;

        // 카메라 크기 계산
        camHeight = cam.orthographicSize * 2;
        camWidth = camHeight * cam.aspect;

        Debug.Log($"카메라 크기: 가로 {camWidth}, 세로 {camHeight}");
    }

    void LateUpdate()
    {
        // 플레이어 위치 기준으로 카메라 위치 계산
        Vector3 targetPosition = player.position;

        // 카메라 경계 제한 (Tilemap 경계에 딱 맞게)
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x + camWidth / 2, maxBounds.x - camWidth / 2);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y + camHeight / 2, maxBounds.y - camHeight / 2);

        // 카메라 이동
        transform.position = new Vector3(targetPosition.x, targetPosition.y, -10f);
    }
}
