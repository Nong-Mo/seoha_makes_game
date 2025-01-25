using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public Transform player;  // ���� �÷��̾�
    public Vector2 minBounds; // ���� �ּ� ���
    public Vector2 maxBounds; // ���� �ִ� ���

    private Camera cam;
    private float camHeight;
    private float camWidth;

    void Start()
    {
        cam = Camera.main;

        // ī�޶� ũ�� ���
        camHeight = cam.orthographicSize * 2;
        camWidth = camHeight * cam.aspect;

        Debug.Log($"ī�޶� ũ��: ���� {camWidth}, ���� {camHeight}");
    }

    void LateUpdate()
    {
        // �÷��̾� ��ġ �������� ī�޶� ��ġ ���
        Vector3 targetPosition = player.position;

        // ī�޶� ��� ���� (Tilemap ��迡 �� �°�)
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x + camWidth / 2, maxBounds.x - camWidth / 2);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y + camHeight / 2, maxBounds.y - camHeight / 2);

        // ī�޶� �̵�
        transform.position = new Vector3(targetPosition.x, targetPosition.y, -10f);
    }
}
