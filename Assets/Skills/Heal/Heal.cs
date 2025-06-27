using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Skills/Heal")]
public class Heal : Skill
{
    public int healAmount = 20;
    public Color healColor = Color.green;
    //public GameObject player;
    private SpriteRenderer spr;
    public float greenTime = 1f;
    public GameObject HealPrefab;


    public override void Activate(GameObject user)
    {
        PlayerHealth health = user.GetComponent<PlayerHealth>();
        spr = user.GetComponent<SpriteRenderer>();
        base.Activate(user);
        health.health = Mathf.Min(health.health + healAmount, health.maxhealth);
        GameObject healpref = Instantiate(HealPrefab,user.transform.position,Quaternion.identity);
        Destroy(healpref,greenTime);
        // ������������� ����� �������� ������ �����, �������� � �.�.
        // ��������, ������ �� FireballPrefab, ������� ������������ ������������.
    }

    private void changeBack()
    {
        spr.color = Color.white;
    }
}