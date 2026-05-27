using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    [Header("Player")]
    [TextArea]
    public string playerText;

    [Header("NPC")]
    [TextArea]
    public string npcResponse;

    [Header("Settings")]
    public bool closesDialogue = false;

    public bool goBackAfterResponse = true;
}