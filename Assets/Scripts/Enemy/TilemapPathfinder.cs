using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapPathfinder : MonoBehaviour
{
    private Tilemap wallTilemap;
    private Tilemap floorTilemap;

    private static readonly Vector3Int[] directions = {
        new Vector3Int(1,0,0), new Vector3Int(-1,0,0),  // 동서
        new Vector3Int(0,1,0), new Vector3Int(0,-1,0),  // 남북
        new Vector3Int(1,1,0), new Vector3Int(-1,1,0),  // 대각선
        new Vector3Int(1,-1,0), new Vector3Int(-1,-1,0) // 대각선
    };

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return floorTilemap.WorldToCell(worldPos);
    }

    // 스폰 포인트를 타일맵 셀 좌표로 확인
    void Start()
    {
        TilemapPathfinder pathfinder = FindObjectOfType<TilemapPathfinder>();
        Vector3Int cellPos = pathfinder.WorldToCell(transform.position);
        Debug.Log($"Spawn point cell position: {cellPos}");
    }

    void Awake()
    {
        GameObject wallObj = GameObject.FindGameObjectWithTag("Wall");
        GameObject floorObj = GameObject.FindGameObjectWithTag("Floor");

        if (wallObj != null) wallTilemap = wallObj.GetComponent<Tilemap>();
        if (floorObj != null) floorTilemap = floorObj.GetComponent<Tilemap>();

        Debug.Log($"TilemapPathfinder initialized - Wall: {wallTilemap != null}, Floor: {floorTilemap != null}");
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        if (wallTilemap == null || floorTilemap == null)
        {
            Debug.LogError("TilemapPathfinder: Required tilemaps not found!");
            return new List<Vector3>();
        }

        Vector3Int startCell = floorTilemap.WorldToCell(startWorldPos);
        Vector3Int endCell = floorTilemap.WorldToCell(endWorldPos);

        if (!IsWalkable(startCell))
        {
            Debug.LogWarning($"Start position not walkable: {startCell}");
            return new List<Vector3>();
        }

        if (!IsWalkable(endCell))
        {
            Debug.LogWarning($"End position not walkable: {endCell}");
            return new List<Vector3>();
        }

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        queue.Enqueue(startCell);
        visited.Add(startCell);

        bool pathFound = false;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            if (current == endCell)
            {
                pathFound = true;
                break;
            }

            foreach (var dir in directions)
            {
                Vector3Int next = current + dir;
                if (visited.Contains(next)) continue;

                if (IsWalkable(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                    cameFrom[next] = current;
                }
            }
        }

        List<Vector3> path = new List<Vector3>();
        if (pathFound)
        {
            Vector3Int cur = endCell;
            while (cur != startCell)
            {
                path.Add(CellCenterToWorld(cur));
                cur = cameFrom[cur];
            }
            path.Add(CellCenterToWorld(startCell));
            path.Reverse();
            Debug.Log($"Path found with {path.Count} points");
        }
        else
        {
            Debug.LogWarning("No path found!");
        }

        return path;
    }

    public bool IsWithinTilemapBounds(Vector3Int cellPos)
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        return bounds.Contains(cellPos);
    }

    private bool IsWalkable(Vector3Int cellPos)
    {
        // 범위 밖이면 갈 수 없음
        if (!IsWithinTilemapBounds(cellPos))
            return false;

        // 벽이 있으면 갈 수 없음 
        if (wallTilemap.HasTile(cellPos))
            return false;

        // 바닥이 있으면 갈 수 있음 (제한 완화)
        if (floorTilemap.HasTile(cellPos))
            return true;

        // 타일맵 범위 안이면서 바닥이 없는 곳도 이동 가능
        return true;
    }

    private Vector3 CellCenterToWorld(Vector3Int cellPos)
    {
        Vector3 worldPos = floorTilemap.CellToWorld(cellPos);
        worldPos += floorTilemap.cellSize / 2f;
        return worldPos;
    }
}