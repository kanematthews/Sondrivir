using UnityEngine;

public class MMOCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance")]
    public float distance = 80f;

    public float minDistance = 20f;

    public float maxDistance = 100f;

    [Header("Rotation")]
    public float yaw = 45f;

    public float pitch = 58f;

    public float rotationSpeed = 3f;

    [Header("Zoom")]
    public float zoomSpeed = 4f;

    [Header("Smoothing")]
    public float smoothSpeed = 10f;

    [Header("Look Offset")]
    public Vector3 lookOffset =
        new Vector3(0, 1.5f, 0);

    [Header("Camera")]
    public Camera cam;

    public float fieldOfView = 50f;

    private Vector3 velocity;

    void Start()
    {
        if (cam == null)
        {
            cam =
                GetComponent<Camera>();
        }

        if (cam != null)
        {
            cam.fieldOfView =
                fieldOfView;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        HandleRotation();

        HandleZoom();

        // MMO CAMERA LIMITS
        pitch =
            Mathf.Clamp(
                pitch,
                60f,
                72f);

        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0);

        // CAMERA POSITION
        Vector3 direction =
            rotation *
            new Vector3(
                0,
                0,
                -distance);

        Vector3 desiredPosition =
            target.position +
            direction;

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                1f / smoothSpeed);

        // LOOK SLIGHTLY ABOVE PLAYER
        transform.LookAt(
            target.position +
            lookOffset);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            yaw +=
                Input.GetAxis("Mouse X") *
                rotationSpeed;
        }
    }

    void HandleZoom()
    {
        float scroll =
            Input.GetAxis(
                "Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -=
                scroll *
                zoomSpeed;

            distance =
                Mathf.Clamp(
                    distance,
                    minDistance,
                    maxDistance);
        }
    }
}