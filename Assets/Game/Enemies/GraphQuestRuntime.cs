using UnityEngine;

public class GraphQuestRuntime : MonoBehaviour
{
    public static GraphQuestRuntime Instance;

    private QuestManager questManager;

    private PlayerStats playerStats;

    private PlayerInventory playerInventory;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        questManager =
            FindFirstObjectByType<QuestManager>();

        playerStats =
            FindFirstObjectByType<PlayerStats>();

        playerInventory =
            FindFirstObjectByType<PlayerInventory>();
    }

    // =====================================
    // START QUEST
    // =====================================

    public void StartQuest(
        QuestData quest)
    {
        if (
            questManager == null ||
            quest == null)
        {
            return;
        }

        if (
            questManager.HasQuest(
                quest.questID))
        {
            return;
        }

        questManager.StartQuest(
            quest);
    }

    // =====================================
    // READY TO TURN IN
    // =====================================

    public bool CanTurnInQuest(
        string questID)
    {
        if (
            questManager == null)
        {
            return false;
        }

        return questManager
            .HasQuestReadyToTurnIn(
                questID);
    }

    // =====================================
    // TURN IN
    // =====================================

    public void TurnInQuest(
        QuestData quest)
    {
        if (
            quest == null ||
            questManager == null)
        {
            return;
        }

        if (
            !questManager
                .HasQuestReadyToTurnIn(
                    quest.questID))
        {
            return;
        }

        GiveRewards(
            quest);

        questManager
            .CompleteQuest(
                quest.questID);

        Debug.Log(
            "Quest Turned In: "
            + quest.questName);
    }

    // =====================================
    // REWARDS
    // =====================================

    private void GiveRewards(
        QuestData quest)
    {
        GiveExperience(
            quest.experienceReward);

        Debug.Log(
            "Gold Reward: "
            + quest.goldReward);
    }

    // =====================================
    // EXPERIENCE
    // =====================================

    private void GiveExperience(
        int amount)
    {
        if (
            playerStats == null)
        {
            return;
        }

        playerStats.experience +=
            amount;

        Debug.Log(
            "XP Gained: "
            + amount);

        while (
            playerStats.experience >=
            playerStats
                .experienceToNextLevel)
        {
            playerStats.experience -=
                playerStats
                    .experienceToNextLevel;

            playerStats.level++;

            playerStats.statPoints += 5;

            playerStats.experienceToNextLevel =
                Mathf.RoundToInt(
                    playerStats
                        .experienceToNextLevel
                    * 1.25f);

            Debug.Log(
                "LEVEL UP! "
                + playerStats.level);
        }
    }
}