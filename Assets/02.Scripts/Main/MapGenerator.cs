using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private Vector2Int mapSize = new Vector2Int(50, 50);
    [SerializeField] private float minimumDevideRate = 0.3f;
    [SerializeField] private float maximumDivideRate = 0.7f;
    [SerializeField] private int maximumDepth = 2;

    [Header("Tilemap")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    [Header("Visual Debug")]
    [SerializeField] private bool showDebugLines = false;
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject roomLine;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Room Triggers")]
    [SerializeField] private GameObject roomTriggerPrefab;

    [Header("Stairs")]
    [SerializeField] private GameObject stairsPrefab;



    [Header("Boss")]
    [SerializeField] private GameObject bossMonsterPrefab; 


    void SpawnPlayerInTopLeftRoom()
    {
        if (leafNodes.Count == 0)
        {
            return;
        }

        Node topLeftRoom = leafNodes[0];
        float minX = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Node node in leafNodes)
        {
            Vector2 roomCenter = new Vector2(
                node.roomRect.center.x + offset.x,
                node.roomRect.center.y + offset.y
            );

            if (roomCenter.x < minX || (roomCenter.x == minX && roomCenter.y > maxY))
            {
                minX = roomCenter.x;
                maxY = roomCenter.y;
                topLeftRoom = node;
            }
        }

        Vector2 spawnPos = new Vector2(
            topLeftRoom.roomRect.center.x + offset.x,
            topLeftRoom.roomRect.center.y + offset.y
        );

        GameObject existingPlayer = GameObject.Find("Player");
        if (existingPlayer != null)
        {
            existingPlayer.transform.position = spawnPos;
        }
        else
        {
            if (playerPrefab != null)
            {
                GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                player.name = "Player";
            }
        }

        if (topLeftRoom.roomTriggerScript != null)
        {
            topLeftRoom.roomTriggerScript.SetStartRoom(true);
        }

        // 현재 던전 층수
        int currentFloor = 1;
        if (GameManager.Instance != null)
        {
            currentFloor = GameManager.Instance.GetCurrentFloor();
        }

        // 3층이면 보스 생성
        if (currentFloor >= 3)
        {
            SpawnBoss();
        }
        else
        {
            // 3층 미만이면 계단 생성
            if (leafNodes.Count > 1)
            {
                Node safeRoom = leafNodes[Random.Range(0, leafNodes.Count)];
                while (safeRoom == topLeftRoom)
                {
                    safeRoom = leafNodes[Random.Range(0, leafNodes.Count)];
                }

                if (safeRoom.roomTriggerScript != null)
                {
                    safeRoom.roomTriggerScript.SetSafeRoom(true);
                }

                SpawnStairs(safeRoom);
            }
        }
    }

    void SpawnBoss()
    {
        if (bossMonsterPrefab == null)
        {
            return;
        }

        if (leafNodes.Count > 0)
        {
            Node bossRoom = leafNodes[leafNodes.Count - 1];

            // 보스 방을 안전방
            if (bossRoom.roomTriggerScript != null)
            {
                bossRoom.roomTriggerScript.SetSafeRoom(true);
            }

            Vector2 bossPos = new Vector2(
                bossRoom.roomRect.center.x + offset.x,
                bossRoom.roomRect.center.y + offset.y
            );

            GameObject boss = Instantiate(bossMonsterPrefab, bossPos, Quaternion.identity);
            boss.name = "BossMonster";
        }
    }




    private Node root;
    private Vector3Int offset;
    private List<Node> leafNodes = new List<Node>();
    private List<RoomTrigger> allRoomTriggers = new List<RoomTrigger>();

    void Start()
    {
        offset = new Vector3Int(-mapSize.x / 2, -mapSize.y / 2, 0);
        root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));

        if (showDebugLines)
        {
            DrawMap(0, 0);
        }

        Divide(root, 0);
        GenerateRoom(root, 0);
        DrawTilemap(root, 0);
        ConnectAllRoomsMST();
        DrawWalls();

        if (playerPrefab != null)
        {
            SpawnPlayerInTopLeftRoom();
        }

        // 인벤토리 복원 
        RestoreInventory();
    }

    void RestoreInventory()
    {
        if (GameManager.Instance != null && Inventory.Instance != null)
        {
            GameManager.Instance.LoadInventory();
    
        }
    }

    private void DrawMap(int x, int y)
    {
        if (map == null) return;
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(x + mapSize.x, y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(x + mapSize.x, y + mapSize.y) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(x, y + mapSize.y) - mapSize / 2);
    }

    void Divide(Node tree, int n)
    {
        if (n == maximumDepth)
        {
            leafNodes.Add(tree);
            return;
        }

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));

        if (tree.nodeRect.width >= tree.nodeRect.height)
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));

            if (showDebugLines)
                DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));

            if (showDebugLines)
                DrawLine(new Vector2(tree.nodeRect.x, tree.nodeRect.y + split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y + split));
        }

        tree.leftNode.parNode = tree;
        tree.rightNode.parNode = tree;

        Divide(tree.leftNode, n + 1);
        Divide(tree.rightNode, n + 1);
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        if (line == null) return;
        LineRenderer lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }

    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;

        if (n == maximumDepth)
        {
            rect = tree.nodeRect;

            int width = Random.Range(Mathf.Max(5, rect.width * 6 / 10), rect.width * 8 / 10);
            int height = Random.Range(Mathf.Max(5, rect.height * 6 / 10), rect.height * 8 / 10);

            int paddingX = (rect.width - width) / 2;
            int paddingY = (rect.height - height) / 2;
            int x = rect.x + Mathf.Max(2, paddingX);
            int y = rect.y + Mathf.Max(2, paddingY);

            rect = new RectInt(x, y, width, height);

            if (showDebugLines)
                DrawRectangle(rect);
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }

        return rect;
    }

    private void DrawRectangle(RectInt rect)
    {
        if (roomLine == null) return;
        LineRenderer lineRenderer = Instantiate(roomLine).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(rect.x, rect.y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(rect.x + rect.width, rect.y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(rect.x + rect.width, rect.y + rect.height) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(rect.x, rect.y + rect.height) - mapSize / 2);
    }

    // MST로 모든 방 연결
    private void ConnectAllRoomsMST()
    {
        if (leafNodes.Count <= 1)
        {
            return;
        }



        List<Node> unconnected = new List<Node>(leafNodes);
        List<Node> connected = new List<Node>();

        connected.Add(unconnected[0]);
        unconnected.RemoveAt(0);

        int connectionCount = 0;

        while (unconnected.Count > 0)
        {
            float minDistance = float.MaxValue;
            Node closestConnected = null;
            Node closestUnconnected = null;

            foreach (Node connectedNode in connected)
            {
                foreach (Node unconnectedNode in unconnected)
                {
                    float distance = Vector2.Distance(connectedNode.center, unconnectedNode.center);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestConnected = connectedNode;
                        closestUnconnected = unconnectedNode;
                    }
                }
            }

            if (closestUnconnected != null)
            {
                DrawCorridor(closestConnected.center, closestUnconnected.center);
                connected.Add(closestUnconnected);
                unconnected.Remove(closestUnconnected);

                connectionCount++;
               }
        }


    }

    private void DrawTilemap(Node tree, int n)
    {
        if (floorTilemap == null || floorTile == null) return;

        if (n == maximumDepth)
        {
            RectInt room = tree.roomRect;

            for (int x = room.x; x < room.x + room.width; x++)
            {
                for (int y = room.y; y < room.y + room.height; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0) + offset;
                    floorTilemap.SetTile(tilePos, floorTile);
                }
            }

            CreateRoomTrigger(room, tree);
        }
        else
        {
            DrawTilemap(tree.leftNode, n + 1);
            DrawTilemap(tree.rightNode, n + 1);
        }
    }

    private void DrawWalls()
    {
        if (wallTilemap == null || wallTile == null || floorTilemap == null) return;

        BoundsInt bounds = floorTilemap.cellBounds;

        for (int x = bounds.xMin - 1; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (floorTilemap.GetTile(pos) == null)
                {
                    bool hasFloorNearby = false;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            Vector3Int checkPos = pos + new Vector3Int(dx, dy, 0);
                            if (floorTilemap.GetTile(checkPos) != null)
                            {
                                hasFloorNearby = true;
                                break;
                            }
                        }
                        if (hasFloorNearby) break;
                    }

                    if (hasFloorNearby)
                    {
                        wallTilemap.SetTile(pos, wallTile);
                    }
                }
            }
        }
    }

    private void CreateRoomTrigger(RectInt room, Node node)
    {
        if (roomTriggerPrefab == null)
        {
            return;
        }

        Vector2 roomCenter = new Vector2(
            room.center.x + offset.x,
            room.center.y + offset.y
        );

        GameObject trigger = Instantiate(roomTriggerPrefab, roomCenter, Quaternion.identity);
        trigger.name = $"Room_{room.x}_{room.y}";

        BoxCollider2D collider = trigger.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = new Vector2(room.width, room.height);
            collider.isTrigger = true;
        }

        RoomTrigger roomTriggerScript = trigger.GetComponent<RoomTrigger>();
        if (roomTriggerScript != null)
        {
            roomTriggerScript.SetRoomBounds(room, offset);
            allRoomTriggers.Add(roomTriggerScript);
            node.roomTriggerScript = roomTriggerScript;
        }
    }

    // 맵 그리기
    private void DrawCorridor(Vector2Int from, Vector2Int to)
    {
        if (floorTilemap == null || floorTile == null) return;

        int corridorWidth = 3;
        int halfWidth = corridorWidth / 2;

        int startX = Mathf.Min(from.x, to.x);
        int endX = Mathf.Max(from.x, to.x);

        for (int x = startX; x <= endX; x++)
        {
            for (int dy = -halfWidth; dy <= halfWidth; dy++)
            {
                Vector3Int tilePos = new Vector3Int(x, from.y + dy, 0) + offset;
                floorTilemap.SetTile(tilePos, floorTile);
            }
        }

        int startY = Mathf.Min(from.y, to.y);
        int endY = Mathf.Max(from.y, to.y);

        for (int y = startY; y <= endY; y++)
        {
            for (int dx = -halfWidth; dx <= halfWidth; dx++)
            {
                Vector3Int tilePos = new Vector3Int(to.x + dx, y, 0) + offset;
                floorTilemap.SetTile(tilePos, floorTile);
            }
        }

        for (int dx = -halfWidth; dx <= halfWidth; dx++)
        {
            for (int dy = -halfWidth; dy <= halfWidth; dy++)
            {
                Vector3Int tilePos1 = new Vector3Int(from.x + dx, from.y + dy, 0) + offset;
                Vector3Int tilePos2 = new Vector3Int(to.x + dx, to.y + dy, 0) + offset;
                floorTilemap.SetTile(tilePos1, floorTile);
                floorTilemap.SetTile(tilePos2, floorTile);
            }
        }
    }

 

    void SpawnStairs(Node safeRoom)
    {
        if (stairsPrefab == null)
        {
            Debug.LogWarning("Stairs prefab not assigned!");
            return;
        }

        Vector2 stairsPos = new Vector2(
            safeRoom.roomRect.center.x + offset.x,
            safeRoom.roomRect.center.y + offset.y
        );

        GameObject stairs = Instantiate(stairsPrefab, stairsPos, Quaternion.identity);
        stairs.name = "Stairs";

    }
}

public class Node
{
    public Node parNode;
    public Node leftNode;
    public Node rightNode;

    public RectInt nodeRect;
    public RectInt roomRect;
    public RoomTrigger roomTriggerScript;

    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
    }

    public Node(RectInt rect)
    {
        nodeRect = rect;
        roomRect = rect;
    }
}