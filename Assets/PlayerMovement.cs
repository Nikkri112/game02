using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 8f;
    public float acceleration = 70f;
    public float deceleration = 50f;
    public float airAcceleration = 30f;
    public float inertiaDuration = 0.1f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float fallMultiplier = 2f; // Ускорение падения

    private float groundedRadius;

    [Header("Wall Interaction")]
    public float wallSlideSpeed = 2f;    // Скорость скольжения по стене
    public float wallJumpForceX = 10f;  // Горизонтальная сила прыжка от стены
    public float wallJumpForceY = 12f;   // Вертикальная сила прыжка от стены
    public float wallJumpDuration = 0.2f; // Время, когда игрок не может управлять движением после прыжка от стены4
    private float leftrightRayDistance;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    public int maxAirDashes = 1;        // Количество рывков в воздухе

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isFacingRight = true;
    private bool isDashing;
    private bool canDash = true;
    private int airDashesLeft;

    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float lastWallJumpTime;
    private float lastDashTime;
    private float velocityXSmoothing;

    public Animator anim;

    private Collider2D collider1;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        airDashesLeft = maxAirDashes;
        collider1 = GetComponent<Collider2D>();
        groundedRadius = (collider1.bounds.size.y/2)+0.2f;
        leftrightRayDistance = (collider1.bounds.size.x/2)+0.2f;
        
    }

    private void Update()
    {
        
        moveInput = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("IsRunning",Math.Abs(rb.linearVelocityX));
        // Проверка земли и стен
        CheckCollisions();

        // Обработка прыжков
        HandleJump();

        // Обработка рывка (Dash)
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            TryDash();
        }

        // Поворот спрайта
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
        anim.SetBool("WallSliding", isWallSliding);

        // Ускоренное падение
        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            gameObject.tag = "IgnoreThisTag";
            gameObject.layer = 8;
            return; // Не управлять движением во время рывка
        }
        else {
            gameObject.layer = 0;
            gameObject.tag = "Player";
        }
        UpdateMovement();
        HandleWallSlide();
    }

    //=== ОСНОВНОЕ ДВИЖЕНИЕ ===//
    private void UpdateMovement()
    {
        float targetVelocityX = moveInput * maxSpeed;
        float currentAcceleration;

        // Если игрок прижат к стене и не скользит, запрещаем движение
        if (isTouchingWall && !isGrounded && moveInput != 0 && Mathf.Sign(moveInput) == Mathf.Sign(transform.localScale.x))
        {
            targetVelocityX = 0;
        }

        // Выбираем ускорение в зависимости от состояния
        if (isGrounded)
        {
            currentAcceleration = (Mathf.Sign(moveInput) != Mathf.Sign(rb.linearVelocity.x)) ? deceleration : acceleration;
        }
        else
        {
            currentAcceleration = airAcceleration;
        }

        // Плавное изменение скорости
        float newVelocityX = Mathf.SmoothDamp(
            rb.linearVelocity.x,
            targetVelocityX,
            ref velocityXSmoothing,
            inertiaDuration,
            currentAcceleration
        );

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
    }

    //=== ПРЫЖКИ ===//
    private void HandleJump()
    {
        // Обновляем таймеры
        if (isGrounded)
        {
            lastGroundedTime = coyoteTime;
            airDashesLeft = maxAirDashes; // Сброс рывков при приземлении
        }
        else
        {
            lastGroundedTime -= Time.deltaTime;
        }

        // Буфер прыжка
        if (Input.GetButtonDown("Jump"))
        {
            lastJumpPressedTime = jumpBufferTime;
        }
        else
        {
            lastJumpPressedTime -= Time.deltaTime;
        }

        // Обычный прыжок с земли
        if (lastJumpPressedTime > 0 && lastGroundedTime > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpPressedTime = 0;
            lastGroundedTime = 0;
            anim.SetTrigger("JumpTrigger");
        }
        // Прыжок от стены
        else if (lastJumpPressedTime > 0 && isWallSliding)
        {
            WallJump();
            anim.SetTrigger("JumpTrigger");
        }
    }

    //=== СТЕННОЕ ВЗАИМОДЕЙСТВИЕ ===//
    private void HandleWallSlide()
    {
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(-wallSlideSpeed, rb.linearVelocity.y));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        float direction = -Mathf.Sign(transform.localScale.x); // Прыжок в противоположную сторону от стены
        rb.linearVelocity = new Vector2(direction * wallJumpForceX, wallJumpForceY);
        lastJumpPressedTime = 0;
        lastWallJumpTime = wallJumpDuration;
    }

    //=== РЫВОК (DASH) ===//
    private void TryDash()
    {
        if (!canDash || (isGrounded && airDashesLeft <= 0)) return;

        if (!isGrounded)
        {
            airDashesLeft--;
        }

        isDashing = true;
        canDash = false;
        lastDashTime = dashDuration;
        //anim.SetBool("IsSliding", true);
        Debug.Log("Дешит");
        anim.SetTrigger("SlideTrigger");
        // Направление рывка (в сторону движения или просто вперёд, если стоит)
        float dashDirection = moveInput != 0 ? Mathf.Sign(moveInput) : (isFacingRight ? 1 : -1);
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);

        Invoke(nameof(StopDash), 0.35f);
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void StopDash()
    {
        //anim.SetBool("IsSliding", isDashing);
        isDashing = false;
        Debug.Log("Не дешит");
        //anim.SetBool("IsSliding", false);
    }

    private void ResetDash()
    {
        canDash = true;
    }

    //=== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ===//
    private void CheckCollisions()
    {
        // Проверка земли (можно заменить на Raycast или CircleCast)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundedRadius, LayerMask.GetMask("Ground")) || Physics2D.Raycast(transform.position, Vector2.down, groundedRadius, LayerMask.GetMask("EnemyHead"));
        // Проверка стены (используем Raycast в сторону, куда смотрит игрок
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(transform.position, rayDirection, leftrightRayDistance, LayerMask.GetMask("Ground"));
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        // Визуализация Raycast для стены
        Gizmos.color = Color.red;
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rayDirection * leftrightRayDistance));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Vector2.down * groundedRadius));
    }
    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}