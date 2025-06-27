using UnityEngine;
using UnityEngine.UI;
public class BossRoomEnter : MonoBehaviour
{
    public GameObject healthbar;
    public GameObject door1;
    public GameObject door2;    
    public GameObject boss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "IgnoreThisTag")
        {
            boss.SetActive(true);
            door1.active = true;
            door2.active = true;
            healthbar.active = true;
            Destroy(gameObject);
        }
    }
}
