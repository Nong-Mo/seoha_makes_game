using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapPathfinder : MonoBehaviour
{
    [SerializeField] private Tilemap wallTilemap;   // Wall Ÿ�ϸ� �ϳ��� ����
    [SerializeField] private Tilemap floorTilemap;  // Floor Ÿ�ϸ� �ϳ��� ����

    // Wall�� �ι�° Ÿ�ϸ�
    [SerializeField] private Tilemap wallTilemap2;
    // Floor�� �ι�° Ÿ�ϸ�
    [SerializeField] private Tilemap floorTilemap2;

    private static readonly Vector3Int[] directions = {
        new Vector3Int(1,0,0), new Vector3Int(-1,0,0),  // ����
        new Vector3Int(0,1,0), new Vector3Int(0,-1,0),  // ����
        new Vector3Int(1,1,0), new Vector3Int(-1,1,0),  // �밢��
        new Vector3Int(1,-1,0), new Vector3Int(-1,-1,0) // �밢��
    };

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return floorTilemap.WorldToCell(worldPos);
    }

    // ���� ����Ʈ�� Ÿ�ϸ� �� ��ǥ�� Ȯ��
    void Start()
    {
        TilemapPathfinder pathfinder = FindObjectOfType<TilemapPathfinder>();
        Vector3Int cellPos = pathfinder.WorldToCell(transform.position);
    }

    void Awake()
    {
        GameObject wallObj = GameObject.FindGameObjectWithTag("Wall");
        GameObject floorObj = GameObject.FindGameObjectWithTag("Floor");

        if (wallObj != null)
        {
            wallTilemap = wallObj.GetComponent<Tilemap>();
        }


        if (floorObj != null)
        {
            floorTilemap = floorObj.GetComponent<Tilemap>();
        }
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        if (wallTilemap == null || floorTilemap == null)
        {
            return new List<Vector3>();
        }

        Vector3Int startCell = floorTilemap.WorldToCell(startWorldPos);
        Vector3Int endCell = floorTilemap.WorldToCell(endWorldPos);

        if (!IsWalkable(startCell))
        {
            return new List<Vector3>();
        }

        if (!IsWalkable(endCell))
        {
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
        }
        return path;
    }

    private bool IsWalkable(Vector3Int cellPos)
    {
        // ���� ������ �� �� ���� 
        if (wallTilemap.HasTile(cellPos))
            return false;

        // �ٴ��� ������ �� �� ���� (���� ��ȭ)
        if (floorTilemap.HasTile(cellPos))
            return true;

        return false;
    }

    private Vector3 CellCenterToWorld(Vector3Int cellPos)
    {
        Vector3 worldPos = floorTilemap.CellToWorld(cellPos);
        worldPos += floorTilemap.cellSize / 2f;
        return worldPos;
    }
}