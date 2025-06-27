using UnityEngine;

public class Boss : Enemy
{
    [Header("Triple Shot Settings")]
    public float angleBetweenBullets = 15f;  // Угол между пулями в тройном выстреле

    [Header("Circular Shot Settings")]
    public int circularBullets = 8;         // Количество пуль в круговом обстреле
    public float circularShotInterval = 4f; // Как часто стреляет по кругу
    public float timeBetweenCircularShots = 0.2f; // Задержка между пулями (для эффекта волны)

    private float circularShotTimer = 0f;
    private bool isCircularAttacking = false;

    protected override void Update()
    {
        base.Update(); // Базовая логика преследования и стрельбы

        if (player == null) return;

        // Круговой обстрел по таймеру
        circularShotTimer += Time.deltaTime;
        if (circularShotTimer >= circularShotInterval && !isCircularAttacking)
        {
            StartCoroutine(CircularAttack());
        }
    }

    // Переопределяем стрельбу (тройной выстрел вместо одиночного)
    protected override void ShootAtPlayer()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = (player.transform.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + shootDirection * (obstacleCheckDistance + 0.2f);

        // Центральная пуля
        ShootSingleBullet(spawnPosition, shootDirection);

        // Две дополнительные под углом
        Vector2 leftDir = Quaternion.Euler(0, 0, angleBetweenBullets) * shootDirection;
        Vector2 rightDir = Quaternion.Euler(0, 0, -angleBetweenBullets) * shootDirection;
        ShootSingleBullet(spawnPosition, leftDir);
        ShootSingleBullet(spawnPosition, rightDir);
    }

    // Круговой обстрел (корутина для эффекта "волны")
    private System.Collections.IEnumerator CircularAttack()
    {
        isCircularAttacking = true;
        circularShotTimer = 0f;

        float angleStep = 360f / circularBullets;
        //Vector2 spawnPos = transform.position;

        for (int i = 0; i < circularBullets; i++)
        {
            Vector2 spawnPos = transform.position;
            float angle = i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
            ShootSingleBullet(spawnPos, dir);

            yield return new WaitForSeconds(timeBetweenCircularShots); // Задержка между пулями
        }

        isCircularAttacking = false;
    }

    private void ShootSingleBullet(Vector2 position, Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.direction = direction;
            bulletScript.shooter = gameObject;
        }
    }

    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
