using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class RoomTrigger : MonoBehaviour
{
    [Header("Monster Spawn")]
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private int minMonsters = 5;
    [SerializeField] private int maxMonsters = 10;
    [SerializeField] private bool isStartRoom = false;
    [SerializeField] private bool isSafeRoom = false;

    [Header("Door Settings")]
    [SerializeField] private GameObject doorPrefab;

    private bool hasSpawned = false;
    private bool isCleared = false;
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    private List<GameObject> spawnedDoors = new List<GameObject>();
    private BoxCollider2D roomCollider;
    private RectInt actualRoom;
    private Vector3Int offset;
    private bool boundsSet = false;

    void Start()
    {
        roomCollider = GetComponent<BoxCollider2D>();
    }

    public void SetRoomBounds(RectInt room, Vector3Int mapOffset)
    {
        actualRoom = room;
        offset = mapOffset;
        boundsSet = true;
    }

    public void SetStartRoom(bool isStart)
    {
        isStartRoom = isStart;
        isCleared = isStart;
    }

    public void SetSafeRoom(bool isSafe)
    {
        isSafeRoom = isSafe;
        isCleared = isSafe;
    }

    public bool IsCleared()
    {
        return isCleared;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }

    public void OnPlayerEnter()
    {
        if (hasSpawned)
        {
            return;
        }

        if (isStartRoom || isSafeRoom)
        {
            hasSpawned = true;
            isCleared = true;
            return;
        }

   
        SpawnMonsters();
        SpawnDoors();
        hasSpawned = true;
    }

    void SpawnMonsters()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
           
            return;
        }

        int monsterCount = Random.Range(minMonsters, maxMonsters + 1);
    
        for (int i = 0; i < monsterCount; i++)
        {
            GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            Vector2 randomPos = GetRandomPositionInRoom();

            GameObject monster = Instantiate(monsterPrefab, randomPos, Quaternion.identity);
            monster.transform.parent = transform;
            spawnedMonsters.Add(monster);
        }
    }

    void SpawnDoors()
    {


        if (doorPrefab == null)
        {

            return;
        }

        if (!boundsSet)
        {

            return;
        }

        //방 경계에 무조건 문 생성
        Vector2 roomCenter = new Vector2(
            actualRoom.center.x + offset.x,
            actualRoom.center.y + offset.y
        );

        // 4방향에 문 생성
        CreateDoor(new Vector2(roomCenter.x, actualRoom.yMax + offset.y + 0.5f), new Vector3(3, 1, 1)); // 위
        CreateDoor(new Vector2(roomCenter.x, actualRoom.yMin + offset.y - 0.5f), new Vector3(3, 1, 1)); // 아래
        CreateDoor(new Vector2(actualRoom.xMax + offset.x + 0.5f, roomCenter.y), new Vector3(1, 3, 1)); // 오른쪽
        CreateDoor(new Vector2(actualRoom.xMin + offset.x - 0.5f, roomCenter.y), new Vector3(1, 3, 1)); // 왼쪽
    }

    void CreateDoor(Vector2 position, Vector3 scale)
    {
        GameObject door = Instantiate(doorPrefab, position, Quaternion.identity);
        door.transform.parent = transform;
        door.transform.localScale = scale;
        door.name = "Door";

        spawnedDoors.Add(door);
    }

    Vector2 GetRandomPositionInRoom()
    {
        float padding = 1.5f;

        if (boundsSet)
        {
            float minX = actualRoom.x + offset.x + padding;
            float maxX = actualRoom.x + actualRoom.width + offset.x - padding;
            float minY = actualRoom.y + offset.y + padding;
            float maxY = actualRoom.y + actualRoom.height + offset.y - padding;

            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }

        return transform.position;
    }

    void Update()
    {
        if (isCleared) return;

        spawnedMonsters.RemoveAll(monster => monster == null);

        if (hasSpawned && spawnedMonsters.Count == 0)
        {
            OnRoomCleared();
        }
    }

    void OnRoomCleared()
    {
        isCleared = true;
      
        foreach (GameObject door in spawnedDoors)
        {
            if (door != null)
            {
                Destroy(door);
            }
        }
        spawnedDoors.Clear();
    }
}