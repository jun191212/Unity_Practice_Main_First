using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Bounds")]
    [SerializeField] private Tilemap groundTilemap; 

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Bounds movementBounds;

    private readonly string ANIM_IDLE = "Idle";
    private readonly string ANIM_RUN = "Run";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0;

        // 타일맵 경계 
        CalculateBounds();
    }

    void CalculateBounds()
    {
        if (groundTilemap != null)
        {
            // 타일맵의 실제 범위
            groundTilemap.CompressBounds();
            movementBounds = groundTilemap.localBounds;

        }
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }

        UpdateAnimation();

        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void FixedUpdate()
    {
        // 이동
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        // 경계 내로 제한
        newPosition.x = Mathf.Clamp(newPosition.x, movementBounds.min.x, movementBounds.max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, movementBounds.min.y, movementBounds.max.y);

        rb.MovePosition(newPosition);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        if (moveInput.magnitude > 0.1f)
        {
            animator.Play(ANIM_RUN);
        }
        else
        {
            animator.Play(ANIM_IDLE);
        }
    }

}