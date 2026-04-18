using UnityEngine;

public class CameraRTS : MonoBehaviour
{ 
    public float speed = 250f;
    public int camera_size = 17;
    public float camera_speed_variation = 2.0f;

    void Start()
    {
        GetComponent<Camera>().orthographicSize = camera_size;
    }
    void Update()
    {
        scaleCamera();
        moveCamera();
    }

    void moveCamera()
    {
        float mouseX = Input.GetAxis("Mouse X"); // horizontal movement
        float mouseY = Input.GetAxis("Mouse Y"); // vertical movement

        if (Input.GetMouseButton(2))
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();

            transform.position -= right * mouseX * Time.deltaTime * speed * camera_speed_variation;
            transform.position -= forward * mouseY * Time.deltaTime * speed * camera_speed_variation;
        }
    }

    void scaleCamera()
    {
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if ((scroll > 0) && (camera_size > 10))
        {
            camera_size -= 1;
            GetComponent<Camera>().orthographicSize = camera_size;
            camera_speed_variation -= 0.14285f;
        }
            
        if ((scroll < 0) && (camera_size < 24))
        {
            camera_size += 1;
            GetComponent<Camera>().orthographicSize = camera_size;
            camera_speed_variation += 0.14285f;
        }
    }
}