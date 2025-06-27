using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;  // ������ ����
    public float bulletSpeed = 10f;  // �������� ����
    public float bulletOffset = 1f; // �������� ������� ��� ��������
    public Transform shootPoint;    // ����� �������� (�� �����������, ����� ������������ ������)
    public float shotCD = 0.5f; 
    private bool isShooting = false; // ����, ��� ����� ��������
    private Vector2 shootDirection;  // ����������� ��������

    private void Update()
    {
        // ��������� ����� (J ��� �������� �����, K ��� �������� ������)
        if (!isShooting)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Shoot(Vector2.left); // �������� �����
                isShooting = true;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                Shoot(Vector2.right); // �������� ������
                isShooting = true;
            }
        }
        else
        {
            isShooting = false;
        }
    }

    // ����� ��� ��������
    void Shoot(Vector2 direction)
    {

        // ��������, ��� ������ ���� ��������
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab �� ��������! �������� �� ����� ���� ���������.");
            return;  // ���������� ����������, ���� ���� �� ���������
        }

        // ������� ��� ����, ������� �� �����������
        Vector3 spawnPos = transform.position + (Vector3)(direction.normalized * bulletOffset);

        // �������� �������� ����

        // ������ ����
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // �������� ��������� Bullet � ������ �����������
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.direction = direction;  // ����������� ����
            bulletScript.shooter = gameObject;   // ����������� ������ ��� ��������� ����
            bulletScript.speed = bulletSpeed;
        }
    }
}
