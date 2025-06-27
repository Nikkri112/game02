using System;
using UnityEditor.Rendering.Analytics;
using UnityEditorInternal;
using UnityEngine;

public class EnemyRange : MonoBehaviour
{
    public string tagToIgnore = "IgnoreThisTag";
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public float moveSpeed = 0.75f;
    public float chaseMoveSpeed = 2f;
    public float patrolMoveSpeed = 2f;
    public float jumpForce = 5f;
    public float visionRadius = 10f;
    public LayerMask groundLayer; // Для проверки земли и препятствий
    private float groundCheckRadius;
    public float attackRange = 2;
    public float tooClose = 5;
    public float runAwayTime = 2.5f;
    public float obstacleCheckDistance = 0.5f; // Как далеко искать препятствия перед собой
    public GameObject patrolPoint1 = null;
    public GameObject patrolPoint2 = null;
    private float shootTimer = 0f;
    protected GameObject player;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Collider2D collider1;
    private Vector2 moveDirection = Vector2.zero;
    private bool OnPatrol = true;
    private float ChaseTimer;
    public float ChaseForSeconds = 3f;
    private Transform LastSeenPos;
    private bool GoToPointA = false;
    private SpriteRenderer lookdir;
    private float fliptimer = 0;


    private bool isRunningAway = false;
    private float runAwayTimer = 0f;
    


    public Animator anim;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        collider1 = GetComponent<Collider2D>();
        lookdir = GetComponent<SpriteRenderer>();

        // Автоматически настраиваем размеры проверок по коллайдеру
        groundCheckRadius = (collider1.bounds.size.y / 2) + 0.2f;
        obstacleCheckDistance = (collider1.bounds.size.x / 2) + 0.3f;
    }

    protected virtual void Update()
    {
        if (player == null) return;
        anim.SetBool("BackAway",isRunningAway);
        shootTimer += Time.deltaTime;
        fliptimer += Time.deltaTime;    
        Vector2 direction = new Vector2(Mathf.Sign(moveDirection.x), 0f);
        if (fliptimer > 0.5f && !isRunningAway)
        {
            if (direction.x < 0f)
            {
                lookdir.flipX = true;
            }
            else
            {
                lookdir.flipX = false;
            }
            fliptimer =0;
        }

        anim.SetFloat("Horizontal Velocity", Mathf.Abs(rb.linearVelocityX));

        // Проверка на землю
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (OnPatrol)
        {
            if (patrolPoint1 != null && patrolPoint2 != null)
            {
                Patrol();
            }
            if (CanSeePlayer())
            {
                OnPatrol = false;
            }
        }
        else
        {
            ChasePlayer();
        }


    }

    protected virtual void ShootAtPlayer()
    {
        if (bulletPrefab == null) return;

        Vector2 shootDirection = (player.transform.position - transform.position).normalized;

        // Спавним пулю немного перед врагом, чтобы она не исчезала
        Vector2 spawnPosition = (Vector2)transform.position + shootDirection * (obstacleCheckDistance + 0.2f);
        //anim.SetTrigger("AttackTrigger");
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        anim.SetTrigger("AttackTrigger");

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.lifeTime = 3f;
            bulletScript.direction = shootDirection;
            bulletScript.shooter = gameObject;
        }
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, groundLayer);

        // Если луч не наткнулся на стены — игрок виден
        return hit.collider == null && distanceToPlayer <= visionRadius;
    }

    bool IsObstacleAhead()
    {
        Vector2 direction = new Vector2(Mathf.Sign(moveDirection.x), 0f); // Право или лево, куда движется враг
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, (collider1.bounds.size.y / 4)), direction, obstacleCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnDrawGizmosSelected()
    {
        // Радиус зрения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        // Линия к игроку
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }

    void ChasePlayer()
    {
        moveSpeed = chaseMoveSpeed;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (CanSeePlayer())
        {
            ChaseTimer = 0;
            LastSeenPos = player.transform;
            distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (isRunningAway)
            {
                runAwayTimer += Time.deltaTime;
                moveDirection = -1 * (player.transform.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
                if (runAwayTimer >= runAwayTime)
                {
                    isRunningAway = false;
                    runAwayTimer = 0;   
                }
            }
            else
            {
                moveDirection = (player.transform.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
            }
            // Двигаемся к игроку

            if (distanceToPlayer <= tooClose)
            {
                isRunningAway = true;
            }

            // Прыгаем через препятствия
            if (isGrounded && IsObstacleAhead())
            {
                Jump();
            }

            // Стреляем по таймеру
            if (shootTimer >= shootInterval && distanceToPlayer <= attackRange)
            {
                shootTimer = 0f;
                ShootAtPlayer();
            }
        }
        else
        {
            distanceToPlayer = Vector2.Distance(transform.position, LastSeenPos.position);
            // Двигаемся к игроку
            moveDirection = (LastSeenPos.transform.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

            // Прыгаем через препятствия
            if (isGrounded && IsObstacleAhead())
            {
                Jump();
            }

            // Стреляем по таймеру
            if (shootTimer >= shootInterval && distanceToPlayer <= attackRange)
            {
                shootTimer = 0f;
                ShootAtPlayer();
            }
            ChaseTimer += Time.deltaTime;
            if (ChaseTimer >= ChaseForSeconds)
            {
                OnPatrol = true;
            }
        }
    }

    void Patrol()
    {
        moveSpeed = patrolMoveSpeed;
        if (GoToPointA)
        {
            moveDirection = (patrolPoint2.transform.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(Math.Sign(moveDirection.x) * moveSpeed, rb.linearVelocity.y);
            if (isGrounded && IsObstacleAhead())
            {
                Jump();
            }
            if (moveDirection.x > 0)
            {
                if (transform.position.x >= patrolPoint2.transform.position.x - 0.5f)
                {
                    GoToPointA = false;
                }
            }
            else
            {
                if (transform.position.x <= patrolPoint2.transform.position.x + 0.5f)
                {
                    GoToPointA = false;
                }
            }
        }
        else
        {
            moveDirection = (patrolPoint1.transform.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(Math.Sign(moveDirection.x) * moveSpeed, rb.linearVelocity.y);
            if (isGrounded && IsObstacleAhead())
            {
                Jump();
            }
            if (moveDirection.x > 0)
            {
                if (transform.position.x >= patrolPoint1.transform.position.x - 0.5f)
                {
                    GoToPointA = true;
                }
            }
            else
            {
                if (transform.position.x <= patrolPoint1.transform.position.x + 0.5f)
                {
                    GoToPointA = true;
                }
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagToIgnore))
        {
            Debug.Log("НЕА");
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider); // Игнорируем столкновение
        }
    }
}
