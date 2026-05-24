using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
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

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();

        mainCamera =
            Camera.main;
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");

        // RAW INPUT MAGNITUDE
        float movementAmount =
            Mathf.Abs(horizontal) +
            Mathf.Abs(vertical);

        movementAmount =
            Mathf.Clamp01(movementAmount);

        // UPDATE ANIMATOR
        if (animator != null)
        {
            animator.SetFloat(
                "Speed",
                movementAmount
            );
        }

        Vector3 inputDirection =
            new Vector3(
                horizontal,
                0f,
                vertical
            ).normalized;

        if (movementAmount > 0.1f)
        {
            Vector3 cameraForward =
                mainCamera.transform.forward;

            Vector3 cameraRight =
                mainCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection =
                (cameraForward * vertical) +
                (cameraRight * horizontal);

            moveDirection.Normalize();

            controller.Move(
                moveDirection *
                moveSpeed *
                Time.deltaTime
            );

            Quaternion targetRotation =
                Quaternion.LookRotation(
                    moveDirection
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed *
                    Time.deltaTime
                );
        }

        if (controller.isGrounded &&
            velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y +=
            gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }
}