using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public NPCDialogueData dialogueData;

    // =====================================
    // INTERACT
    // =====================================

    public void Interact()
    {
        // NO DATA

        if (dialogueData == null)
        {
            Debug.LogWarning(
                "No dialogue data assigned.");

            return;
        }

        NPCDialogueUI.instance
            .OpenDialogue(this);
    }
}