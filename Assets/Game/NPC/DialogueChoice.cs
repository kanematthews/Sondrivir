using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    [Header("Player Text")]
    [TextArea]
    public string playerText;

    // =====================================
    // NEXT NODE
    // =====================================

    [Header("Next Node")]
    public string nextNodeID;

    // =====================================
    // SETTINGS
    // =====================================

    [Header("Settings")]
    public bool closesDialogue = false;

    // =====================================
    // TRADE
    // =====================================

    [Header("Trade")]
    public bool opensTrade = false;

    // =====================================
    // QUESTS
    // =====================================

    [Header("Quest")]
    public bool startsQuest = false;

    public QuestData questToStart;

    // =====================================
    // REQUIREMENTS
    // =====================================

    [Header("Requirements")]

    [Tooltip(
        "Only show this choice if the player completed this quest.")]
    public string requiredCompletedQuestID;
}