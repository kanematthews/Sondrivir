using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController controller;

    private Camera mainCamera;

    private Vector3 velocity;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        controller =
            GetComponent<CharacterController>();
    }

    // =====================================
    // ON NETWORK SPAWN
    // =====================================

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // Disable input on non-owned players
            enabled = false;
            return;
        }

        // Assign camera to local player
        mainCamera = Camera.main;

        MMOCameraController cam =
            mainCamera
                ?.GetComponent<MMOCameraController>();

        if (cam != null)
        {
            cam.target = transform;
        }
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        MovePlayer();
    }

    // =====================================
    // MOVE
    // =====================================

    void MovePlayer()
    {
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");

        float movementAmount =
            Mathf.Clamp01(
                Mathf.Abs(horizontal) +
                Mathf.Abs(vertical));

        if (animator != null)
        {
            animator.SetFloat(
                "Speed",
                movementAmount);
        }

        if (movementAmount > 0.1f)
        {
            Vector3 cameraForward =
                mainCamera.transform.forward;

            Vector3 cameraRight =
                mainCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y  = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection =
                (cameraForward * vertical) +
                (cameraRight  * horizontal);

            moveDirection.Normalize();

            // Use stat speed if available
            PlayerStats stats =
                GetComponent<PlayerStats>();

            float speed =
                stats != null
                    ? stats.moveSpeed
                    : moveSpeed;

            controller.Move(
                moveDirection *
                speed *
                Time.deltaTime);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(
                        moveDirection),
                    rotationSpeed *
                    Time.deltaTime);
        }

        if (
            controller.isGrounded &&
            velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y +=
            gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime);
    }
}
