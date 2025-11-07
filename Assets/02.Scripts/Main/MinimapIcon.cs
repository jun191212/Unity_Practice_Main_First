using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [Header("Icon Settings")]
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Color iconColor = Color.white;
    [SerializeField] private float iconSize = 0.5f;

    private GameObject iconInstance;
    private SpriteRenderer iconRenderer;

    void Start()
    {
        CreateIcon();
    }

    void CreateIcon()
    {
        iconInstance = new GameObject("MinimapIcon");
        iconInstance.transform.SetParent(transform);
        iconInstance.transform.localPosition = Vector3.zero;
        iconInstance.layer = LayerMask.NameToLayer("Minimap");

        iconRenderer = iconInstance.AddComponent<SpriteRenderer>();
        iconRenderer.sprite = CreateCircleSprite();
        iconRenderer.color = iconColor;
        iconRenderer.sortingOrder = 100;
        iconInstance.transform.localScale = Vector3.one * iconSize;
    }

    Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                pixels[y * 32 + x] = dist < 16 ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    void OnDestroy()
    {
        if (iconInstance != null)
        {
            Destroy(iconInstance);
        }
    }
}