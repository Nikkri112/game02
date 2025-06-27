using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10f;
    public GameObject shooter; // ��� �������� ����
    public float lifeTime = 3f; // ����� ������� ������ ���������� ����
    public int BulletDMG = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // ��������������� ����� �����
    }

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.linearVelocity.x <= 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, 0,0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == shooter)
        {
            // ���������� ��������� � ���������
            return;
        }
        if (collision.CompareTag("IgnoreThisTag"))
        {
            //������� ���� ��� ����
            return;
        }

        // ����� ����� ��������� ����
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(BulletDMG);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
            return;
        }
       // ���� ������������ ��� ����� ������������
       
    }
}
