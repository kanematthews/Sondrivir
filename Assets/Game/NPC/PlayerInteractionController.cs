using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Interaction")]
    public float maxInteractionDistance = 5f;

    [Header("References")]
    public Camera playerCamera;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray =
            playerCamera.ScreenPointToRay(
                Input.mousePosition);

        if (
            !Physics.Raycast(
                ray,
                out RaycastHit hit,
                100f))
        {
            return;
        }

        NPCInteraction npc =
            hit.collider.GetComponentInParent<NPCInteraction>();

        if (npc == null)
        {
            return;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                npc.transform.position);

        if (
            distance >
            npc.interactionDistance)
        {
            Debug.Log(
                "Too far away.");
            return;
        }

        npc.Interact();
    }
}