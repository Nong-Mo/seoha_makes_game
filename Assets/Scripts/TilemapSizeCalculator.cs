using UnityEngine;

public class TilemapSizeCalculator : MonoBehaviour
{
    public Camera mainCamera; // 카메라 연결
    public Vector2 tileSize = new Vector2(1, 1); // 타일 하나의 크기 (기본: 1x1)

    void Start()
    {
        // 카메라 크기 계산
        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;

        // 타일 기준 최소 맵 크기 계산
        float minTilemapWidth = Mathf.Ceil(camWidth / tileSize.x);
        float minTilemapHeight = Mathf.Ceil(camHeight / tileSize.y);

        Debug.Log($"카메라 크기: 가로 {camWidth}, 세로 {camHeight}");
        Debug.Log($"Tilemap 최소 크기: {minTilemapWidth} x {minTilemapHeight} 타일");
    }
}
