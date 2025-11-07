using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DungeonPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 5000;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private int defense = 0;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private bool showAttackRange = true;
    [SerializeField] private Color attackRangeColor = new Color(1, 0, 0, 0.3f);

    [Header("Effects & Sound")]
    [SerializeField] private AudioClip attackSound;
    private AudioSource audioSource;

    private ExperienceSystem expSystem;
    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private float lastAttackTime;
    private float originalMoveSpeed;
    private GameObject attackRangeVisual;

    private readonly string ANIM_IDLE = "Idle";
    private readonly string ANIM_RUN = "Run";

    void Awake()
    {
        if (FindObjectsOfType<DungeonPlayerController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        expSystem = GetComponent<ExperienceSystem>();

        rb.gravityScale = 0;
        currentHealth = maxHealth;

        originalMoveSpeed = moveSpeed; 

        audioSource = gameObject.AddComponent<AudioSource>();

        if (PlayerPrefs.GetInt("RecoverHealth", 0) == 1)
        {
            RecoverHealth();
            PlayerPrefs.SetInt("RecoverHealth", 0);
        }

        // 공격 범위 시각화
        if (showAttackRange)
        {
            CreateAttackRangeVisual();
        }
    }

    void CreateAttackRangeVisual()
    {
        attackRangeVisual = new GameObject("AttackRangeVisual");
        attackRangeVisual.transform.SetParent(transform);
        attackRangeVisual.transform.localPosition = Vector3.zero;

        LineRenderer lineRenderer = attackRangeVisual.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = attackRangeColor;
        lineRenderer.endColor = attackRangeColor;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.sortingOrder = 10;

        int segments = 50;
        lineRenderer.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * 360f / segments * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * attackRange;
            float y = Mathf.Sin(angle) * attackRange;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void RecoverHealth()
    {
        currentHealth = maxHealth;
        moveSpeed = originalMoveSpeed;
    }

    public void SetStats(int attack, int health, int def)
    {
        attackDamage = attack;
        defense = def;

        // 최대 체력만 설정
        maxHealth = health;

        // 현재 체력이 최대 체력을 초과하면 조정
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
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

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Attack();
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
        rb.velocity = moveInput * moveSpeed;
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

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void IncreaseMaxHealth(int amount)
    {
        int previousMax = maxHealth;
        maxHealth += amount;

        float ratio = (float)currentHealth / previousMax;
        currentHealth = Mathf.RoundToInt(maxHealth * ratio);

        if (ExperienceSystem.Instance != null)
        {
            ExperienceSystem.Instance.UpdateHealthUI();
        }
    }

    public void AddAttack(int amount)
    {
        attackDamage += amount;
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, monsterLayer);
        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(attackDamage);
            }
        }
    }

    public int GetAttackDamage()
    {
        return attackDamage;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - defense);
        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            Die();
        }

        if (ExperienceSystem.Instance != null)
        {
            ExperienceSystem.Instance.UpdateHealthUI();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void Die()
    {
        moveSpeed = 0;

        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOverUI();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Room"))
        {
            RoomTrigger room = other.GetComponent<RoomTrigger>();
            if (room != null)
            {
                room.OnPlayerEnter();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}