using System.Collections.Generic;
using UnityEngine;

public class QuestManager
    : MonoBehaviour
{
    [Header("Quests")]
    public List<QuestInstance>
        activeQuests =
            new List<QuestInstance>();

    // =====================================
    // GOLD COIN ITEM
    // =====================================

    [Header("Economy")]
    [Tooltip("Drag the GoldCoin ItemData asset here. " +
             "Run MMO > Create Gold Coin Item to generate it.")]
    public ItemData goldCoinItem;

    // =====================================
    // EVENTS
    // =====================================

    public System.Action onQuestUpdated;

    // =====================================
    // START QUEST
    // =====================================

    public void StartQuest(
        QuestData quest)
    {
        if (quest == null)
        {
            return;
        }

        // ALREADY EXISTS

        foreach (
            QuestInstance q
            in activeQuests)
        {
            if (
                q.questData ==
                quest)
            {
                return;
            }
        }

        QuestInstance instance =
            new QuestInstance();

        instance.questData =
            quest;

        instance.state =
            QuestState.Active;

        // INITIALIZE OBJECTIVE PROGRESS

        foreach (
            QuestObjective objective
            in quest.objectives)
        {
            instance.progress.Add(0);
        }

        activeQuests.Add(instance);

        Debug.Log(
            "Quest Started: " +
            quest.questName);

        onQuestUpdated?.Invoke();
    }

    // =====================================
    // COMPLETE QUEST
    // =====================================

    public void CompleteQuest(
        string questID)
    {
        foreach (
            QuestInstance q
            in activeQuests)
        {
            if (
                q.questData.questID ==
                questID)
            {
                q.state =
                    QuestState.Completed;

                Debug.Log(
                    "Quest Completed: " +
                    q.questData.questName);

                return;
            }
        }
    }

    // =====================================
    // HAS QUEST
    // =====================================

    public bool HasQuest(
        string questID)
    {
        foreach (
            QuestInstance q
            in activeQuests)
        {
            if (
                q.questData.questID ==
                questID)
            {
                return true;
            }
        }

        return false;
    }

    // =====================================
    // HAS COMPLETED QUEST
    // =====================================

    public bool HasCompletedQuest(
        string questID)
    {
        foreach (
            QuestInstance q
            in activeQuests)
        {
            if (
                q.questData.questID ==
                questID &&
                q.state ==
                QuestState.Completed)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasQuestReadyToTurnIn(
    string questID)
    {
        foreach (
            QuestInstance q
            in activeQuests)
        {
            if (
                q.questData.questID ==
                questID &&
                q.state ==
                QuestState.ReadyToTurnIn)
            {
                return true;
            }
        }

        return false;
    }

    // =====================================
    // AWARD GOLD
    // =====================================

    // Returns true if gold was awarded, false if inventory was full.
    // 'message' contains a human-readable result either way.

    public bool TryAwardGold(
        int amount,
        out string message)
    {
        message = "";

        if (amount <= 0)
        {
            return true;
        }

        if (goldCoinItem == null)
        {
            // No coin item assigned — fall back to PlayerStats
            PlayerStats stats =
                FindFirstObjectByType<PlayerStats>();

            if (stats != null)
            {
                stats.AddGold(amount);
            }

            message =
                "+" + amount + "g";

            return true;
        }

        // Try to add to inventory
        PlayerInventory inventory =
            FindFirstObjectByType<PlayerInventory>();

        if (inventory == null)
        {
            message =
                "No player inventory found.";

            return false;
        }

        bool added =
            inventory.AddItem(
                goldCoinItem,
                amount);

        if (added)
        {
            message =
                "+" + amount + "g added to inventory.";

            return true;
        }

        // Inventory full — count occupied slots

        int occupied = 0;

        foreach (ItemStack s in inventory.slots)
        {
            if (s != null)
            {
                occupied++;
            }
        }

        message =
            "Your inventory is full! " +
            "Clear some space and collect your reward.";

        return false;
    }

    // =====================================
    // REGISTER KILL
    // =====================================

    public void RegisterKill(
        string enemyID)
    {
        foreach (
            QuestInstance quest
            in activeQuests)
        {
            // ONLY ACTIVE QUESTS

            if (
                quest.state !=
                QuestState.Active)
            {
                continue;
            }

            // OBJECTIVES

            for (
                int i = 0;
                i <
                quest.questData
                    .objectives.Length;
                i++)
            {
                QuestObjective objective =
                    quest.questData
                        .objectives[i];

                // ONLY KILL OBJECTIVES

                if (
                    objective.objectiveType !=
                    QuestObjectiveType.Kill)
                {
                    continue;
                }

                // WRONG TARGET

                if (
                    objective.targetID !=
                    enemyID)
                {
                    continue;
                }

                // ADD PROGRESS

                quest.progress[i]++;

                Debug.Log(
                    objective.description +
                    " (" +
                    quest.progress[i] +
                    "/" +
                    objective.requiredAmount +
                    ")");

                onQuestUpdated?.Invoke();

                // CHECK COMPLETE

                bool complete =
                    true;

                for (
                    int j = 0;
                    j <
                    quest.questData
                        .objectives.Length;
                    j++)
                {
                    QuestObjective obj =
                        quest.questData
                            .objectives[j];

                    if (
                        quest.progress[j] <
                        obj.requiredAmount)
                    {
                        complete = false;

                        break;
                    }
                }

                // COMPLETE QUEST

                if (complete)
                {
                    quest.state =
                        QuestState.ReadyToTurnIn;

                    Debug.Log(
                        "Quest Ready To Turn In: " +
                        quest.questData.questName);

                    onQuestUpdated?.Invoke();
                }

                return;
            }
        }
    }
}