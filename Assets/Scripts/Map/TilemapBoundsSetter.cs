using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBoundsSetter : MonoBehaviour
{
    public Tilemap tilemap;              // Tilemap ����
    public CameraBounds cameraBounds;   // CameraBounds ����

    void Start()
    {
        // Tilemap ��� ���
        Bounds bounds = tilemap.localBounds;

        cameraBounds.minBounds = bounds.min;
        cameraBounds.maxBounds = bounds.max;

        Debug.Log($"Tilemap Bounds: Min {bounds.min}, Max {bounds.max}");
    }
}
