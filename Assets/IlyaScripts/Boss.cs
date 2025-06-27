using UnityEngine;

public class Boss : Enemy
{
    [Header("Triple Shot Settings")]
    public float angleBetweenBullets = 15f;  // ���� ����� ������ � ������� ��������

    [Header("Circular Shot Settings")]
    public int circularBullets = 8;         // ���������� ���� � �������� ��������
    public float circularShotInterval = 4f; // ��� ����� �������� �� �����
    public float timeBetweenCircularShots = 0.2f; // �������� ����� ������ (��� ������� �����)

    private float circularShotTimer = 0f;
    private bool isCircularAttacking = false;

    protected override void Update()
    {
        base.Update(); // ������� ������ ������������� � ��������

        if (player == null) return;

        // �������� ������� �� �������
        circularShotTimer += Time.deltaTime;
        if (circularShotTimer >= circularShotInterval && !isCircularAttacking)
        {
            StartCoroutine(CircularAttack());
        }
    }

    // �������������� �������� (������� ������� ������ ����������)
    protected override void ShootAtPlayer()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = (player.transform.position - transform.position).normalized;
        Vector2 spawnPosition = (Vector2)transform.position + shootDirection * (obstacleCheckDistance + 0.2f);

        // ����������� ����
        ShootSingleBullet(spawnPosition, shootDirection);

        // ��� �������������� ��� �����
        Vector2 leftDir = Quaternion.Euler(0, 0, angleBetweenBullets) * shootDirection;
        Vector2 rightDir = Quaternion.Euler(0, 0, -angleBetweenBullets) * shootDirection;
        ShootSingleBullet(spawnPosition, leftDir);
        ShootSingleBullet(spawnPosition, rightDir);
    }

    // �������� ������� (�������� ��� ������� "�����")
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

            yield return new WaitForSeconds(timeBetweenCircularShots); // �������� ����� ������
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

    // ������������ � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
