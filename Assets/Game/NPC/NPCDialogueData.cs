using UnityEngine;

[CreateAssetMenu(
    fileName = "New NPC Dialogue",
    menuName = "MMO/NPC Dialogue")]
public class NPCDialogueData
    : ScriptableObject
{
    [Header("NPC")]
    public string npcName =
        "Villager";

    [TextArea]
    public string greeting =
        "Greetings traveler.";

    [Header("Dialogue Choices")]
    public DialogueChoice[] dialogueChoices;
}