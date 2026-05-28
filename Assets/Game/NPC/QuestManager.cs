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
                    CompleteQuest(
                        quest.questData.questID);
                }

                return;
            }
        }
    }
}