using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBoundsSetter : MonoBehaviour
{
    public Tilemap tilemap;              // Tilemap 연결
    public CameraBounds cameraBounds;   // CameraBounds 연결

    void Start()
    {
        // Tilemap 경계 계산
        Bounds bounds = tilemap.localBounds;

        cameraBounds.minBounds = bounds.min;
        cameraBounds.maxBounds = bounds.max;

        Debug.Log($"Tilemap Bounds: Min {bounds.min}, Max {bounds.max}");
    }
}
