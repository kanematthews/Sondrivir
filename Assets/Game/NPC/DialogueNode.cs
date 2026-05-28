using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [Header("Node")]
    public string nodeID;

    [TextArea]
    public string npcText;

    [Header("Choices")]
    public DialogueChoice[] choices;
}