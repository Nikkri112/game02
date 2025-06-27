using UnityEngine;
using System.Collections; // Для корутин

public class MovingPlatform : MonoBehaviour
{
    // Точки, между которыми будет двигаться платформа
    public Vector2[] waypoints;
    // Скорость движения платформы
    public float speed = 2f;
    // Задержка в конечных точках (перед тем как двинуться обратно)
    public float waitTimeAtWaypoint = 1f;

    private int currentWaypointIndex = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Если нет точек, платформа не будет двигаться
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("MovingPlatform: Укажите хотя бы 2 точки для движения.", this);
            enabled = false; // Отключаем скрипт, если точек недостаточно
            return;
        }

        // Убедимся, что начальная позиция платформы соответствует первой точке маршрута
        // Если вы хотите, чтобы платформа начинала с текущей позиции, просто удалите эту строку.
        //transform.position = waypoints[0]; // Можно использовать это, если хотите, чтобы платформа всегда начинала с первой точки
        // Однако, для размещения в редакторе лучше, чтобы платформа оставалась там, где вы её поставили.

        // Запускаем корутину для начала движения
        StartCoroutine(MoveBetweenWaypoints());
    }

    IEnumerator MoveBetweenWaypoints()
    {
        while (true) // Бесконечный цикл для постоянного движения
        {
            // Получаем текущую целевую точку
            Vector2 targetPosition = waypoints[currentWaypointIndex];

            // Двигаемся к целевой точке
            while (Vector2.Distance(rb.position, targetPosition) > 0.05f) // Достаточно близко к точке
            {
                // Вычисляем новое положение
                Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
                rb.MovePosition(newPosition);
                yield return null; // Ждем следующий кадр
            }

            // Убеждаемся, что платформа точно в целевой точке, чтобы избежать накопления ошибок
            rb.position = targetPosition;

            // Ждем в конечной точке
            yield return new WaitForSeconds(waitTimeAtWaypoint);

            // Переходим к следующей точке
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Циклический переход
        }
    }

    // Это опционально: можно нарисовать линии маршрута в редакторе для удобства
    void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.1f); // Рисуем сферу в каждой точке
                if (i < waypoints.Length - 1)
                {
                    Gizmos.DrawLine(waypoints[i], waypoints[i + 1]); // Рисуем линию между точками
                }
            }
            // Рисуем линию от последней точки к первой, чтобы показать цикл
            if (waypoints.Length > 1)
            {
                Gizmos.DrawLine(waypoints[waypoints.Length - 1], waypoints[0]);
            }
        }
    }
}