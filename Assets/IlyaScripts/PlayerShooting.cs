using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;  // Префаб пули
    public float bulletSpeed = 10f;  // Скорость пули
    public float bulletOffset = 1f; // Смещение позиции для выстрела
    public Transform shootPoint;    // Точка выстрела (не обязательно, можно использовать игрока)
    public float shotCD = 0.5f; 
    private bool isShooting = false; // Флаг, что игрок стреляет
    private Vector2 shootDirection;  // Направление выстрела

    private void Update()
    {
        // Обработка ввода (J для стрельбы влево, K для стрельбы вправо)
        if (!isShooting)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Shoot(Vector2.left); // Стрельба влево
                isShooting = true;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                Shoot(Vector2.right); // Стрельба вправо
                isShooting = true;
            }
        }
        else
        {
            isShooting = false;
        }
    }

    // Метод для выстрела
    void Shoot(Vector2 direction)
    {

        // Проверка, что префаб пули назначен
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab не назначен! Стрельба не может быть выполнена.");
            return;  // Прекращаем выполнение, если пуля не назначена
        }

        // Позиция для пули, зависит от направления
        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * bulletOffset);

        // Логируем создание пули

        // Создаём пулю
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // Получаем компонент Bullet и задаем направление
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.direction = direction;  // Направление пули
            bulletScript.shooter = gameObject;   // Присваиваем игрока как создателя пули
            bulletScript.speed = bulletSpeed;
        }
    }
}
