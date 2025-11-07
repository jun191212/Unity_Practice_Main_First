using UnityEngine;

public class CompleteRoomBarrier : MonoBehaviour
{
    private RoomTrigger linkedRoom;
    private BoxCollider2D barrierCollider;
    private SpriteRenderer barrierSprite;

    [SerializeField] private Color lockedColor = new Color(1, 0, 0, 0.5f); // 빨간색 반투명
    [SerializeField] private Color openColor = new Color(0, 1, 0, 0.5f); // 초록색 반투명

    void Start()
    {
        barrierCollider = GetComponent<BoxCollider2D>();
        barrierSprite = GetComponent<SpriteRenderer>();

        if (barrierCollider == null)
        {
            barrierCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        if (barrierSprite != null)
        {
            barrierSprite.color = lockedColor;
        }

        UpdateBarrier();
    }

    void Update()
    {
        UpdateBarrier();
    }

    public void SetLinkedRoom(RoomTrigger room)
    {
        linkedRoom = room;
    }

    void UpdateBarrier()
    {
        if (linkedRoom == null) return;

        bool shouldBlock = !linkedRoom.IsCleared();

        if (barrierCollider != null)
        {
            barrierCollider.enabled = shouldBlock;
        }

        if (barrierSprite != null)
        {
            barrierSprite.enabled = shouldBlock;
            barrierSprite.color = shouldBlock ? lockedColor : openColor;
        }
    }
}