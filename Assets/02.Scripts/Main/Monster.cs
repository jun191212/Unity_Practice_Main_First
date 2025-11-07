using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public int maxHealth = 30;
    [SerializeField] private int damage = 10;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int expReward = 10; 

    public int currentHealth;
    protected Transform player;          
    protected Rigidbody2D rb;           
    protected SpriteRenderer spriteRenderer; 
    private Animator animator;

    private float lastAttackTime;
    protected bool isDead = false;       

    private readonly int ANIM_SPEED = Animator.StringToHash("Speed");

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0;
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // 공격
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // 추적
            ChasePlayer();
        }
        else
        {
            // 범위 밖 정지
            rb.velocity = Vector2.zero;
            if (animator != null) animator.SetFloat(ANIM_SPEED, 0);
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        if (animator != null)
        {
            animator.SetFloat(ANIM_SPEED, moveSpeed);
        }
    }

    void Attack()
    {
        rb.velocity = Vector2.zero;
        if (animator != null) animator.SetFloat(ANIM_SPEED, 0);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;


            var dungeonController = player.GetComponent<DungeonPlayerController>();
            if (dungeonController != null)
            {
                dungeonController.TakeDamage(damage);
                return;
            }
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;


        // 경험치 드롭
        DropExperience();

        Destroy(gameObject, 0.5f);
    }

    protected virtual void DropExperience()
    {

        if (SceneManager.GetActiveScene().name == "" || SceneManager.GetActiveScene().isLoaded == false)
        {
            return;
        }

        if (ExperienceSystem.Instance != null)
        {
            ExperienceSystem.Instance.GainExperience(expReward);
        }

    }
    void OnDrawGizmosSelected()
    {
        // 감지 범위 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 공격 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}