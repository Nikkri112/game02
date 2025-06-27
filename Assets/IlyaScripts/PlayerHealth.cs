using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    public int maxhealth = 60;
    public float redTime = 0.5f;
    private SpriteRenderer spr;
    public Color color = Color.white;
    public Slider slider;
    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        slider.maxValue = maxhealth;
        slider.value = maxhealth;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Player Died!");
            Destroy(gameObject);
        }
        gameObject.layer = 8;
        spr.color = color;
        Invoke("ChangeBack", redTime);
    }

    public void ChangeBack()
    {
        gameObject.layer = 0;
        spr.color = Color.white;
    }

    private void Update()
    {
        slider.value = health;
    }
}