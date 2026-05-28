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

    [Header("Start")]
    public string startingNodeID =
        "ROOT";

    [Header("Dialogue Nodes")]
    public DialogueNode[] nodes;
}