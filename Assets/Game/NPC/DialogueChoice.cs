using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    [TextArea(2, 4)]
    public string playerText;

    public string nextNodeID;

    [Header("Actions")]
    public bool closesDialogue;

    public bool opensTrade;

    public bool startsQuest;

    public bool completesQuest;

    [Header("Quest")]
    public QuestData questToStart;

    public string questToCompleteID;

    [Header("Requirements")]
    public string requiredCompletedQuestID;

    public string requiredReadyToTurnInQuestID;
}