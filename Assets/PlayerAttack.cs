using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public GameObject[] attackPrefabs;
    // public Transform attackSpawnPoint; // Мы будем рассчитывать эту точку динамически
    public float attackCooldown = 0.5f;
    public float projectileSpeed = 10f;
    public float attackAnimationDuration = 0.7f;
    public float attackPush = 3f;

    public MonoBehaviour playerMovementScript;

    public Animator AttackAnimator;

    // --- Новые поля для расчета точки спавна ---
    public float forwardOffset = 1.0f; // Насколько далеко вперед от игрока спавнится снаряд
    // Если у вас нет CharacterController, можно использовать эту высоту вручную
    // public float playerHeight = 1.8f; // Примерная высота модели игрока

    private int currentAttackIndex = 0;
    private float lastAttackTime;
    private bool isAttacking = false;

    private CharacterController characterController; // Для получения высоты игрока, если есть
    private Collider2D playerCollider; // Общий коллайдер игрока
    private Rigidbody2D prb;

    private string[] triggerstrings = { "PunchDown", "PunchUp", "PunchDown" };

    void Start()
    {
        prb = GetComponent<Rigidbody2D>();  
        lastAttackTime = -attackCooldown; // Инициализируем, чтобы первая атака была сразу доступна
        // Попытка получить CharacterController или Collider игрока
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }
        if (characterController == null && playerCollider == null)
        {
            Debug.LogWarning("No CharacterController or Collider found on the player. Attack spawn point calculation might be inaccurate. Consider assigning playerHeight manually if needed.");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(PerformAttackSequence());
            //AttackAnimator.SetTrigger("PunchAttack");
        }
    }

    IEnumerator PerformAttackSequence()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        Vector3 scale = transform.localScale;
        prb.linearVelocity = transform.right*Mathf.Sign(scale.x)*1.5f;
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
            Debug.Log("Player movement disabled.");
        }
        else
        {
            Debug.LogWarning("Player movement script not assigned in PlayerAttack script!");
        }
        

        // --- Расчет точки спавна атаки ---
        Vector3 spawnPosition = CalculateAttackSpawnPoint();
        Quaternion spawnRotation = transform.rotation; // Снаряд смотрит туда же, куда и игрок

        // Воспроизводим анимацию атаки (если у вас есть Animator)
        // GetComponent<Animator>().SetTrigger("Attack");

        GameObject currentPrefab = attackPrefabs[currentAttackIndex];

        //AttackAnimator.SetTrigger(triggerstrings[currentAttackIndex]);
        

        // Создаем экземпляр префаба
        GameObject projectile = Instantiate(currentPrefab, spawnPosition, spawnRotation);
        projectile.transform.localScale = new Vector3(Mathf.Sign(scale.x) * projectile.transform.localScale.x, projectile.transform.localScale.y, projectile.transform.localScale.z);
        MeleeAttackHitbox meleeAttackHitbox = projectile.GetComponent<MeleeAttackHitbox>();
        meleeAttackHitbox.attacker = gameObject;
        //ActivateAttack();
        Debug.Log("я ебаный долбаеб");

        currentAttackIndex = (currentAttackIndex + 1) % attackPrefabs.Length;

        AttackAnimator.SetTrigger(triggerstrings[currentAttackIndex]);
        yield return new WaitForSeconds(attackAnimationDuration);
        if (playerMovementScript != null)
        {   
            playerMovementScript.enabled = true;
            Debug.Log("Player movement enabled.");
        }

        isAttacking = false;
    }

    // --- Новая функция для расчета точки спавна ---
    Vector3 CalculateAttackSpawnPoint()
    {
        Vector3 playerCenter = transform.position; // Начальная позиция - у основания игрока

        if (characterController != null)
        {
            // Используем центр CharacterController для более точного позиционирования по высоте
            playerCenter = transform.position + characterController.center;
        }
        else if (playerCollider != null)
        {
            // Используем центр коллайдера
            playerCenter = playerCollider.bounds.center;
            Debug.Log(playerCenter);
        }
        // Если нет ни CharacterController, ни Collider, предполагаем, что transform.position это основание
        // и добавим половину "средней" высоты игрока
        else
        {
            // Если вы знаете точную высоту вашей модели, используйте ее:
            // playerCenter.y += playerHeight / 2f;
            // Иначе, можно использовать некое усредненное значение, или оставить как есть, если игрок "центрирован"
            playerCenter.y += 0.9f; // Примерно середина для стандартного персонажа высотой 1.8м
        }
        Vector3 scale = transform.localScale;
        // Добавляем отступ по направлению взгляда игрока
        Vector3 spawnPoint = playerCenter + transform.right * Mathf.Sign(scale.x) * forwardOffset;
        AttackAnimator.SetTrigger("PunchAttack");
        return spawnPoint;
    }
}