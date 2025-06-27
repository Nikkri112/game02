using UnityEngine;
using UnityEngine.UI;

public class SpawnEnemiesFromSlider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Slider slider;
    public GameObject spawnPoint;
    public GameObject spawnEnemy;
    public GameObject EnemyBullet;
    public GameObject myinterface;
    public float EnemyVisionRadius = 10f;
    public float timeBetweenSpawns = 0.5f;
    private int numberOfEnemies;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ipressedButton()
    {
        numberOfEnemies = (int)Mathf.Floor(slider.value);
        myinterface.SetActive(false);
        StartCoroutine(SpawningEnems());

    }

    private System.Collections.IEnumerator SpawningEnems()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enem = Instantiate(spawnEnemy,spawnPoint.transform.position, Quaternion.identity);
            Enemy enemscript = enem.GetComponent<Enemy>();
            enemscript.bulletPrefab = EnemyBullet;
            enemscript.visionRadius = EnemyVisionRadius;
            yield return new WaitForSeconds(timeBetweenSpawns); // Задержка между spawnami
        }
    }

}
