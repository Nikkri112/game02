using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    public enum BossState
    {
        Chase,
        Retreat,
        DashAttack,
        MeleeAttack,
        RangedAttack
    }

    [Header("Health Settings")]
    public int maxHealth = 500;
    public int currentHealth;
    public HealthBar healthBar;
    public EnemyHealth enemyHealth;

    [Header("Movement Settings")]
    public float chaseSpeed = 5f;
    public float retreatSpeed = 7f;
    public float stoppingDistance = 2f;
    public float retreatDistance = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.5f;

    [Header("Attack Settings")]
    public float meleeAttackRange = 2f;
    public float rangedAttackRange = 10f;
    public float meleeAttackCooldown = 2f;
    public float rangedAttackCooldown = 3f;
    public float dashAttackCooldown = 5f;
    public GameObject meleeAttackPrefab;
    public GameObject rangedAttackPrefab;
    public float attackOffset = 0.5f; // Смещение атаки перед боссом
    public Animator animator;

    [Header("State Timing")]
    public float minStateDuration = 1f;
    public float maxStateDuration = 3f;

    private Transform player;
    private BossState currentState;
    private float stateTimer;
    private float meleeCooldownTimer;
    private float rangedCooldownTimer;
    private float dashCooldownTimer;
    private bool isDashing;
    private Vector2 dashDirection;
    private Rigidbody2D rb;
    private Vector2 lastMovementDirection = Vector2.right;
    // Начальное направление

    [Header("Layer Settings")]
    [SerializeField] private LayerMask normalCollisionLayers;
    [SerializeField] private LayerMask dashCollisionLayers;
    [SerializeField] private Collider2D bossCollider;

    public GameObject pobject;

    void Start()
    {
        player = pobject.transform;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        if (bossCollider == null)
        {
            bossCollider = GetComponent<Collider2D>();
        }
        ChangeState(BossState.Chase);
    }

    void Update()
    {
        // ��������� ������� ���������
        UpdateCooldowns();

        // ��������� ������� ���������
        UpdateState();

        currentHealth = enemyHealth.currentHealth;
        healthBar.SetHealth(currentHealth);
        
        // ��������� ��������
        if (currentHealth <= 0)
        {
            Destroy(healthBar, 2);
            Die();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        animator.SetFloat("Walk", Mathf.Abs(rb.linearVelocityX));
    }

    void UpdateCooldowns()
    {
        if (meleeCooldownTimer > 0) meleeCooldownTimer -= Time.deltaTime;
        if (rangedCooldownTimer > 0) rangedCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
    }

    void UpdateState()
    {
        stateTimer -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Chase:
                ChasePlayer(distanceToPlayer);
                break;

            case BossState.Retreat:
                RetreatFromPlayer(distanceToPlayer);
                break;

            case BossState.DashAttack:
                if (!isDashing && stateTimer <= 0)
                {
                    ChangeState(GetRandomState());
                }
                break;

            case BossState.MeleeAttack:
                if (stateTimer <= 0)
                {
                    PerformMeleeAttack();
                    ChangeState(GetRandomState());
                }
                break;

            case BossState.RangedAttack:
                if (stateTimer <= 0)
                {
                    PerformRangedAttack();
                    ChangeState(GetRandomState());
                }
                break;
        }

        // �������� �� ����������� �����
        if (currentState != BossState.DashAttack &&
            currentState != BossState.MeleeAttack &&
            currentState != BossState.RangedAttack)
        {
            if (distanceToPlayer <= meleeAttackRange && meleeCooldownTimer <= 0)
            {
                ChangeState(BossState.MeleeAttack);
            }
            else if (distanceToPlayer <= rangedAttackRange && rangedCooldownTimer <= 0)
            {
                ChangeState(BossState.RangedAttack);
            }
            else if (distanceToPlayer > stoppingDistance && dashCooldownTimer <= 0)
            {
                ChangeState(BossState.DashAttack);
            }
        }
    }

    void ChasePlayer(float distance)
    {
        if (distance > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * chaseSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (stateTimer <= 0)
            {
                ChangeState(BossState.Retreat);
            }
        }
    }

    void RetreatFromPlayer(float distance)
    {
        if (distance < retreatDistance)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            rb.linearVelocity = direction * retreatSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (stateTimer <= 0)
            {
                ChangeState(BossState.Chase);
            }
        }
    }

    void PerformDashAttack()
    {
        dashDirection = (player.position - transform.position).normalized;
        isDashing = true;

        // Изменяем слои для коллайдера во время рывка
        if (bossCollider != null)
        {
            bossCollider.excludeLayers = dashCollisionLayers;
        }
        StartCoroutine(StopDashing());
    }

    IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        // Возвращаем обычные настройки слоев
        if (bossCollider != null)
        {
            Debug.Log(normalCollisionLayers);
            bossCollider.excludeLayers = normalCollisionLayers;
        }
    }

    Vector2 GetAttackPosition()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        return (Vector2)transform.position + direction * attackOffset;
    }

    // Метод для получения поворота атаки
    Quaternion GetAttackRotation()
    {
        float angle = Mathf.Atan2(lastMovementDirection.y, lastMovementDirection.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void PerformMeleeAttack()
    {
        if (meleeAttackPrefab != null)
        {
           GameObject meleeattack = Instantiate(meleeAttackPrefab, GetAttackPosition(), GetAttackRotation());
           ZombAttack zmbattackscript = meleeattack.GetComponent<ZombAttack>();
           Vector2 direction = (player.position - transform.position).normalized;
            if (zmbattackscript != null)
            {
                zmbattackscript.direction = direction;
                zmbattackscript.shooter = gameObject;
            }
            animator.SetTrigger("MeleeAttack");
        }
        
        meleeCooldownTimer = meleeAttackCooldown;
    }

    void PerformRangedAttack()
    {
        if (rangedAttackPrefab != null)
        {
            GameObject projectile = Instantiate(rangedAttackPrefab, GetAttackPosition(), GetAttackRotation());
            Vector2 direction = (player.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f;
        }
        animator.SetTrigger("Attack");
        rangedCooldownTimer = rangedAttackCooldown;
    }

    void ChangeState(BossState newState)
    {
        currentState = newState;
        stateTimer = Random.Range(minStateDuration, maxStateDuration);

        switch (newState)
        {
            case BossState.DashAttack:
                PerformDashAttack();
                dashCooldownTimer = dashAttackCooldown;
                break;

            case BossState.Chase:
            case BossState.Retreat:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    BossState GetRandomState()
    {
        // ������� ����� ���������� ���������
        float rand = Random.value;
        if (rand < 0.5f) return BossState.Chase;
        if (rand < 0.8f) return BossState.Retreat;
        return BossState.DashAttack;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        // ������� �� ��������� ����� - ��������, �����������
        if (currentState != BossState.Retreat)
        {
            ChangeState(BossState.Retreat);
        }
    }

    IEnumerator LoadCreditsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Credits"); // Замените "CreditsScene" на точное имя вашей сцены с титрами
    }

    void Die()
    {
        
        // ���������� ������ �����
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        // ������������ ��������� � ���������
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}