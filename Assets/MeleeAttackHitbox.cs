using UnityEngine;
using System.Collections.Generic; // ��� ������������� HashSet

public class MeleeAttackHitbox : MonoBehaviour
{
    public int attackDamage = 1; // ����, ������� ������� ��� �����
    public GameObject attacker; // ������ �� GameObject, ������� ��������� ����� (��������, �����)
    public float lifetime = 0.5f;

    // ���������� HashSet ��� ������������ ��� ������� �������� �� ���� ����, ����� �� �������� ���� ��������� ���
    private HashSet<GameObject> hitObjectsThisAttack = new HashSet<GameObject>();


    private void Start()
    {
        Destroy(gameObject, lifetime); // ��������������� ����� �����
    }

    // ����������, ����� ���������-������� ������ � �������
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }
    private void HandleHit(GameObject hitObject)
    {
        // ���������� ��������� � ������ ���������� ��� �������, ������� ��� ���� ������ � ���� �����
        if (hitObject == attacker || hitObjectsThisAttack.Contains(hitObject))
        {
            return;
        }

        // ��������� ������ � ������ ��� �������, ����� �� �������� ���� �������� �� ���� ����
        hitObjectsThisAttack.Add(hitObject);

        // --- ������ ��������� ����� ---
        // ���� ������ �� �����
        if (hitObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = hitObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log($"Melee hit: {hitObject.name}, damage: {attackDamage}");
            }
        }
        // ���� ������ � ������� ������ (���� ���� ����������� ��� �����-������)
        else if (hitObject.CompareTag("Player"))
        {
            // ��������, ��� ��� �� ��� �� �����, ������� ��������
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
        // �������� ������ ����, � �������� �� ������ ����������������� (��������, ����������� �������)
        // else if (hitObject.CompareTag("Destructible"))
        // {
        //     DestructibleObject obj = hitObject.GetComponent<DestructibleObject>();
        //     if (obj != null)
        //     {
        //         obj.TakeDamage(attackDamage);
        //     }
        // }
    }

    // ���� ����� ����� �������� � ������ ������ ����� �����
    // ������ ���������� �� ������� ������, ������� ���������� Hitbox
    public void ResetHitList()
    {
        hitObjectsThisAttack.Clear();
    }

    // �����������: ����� �������� ������� ��� ��������� (����, �������)
    // public void PlayHitEffect(Vector3 hitPosition)
    // {
    //     // Instantiate particle effect at hitPosition
    //     // Play sound
    // }
}