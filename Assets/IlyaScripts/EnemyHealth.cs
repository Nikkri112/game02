using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    private SpriteRenderer spr;
    public float timeOfRed = 0.5f;
    public Color color = Color.white;
    public GameObject endgameobj;

    private void Start()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        spr.color = color;
        Invoke("ChangeColorBack", timeOfRed);
    }

    private void ChangeColorBack()
    {
        spr.color = Color.white;    
    }

    private void Die()
    {
        if (endgameobj != null)
        {
            endgameobj.SetActive(true);
        }
        Destroy(gameObject,0.2f);
    }
}
