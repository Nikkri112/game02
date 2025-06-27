using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float camSpeed = 0.0001f;
    public float camDistance = 6f;
    private Camera cam;
    public GameObject Player;
    private Transform playerTransform;
    private Transform cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = camDistance;
        playerTransform = Player.GetComponent<Transform>();
        cameraTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        CamMovement();
    }

    public void CamMovement()
    {
        if (cameraTransform.position != playerTransform.position)
        {
            float differenceX = -1*(cameraTransform.position.x - playerTransform.position.x);
            float differenceY = -1 * (cameraTransform.position.y - playerTransform.position.y);
            cameraTransform.position += new Vector3(differenceX * camSpeed, differenceY * camSpeed);
        }
        else 
        {
            return;
        }
    }
}
