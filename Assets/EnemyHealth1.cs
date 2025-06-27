using UnityEngine;

public class EnemyHealth1 : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    private SpriteRenderer spr;
    public float timeOfRed = 0.5f;
    public Color color = Color.white;

    private void Start()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        spr.color = color;
        Invoke("ChangeColorBack", timeOfRed);
    }

    private void ChangeColorBack()
    {
        spr.color = Color.white;
    }
}
