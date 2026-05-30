using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public NPCDialogueGraphData dialogue;

    [Header("Interaction")]
    public float interactionDistance = 4f;

    public void Interact()
    {
        if (NPCDialogueUI.instance == null)
        {
            return;
        }

        NPCDialogueUI.instance
            .OpenDialogue(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            interactionDistance);
    }
}