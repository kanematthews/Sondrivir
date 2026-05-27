using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 5f;

    void Update()
    {
        HandleRightClick();
    }

    // =====================================
    // RIGHT CLICK
    // =====================================

    void HandleRightClick()
    {
        if (!Input.GetMouseButtonDown(1))
        {
            return;
        }

        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition);

        RaycastHit hit;

        if (
            Physics.Raycast(
                ray,
                out hit,
                100f))
        {
            // =================================
            // NPC INTERACTION
            // =================================

            NPCInteraction npc =
                hit.collider.GetComponentInParent
                <NPCInteraction>();

            if (npc != null)
            {
                float distance =
                    Vector3.Distance(
                        transform.position,
                        npc.transform.position);

                // TOO FAR

                if (
                    distance >
                    interactDistance)
                {
                    Debug.Log(
                        "Too far away.");

                    return;
                }

                npc.Interact();

                return;
            }

            // =================================
            // FUTURE INTERACTIONS
            // =================================

            // Loot
            // Players
            // Enemies
            // Doors
            // Chests
            // Gathering
            // Etc
        }
    }
}