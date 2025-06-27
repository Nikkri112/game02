using UnityEngine;

[CreateAssetMenu(fileName = "FireballSkill", menuName = "Skills/Fireball")]
public class FireballSkill : Skill
{
    public GameObject fireballPrefab; // ������ ��������� ����
    public float fireballSpeed = 10f;
    public float damage = 20f;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
        Vector2 direction = user.transform.right; // ��� user.GetComponent<PlayerMovement>().FacingDirection;

        
        if (user.transform.localScale.x < 0) 
        {
            direction = -user.transform.right;
        }


        GameObject fireball = Instantiate(fireballPrefab, user.transform.position + (Vector3)direction * 1f, Quaternion.identity);
        if (user.transform.localScale.x < 0) // ������ ��� 2D, ��� scale.x < 0 �������� ��������
        {
            fireball.transform.localScale = new Vector3(-1 * fireball.transform.localScale.x, fireball.transform.localScale.y, fireball.transform.localScale.z);
        }
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * fireballSpeed;
        }

        // ������������� ����� �������� ������ �����, �������� � �.�.
        // ��������, ������ �� FireballPrefab, ������� ������������ ������������.
    }
}