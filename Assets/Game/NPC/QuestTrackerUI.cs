using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =========================================================
// QUEST TRACKER UI
// =========================================================
// Auto-generated via MMO > Setup Quest Tracker.
// Subscribes to QuestManager.onQuestUpdated and rebuilds
// the tracker panel whenever quest state changes.
// No prefab required — rows are spawned directly.
// =========================================================

public class QuestTrackerUI : MonoBehaviour
{
    // =====================================================
    // INSPECTOR
    // =====================================================

    [Header("References")]
    public Transform objectiveContainer;

    // =====================================================
    // STYLE CONSTANTS
    // =====================================================

    private static readonly Color ColorComplete =
        new Color(0.4f, 1f, 0.4f, 1f);

    private static readonly Color ColorActive =
        new Color(0.92f, 0.87f, 0.7f, 1f);

    private static readonly Color ColorDim =
        new Color(0.7f, 0.7f, 0.7f, 1f);

    private static readonly Color ColorHeader =
        new Color(1f, 0.82f, 0.3f, 1f);

    // =====================================================
    // PRIVATE
    // =====================================================

    private QuestManager questManager;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        questManager =
            FindFirstObjectByType<QuestManager>();

        if (questManager != null)
        {
            questManager.onQuestUpdated += Refresh;
        }

        Refresh();
    }

    // =====================================================
    // ON DESTROY
    // =====================================================

    private void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.onQuestUpdated -= Refresh;
        }
    }

    // =====================================================
    // REFRESH
    // =====================================================

    public void Refresh()
    {
        ClearRows();

        if (
            questManager == null ||
            objectiveContainer == null)
        {
            return;
        }

        bool hasAny = false;

        foreach (
            QuestInstance quest
            in questManager.activeQuests)
        {
            if (
                quest.state ==
                QuestState.Completed)
            {
                continue;
            }

            hasAny = true;

            bool allDone =
                quest.state ==
                QuestState.ReadyToTurnIn;

            // QUEST TITLE

            string titleText =
                allDone
                ? "✓ " + quest.questData.questName
                : quest.questData.questName;

            SpawnRow(
                titleText,
                allDone
                    ? ColorComplete
                    : ColorHeader,
                14,
                FontStyles.Bold,
                4f);

            // OBJECTIVES

            for (
                int i = 0;
                i <
                quest.questData.objectives.Length;
                i++)
            {
                QuestObjective obj =
                    quest.questData.objectives[i];

                int current =
                    i < quest.progress.Count
                    ? quest.progress[i]
                    : 0;

                bool done =
                    current >= obj.requiredAmount;

                string line =
                    "  " +
                    obj.description +
                    "  " +
                    current +
                    " / " +
                    obj.requiredAmount;

                SpawnRow(
                    line,
                    done ? ColorComplete : ColorActive,
                    12,
                    done
                        ? FontStyles.Strikethrough
                        : FontStyles.Normal,
                    1f);
            }

            // DIVIDER

            SpawnDivider();
        }

        // HIDE PANEL IF NO ACTIVE QUESTS

        gameObject.SetActive(hasAny);
    }

    // =====================================================
    // SPAWN ROW
    // =====================================================

    private void SpawnRow(
        string text,
        Color color,
        float fontSize,
        FontStyles style,
        float topPadding)
    {
        GameObject row =
            new GameObject(
                "Row",
                typeof(RectTransform));

        row.transform.SetParent(
            objectiveContainer,
            false);

        // LAYOUT ELEMENT

        LayoutElement layout =
            row.AddComponent<LayoutElement>();

        layout.minHeight = fontSize + 6f;

        layout.preferredHeight = fontSize + 8f;

        // TMP TEXT

        GameObject textObj =
            new GameObject(
                "Text",
                typeof(RectTransform));

        textObj.transform.SetParent(
            row.transform,
            false);

        RectTransform rt =
            textObj.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;

        rt.anchorMax = Vector2.one;

        rt.offsetMin =
            new Vector2(topPadding, 0);

        rt.offsetMax = Vector2.zero;

        TMP_Text label =
            textObj.AddComponent<TextMeshProUGUI>();

        label.text = text;

        label.color = color;

        label.fontSize = fontSize;

        label.fontStyle = style;

        label.enableWordWrapping = false;

        label.overflowMode =
            TextOverflowModes.Ellipsis;
    }

    // =====================================================
    // SPAWN DIVIDER
    // =====================================================

    private void SpawnDivider()
    {
        GameObject div =
            new GameObject(
                "Divider",
                typeof(RectTransform));

        div.transform.SetParent(
            objectiveContainer,
            false);

        LayoutElement layout =
            div.AddComponent<LayoutElement>();

        layout.minHeight = 1f;

        layout.preferredHeight = 1f;

        Image line =
            div.AddComponent<Image>();

        line.color =
            new Color(1f, 1f, 1f, 0.08f);
    }

    // =====================================================
    // CLEAR ROWS
    // =====================================================

    private void ClearRows()
    {
        foreach (
            Transform child
            in objectiveContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
