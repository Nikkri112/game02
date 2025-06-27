using UnityEngine;

public class RespawnIfFallen : MonoBehaviour
{
    public GameObject respPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -75)
        {
            transform.position = respPoint.transform.position;
        }
    }
}
