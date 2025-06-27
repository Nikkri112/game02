using UnityEngine;
using System.Linq; // Для использования LINQ для поиска ближайшего врага

[CreateAssetMenu(fileName = "LightningStrikeSkill", menuName = "Skills/Lightning Strike")]
public class LightningStrikeSkill : Skill
{
    [Header("Настройки Lightning Strike")]
    public int damage = 50;
    public GameObject lightningEffectPrefab;
    public LayerMask targetLayer; // Изменил имя с 'enemyLayer' на 'targetLayer' для гибкости
    public string targetTag = "Enemy"; // Новый параметр: тег цели
    public float searchRadius = 20f;
    public float strikeOffsetFromTop = 5f;

    public override void Activate(GameObject user)
    {
        base.Activate(user); // Вызов базового метода для отладочного сообщения

        // 1. Находим ближайшего врага
        GameObject nearestEnemy = FindNearestEnemy(user.transform.position);

        if (nearestEnemy != null)
        {
            Vector3 targetPosition = nearestEnemy.transform.position;

            // 2. Создаем луч молнии (визуальный эффект)
            if (lightningEffectPrefab != null)
            {
                // Определяем начальную точку луча высоко над целью
                // Используем координаты камеры для определения "верха экрана"
                Camera mainCamera = Camera.main;
                Vector3 screenTopWorldPoint = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, mainCamera.nearClipPlane));
                Vector3 lightningSpawnPosition = new Vector3(targetPosition.x, screenTopWorldPoint.y + strikeOffsetFromTop, targetPosition.z);

                // Создаем эффект
                GameObject lightningEffect = Instantiate(lightningEffectPrefab, lightningSpawnPosition, Quaternion.identity);

                // Настраиваем эффект, чтобы он тянулся до врага
                // Предполагаем, что префаб молнии имеет подходящий компонент (например, Line Renderer или Sprite Renderer)
                // Для Line Renderer можно задать начальную и конечную точку
                LineRenderer lineRenderer = lightningEffect.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, lightningSpawnPosition);
                    lineRenderer.SetPosition(1, targetPosition);
                }
                // Если это Sprite Renderer, то можно масштабировать спрайт по высоте
                else
                {
                    // Для спрайта можно вычислить его желаемую высоту и масштабировать его
                    // Это более сложно, так как спрайт должен быть вытянут
                    // ИЛИ префаб должен быть создан так, чтобы он выглядел как луч сам по себе
                    // В данном случае, мы просто создаем его над врагом.
                    // Для эффекта "проходит сквозь всю карту" лучше использовать Line Renderer.
                    Debug.LogWarning("LightningStrikeSkill: lightningEffectPrefab не имеет LineRenderer. Для эффекта луча сквозь всю карту, рекомендуется использовать LineRenderer.");
                }

                // Уничтожаем эффект через некоторое время
                Destroy(lightningEffect, 1f); // Эффект длится 1 секунду
            }

            // 3. Применяем урон к врагу (здесь нужна ваша система здоровья врагов)
            EnemyHealth damageableEnemy = nearestEnemy.GetComponent<EnemyHealth>(); // Ваш интерфейс или компонент здоровья
            if (damageableEnemy != null)
            {
                damageableEnemy.TakeDamage(damage);
                Debug.Log($"Молния поразила {nearestEnemy.name} на {damage} урона.");
            }
            else
            {
                Debug.LogWarning($"У {nearestEnemy.name} нет компонента IDamageable!");
            }
        }
        else
        {
            Debug.Log("Нет врагов в радиусе действия Lightning Strike.");
        }
    }

    // Метод для поиска ближайшего врага
    private GameObject FindNearestEnemy(Vector3 origin)
    {
        // 1. Используем Physics2D.OverlapCircleAll для поиска всех коллайдеров в радиусе на указанном слое.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(origin, searchRadius, targetLayer);

        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Debug.Log("Вьебу ща");

        // 2. Теперь проходим по полученным коллайдерам и фильтруем по тегу и другим критериям.
        foreach (Collider2D hitCollider in hitColliders)
        {
            // Убедитесь, что найденный объект не является самим игроком (если это применимо)
            // И что он имеет нужный тег
            if (hitCollider.CompareTag(targetTag))
            {
                // Опционально: Дополнительная проверка на наличие нужного компонента (например, IDamageable)
                float distance = Vector2.Distance(origin, hitCollider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = hitCollider.gameObject;
                }
            }
        }
        return nearestEnemy;
    }
}