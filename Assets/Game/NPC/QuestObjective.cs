using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    [Header("Objective")]
    public QuestObjectiveType
        objectiveType;

    [TextArea]
    public string description;

    // =====================================
    // TARGET
    // =====================================

    public string targetID;

    public int requiredAmount = 1;
}