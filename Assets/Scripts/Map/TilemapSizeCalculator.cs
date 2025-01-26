using UnityEngine;

public class TilemapSizeCalculator : MonoBehaviour
{
    public Camera mainCamera; // ī�޶� ����
    public Vector2 tileSize = new Vector2(1, 1); // Ÿ�� �ϳ��� ũ�� (�⺻: 1x1)

    void Start()
    {
        // ī�޶� ũ�� ���
        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;

        // Ÿ�� ���� �ּ� �� ũ�� ���
        float minTilemapWidth = Mathf.Ceil(camWidth / tileSize.x);
        float minTilemapHeight = Mathf.Ceil(camHeight / tileSize.y);

        Debug.Log($"ī�޶� ũ��: ���� {camWidth}, ���� {camHeight}");
        Debug.Log($"Tilemap �ּ� ũ��: {minTilemapWidth} x {minTilemapHeight} Ÿ��");
    }
}
