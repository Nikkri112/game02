using UnityEngine;
using System.Collections.Generic; // Для использования HashSet

public class MeleeAttackHitbox : MonoBehaviour
{
    public int attackDamage = 1; // Урон, который наносит эта атака
    public GameObject attacker; // Ссылка на GameObject, который выполняет атаку (например, игрок)
    public float lifetime = 0.5f;

    // Используем HashSet для отслеживания уже задетых объектов за один удар, чтобы не наносить урон несколько раз
    private HashSet<GameObject> hitObjectsThisAttack = new HashSet<GameObject>();


    private void Start()
    {
        Destroy(gameObject, lifetime); // самоуничтожение через время
    }

    // Вызывается, когда коллайдер-триггер входит в контакт
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }
    private void HandleHit(GameObject hitObject)
    {
        // Игнорируем попадание в самого атакующего или объекты, которые уже были задеты в этом ударе
        if (hitObject == attacker || hitObjectsThisAttack.Contains(hitObject))
        {
            return;
        }

        // Добавляем объект в список уже задетых, чтобы не наносить урон повторно за один удар
        hitObjectsThisAttack.Add(hitObject);

        // --- Логика нанесения урона ---
        // Если попали во врага
        if (hitObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = hitObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log($"Melee hit: {hitObject.name}, damage: {attackDamage}");
            }
        }
        // Если попали в другого игрока (если есть мультиплеер или враги-игроки)
        else if (hitObject.CompareTag("Player"))
        {
            // Убедимся, что это не тот же игрок, который атаковал
            if (hitObject != attacker)
            {
                PlayerHealth player = hitObject.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(attackDamage);
                    Debug.Log($"Melee hit player: {hitObject.name}, damage: {attackDamage}");
                }
            }
        }
        // Добавьте другие теги, с которыми вы хотите взаимодействовать (например, разрушаемые объекты)
        // else if (hitObject.CompareTag("Destructible"))
        // {
        //     DestructibleObject obj = hitObject.GetComponent<DestructibleObject>();
        //     if (obj != null)
        //     {
        //         obj.TakeDamage(attackDamage);
        //     }
        // }
    }

    // Этот метод нужно вызывать в начале каждой новой атаки
    // Обычно вызывается из скрипта игрока, который активирует Hitbox
    public void ResetHitList()
    {
        hitObjectsThisAttack.Clear();
    }

    // Опционально: можно добавить эффекты при попадании (звук, частицы)
    // public void PlayHitEffect(Vector3 hitPosition)
    // {
    //     // Instantiate particle effect at hitPosition
    //     // Play sound
    // }
}