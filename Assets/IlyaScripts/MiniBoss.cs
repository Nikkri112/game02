using UnityEngine;

public class MiniBoss : Enemy
{
    [Header("Triple Shot Settings")]
    public float angleBetweenBullets = 15f; // ���� ����� ������

    // �������������� ����� ShootAtPlayer (������ �� virtual � Enemy)
    protected override void ShootAtPlayer()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = (player.transform.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + shootDirection * (obstacleCheckDistance + 0.2f);

        // ����������� ����
        ShootSingleBullet(spawnPosition, shootDirection);

        // ����� ���� (��� �����)
        Vector2 leftDirection = Quaternion.Euler(0, 0, angleBetweenBullets) * shootDirection;
        ShootSingleBullet(spawnPosition, leftDirection);

        // ������ ���� (��� �����)
        Vector2 rightDirection = Quaternion.Euler(0, 0, -angleBetweenBullets) * shootDirection;
        ShootSingleBullet(spawnPosition, rightDirection);
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
}