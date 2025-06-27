using UnityEngine;

public class ZombAttack : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 10f;
    public GameObject shooter; // ��� �������� ����
    public float lifeTime = 3f; // ����� ������� ������ ���������� ����
    public int BulletDMG = 1;
    private SpriteRenderer spr;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();   
        Destroy(gameObject, lifeTime); // ��������������� ����� �����
        if (direction.x < 0)
        {
            spr.flipX = true;
        }
        else
        {
            spr.flipX = false;
        }
    }

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.linearVelocity.x <= 0)
            {
                transform.localScale = new Vector3(-1 * transform.localScale.x, 0, 0);
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
        else
        {
            if (collision.CompareTag("IgnoreThisTag"))
            {
                //������� ���� ��� ����
                return;
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
}

