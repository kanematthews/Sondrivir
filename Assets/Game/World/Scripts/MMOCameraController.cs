using UnityEngine;

public class MMOCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Rotation")]
    public float rotationSpeed = 5f;
    public float verticalSpeed = 3f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    [Header("Smoothing")]
    public float smoothTime = 10f;

    [Header("Reset")]
    public float resetDelay = 10f;

    private float yaw = 0f;
    private float pitch = 20f;

    private float distance = 5f;
    private float defaultDistance = 5f;

    private float lastInputTime;

    void Start()
    {
        defaultDistance = distance;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        HandleRotation();
        HandleZoom();
        HandleReset();

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 direction = rotation * new Vector3(0, 0, -distance);

        transform.position = target.position + direction;
        transform.rotation = rotation;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * verticalSpeed;

            pitch = Mathf.Clamp(pitch, -10f, 80f);

            lastInputTime = Time.time;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            distance -= scroll * zoomSpeed;

            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            lastInputTime = Time.time;
        }
    }

    void HandleReset()
    {
        if (Time.time - lastInputTime > resetDelay)
        {
            distance = Mathf.Lerp(distance, defaultDistance, Time.deltaTime * smoothTime);
        }
    }
}