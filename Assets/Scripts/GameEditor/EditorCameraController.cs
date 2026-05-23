using UnityEngine;

public class EditorCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 20f;

    public float sprintMultiplier = 2f;

    public float panSpeed = 0.02f;

    [Header("Rotation")]
    public float rotationSpeed = 3f;

    [Header("Zoom")]
    public float zoomSpeed = 10f;

    public float minZoom = 5f;

    public float maxZoom = 100f;

    private Vector3 dragOrigin;

    void Update()
    {
        HandleMovement();

        HandleRotation();

        HandleZoom();

        HandleMiddleMousePan();
    }

    void HandleMovement()
    {
        float h =
            Input.GetAxisRaw("Horizontal");

        float v =
            Input.GetAxisRaw("Vertical");

        Vector3 forward =
            transform.forward;

        Vector3 right =
            transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir =
            forward * v +
            right * h;

        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= sprintMultiplier;
        }

        transform.position +=
            moveDir *
            speed *
            Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX =
                Input.GetAxis("Mouse X");

            transform.Rotate(
                Vector3.up,
                mouseX * rotationSpeed,
                Space.World
            );
        }
    }

    void HandleZoom()
    {
        float scroll =
            Input.GetAxis("Mouse ScrollWheel");

        Vector3 localPos =
            cameraTransform.localPosition;

        localPos +=
            cameraTransform.forward *
            scroll *
            zoomSpeed;

        float distance =
            localPos.magnitude;

        distance =
            Mathf.Clamp(
                distance,
                minZoom,
                maxZoom
            );

        localPos =
            localPos.normalized * distance;

        cameraTransform.localPosition =
            localPos;
    }

    void HandleMiddleMousePan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin =
                Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 difference =
                Input.mousePosition -
                dragOrigin;

            Vector3 move =
                (-transform.right * difference.x +
                 -transform.forward * difference.y);

            move.y = 0f;

            transform.position +=
                move *
                panSpeed;

            dragOrigin =
                Input.mousePosition;
        }
    }
}