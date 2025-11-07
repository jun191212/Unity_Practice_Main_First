using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer doorSprite;
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color openColor = Color.green;

    private bool isLocked = true;

    void Start()
    {
        if (doorSprite == null) doorSprite = GetComponent<SpriteRenderer>();
        if (doorCollider == null) doorCollider = GetComponent<BoxCollider2D>();

        SetLocked(true);
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (doorSprite != null)
        {
            doorSprite.color = locked ? lockedColor : openColor;
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = locked;
        }

   
    }
}