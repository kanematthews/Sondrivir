using UnityEngine;

[CreateAssetMenu(
    fileName = "New Quest",
    menuName = "MMO/Quest")]
public class QuestData
    : ScriptableObject
{
    [Header("Quest")]
    public string questID;

    public string questName;

    [TextArea]
    public string description;

    // =====================================
    // OBJECTIVES
    // =====================================

    [Header("Objectives")]
    public QuestObjective[] objectives;

    // =====================================
    // REWARDS
    // =====================================

    [Header("Rewards")]
    public int goldReward;

    public int experienceReward;
}