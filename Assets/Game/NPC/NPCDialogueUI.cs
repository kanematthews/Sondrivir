using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NPCDialogueUI : MonoBehaviour
{
    public static NPCDialogueUI instance;

    [Header("Panel")]
    public GameObject dialoguePanel;

    [Header("Text")]
    public TMP_Text npcNameText;

    public TMP_Text dialogueText;

    [Header("Choices")]
    public Transform choiceContainer;

    public GameObject choiceButtonPrefab;

    [Header("Distance")]
    public float maxDialogueDistance = 5f;

    // =====================================================
    // CURRENT NPC
    // =====================================================

    private NPCInteraction currentNPC;

    // =====================================================
    // CURRENT GRAPH
    // =====================================================

    private NPCDialogueGraphData currentGraph;

    // =====================================================
    // CURRENT NODE
    // =====================================================

    private GraphNodeData currentNode;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        instance = this;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (
            currentNPC == null ||
            !dialoguePanel.activeSelf)
        {
            return;
        }

        // CLOSE WITH SPACE

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseDialogue();

            return;
        }

        // PLAYER DISTANCE

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player");

        if (player == null)
        {
            return;
        }

        float distance =
            Vector3.Distance(
                player.transform.position,
                currentNPC.transform.position);

        if (distance > maxDialogueDistance)
        {
            CloseDialogue();
        }
    }

    // =====================================================
    // OPEN DIALOGUE
    // =====================================================

    public void OpenDialogue(
        NPCInteraction npc)
    {
        if (
            npc == null ||
            npc.dialogue == null)
        {
            return;
        }

        currentNPC = npc;

        currentGraph =
            npc.dialogue;

        dialoguePanel.SetActive(true);

        npcNameText.text =
            currentGraph.npcName;

        if (currentGraph.nodes.Count <= 0)
        {
            return;
        }

        // LOAD FIRST NODE

        LoadNode(
            currentGraph.nodes[0]);
    }

    // =====================================================
    // LOAD NODE
    // =====================================================

    public void LoadNode(GraphNodeData node)
    {
        currentNode = node;

        dialogueText.text =
            node.npcText;

        GenerateChoices();
    }

    // =====================================================
    // GENERATE CHOICES
    // =====================================================

    private void GenerateChoices()
    {
        ClearChoices();

        if (
            currentGraph == null ||
            currentNode == null)
        {
            return;
        }

        List<GraphEdgeData> connections =
            currentGraph.edges
            .FindAll(
                x =>
                x.outputGUID ==
                currentNode.guid);

        foreach (GraphEdgeData edge in connections)
        {
            GameObject buttonObj =
                Instantiate(
                    choiceButtonPrefab,
                    choiceContainer);

            TMP_Text text =
                buttonObj.GetComponentInChildren
                <TMP_Text>();

            if (text != null)
            {
                text.text =
                    string.IsNullOrWhiteSpace(
                        edge.choiceText)
                    ? "Continue"
                    : edge.choiceText;
            }

            StyleChoiceButton(buttonObj);

            Button button =
                buttonObj.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(
                    () =>
                    {
                        SelectChoice(edge);
                    });
            }
        }

        // NO CONNECTIONS

        if (connections.Count <= 0)
        {
            GameObject buttonObj =
                Instantiate(
                    choiceButtonPrefab,
                    choiceContainer);

            TMP_Text text =
                buttonObj.GetComponentInChildren
                <TMP_Text>();

            if (text != null)
            {
                text.text =
                    "[End Conversation]";
            }

            StyleChoiceButton(buttonObj);

            Button button =
                buttonObj.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(
                    () =>
                    {
                        CloseDialogue();
                    });
            }
        }
    }

    // =====================================================
    // SHOW BLOCK MESSAGE
    // =====================================================

    private void ShowBlockMessage(
        string message,
        string buttonLabel)
    {
        dialogueText.text = message;

        ClearChoices();

        GameObject btn =
            Instantiate(
                choiceButtonPrefab,
                choiceContainer);

        TMP_Text label =
            btn.GetComponentInChildren<TMP_Text>(true);

        if (label != null)
        {
            label.text = buttonLabel;
        }

        StyleChoiceButton(btn);

        Button button =
            btn.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(CloseDialogue);
        }
    }

    // =====================================================
    // STYLE CHOICE BUTTON
    // =====================================================

    private void StyleChoiceButton(
        GameObject btn)
    {
        if (btn == null)
        {
            return;
        }

        Image img =
            btn.GetComponent<Image>();

        if (img != null)
        {
            img.color =
                new Color(
                    0.12f, 0.11f, 0.09f, 1f);
        }

        Button button =
            btn.GetComponent<Button>();

        if (button != null)
        {
            ColorBlock cb = button.colors;

            cb.normalColor =
                new Color(
                    0.12f, 0.11f, 0.09f, 1f);

            cb.highlightedColor =
                new Color(
                    0.22f, 0.19f, 0.10f, 1f);

            cb.pressedColor =
                new Color(
                    0.30f, 0.26f, 0.12f, 1f);

            button.colors = cb;
        }

        TMP_Text label =
            btn.GetComponentInChildren<TMP_Text>(true);

        if (label != null)
        {
            label.color =
                new Color(
                    0.95f, 0.88f, 0.65f, 1f);

            label.fontSize = 13f;
        }
    }

    // =====================================================
    // SELECT CHOICE
    // =====================================================

    public void SelectChoice(GraphEdgeData edge)
    {
        GraphNodeData nextNode =
                currentGraph.nodes
                .Find(
                    x =>
                    x.guid ==
                    edge.inputGUID);

        if (nextNode == null)
        {
            return;
        }

        ExecuteNode(nextNode);
    }

    // =====================================================
    // EXECUTE NODE
    // =====================================================

    private void ExecuteNode(GraphNodeData node)
{
currentNode = node;


QuestManager questManager =
    FindFirstObjectByType<QuestManager>();

switch (node.nodeType)
{
    // =================================
    // DIALOGUE
    // =================================

    case NodeType.Dialogue:

        LoadNode(node);
        break;

    // =================================
    // QUEST OFFER
    // =================================

    case NodeType.QuestOffer:

        if (
            node.questData != null &&
            questManager != null)
        {
            bool alreadyHasQuest =
                questManager.activeQuests
                .Exists(
                    q =>
                    q.questData ==
                    node.questData);

            if (!alreadyHasQuest)
            {
                questManager.StartQuest(
                    node.questData);
            }
        }

        // Show quest info in dialogue text
        if (node.questData != null)
        {
            dialogueText.text =
                node.npcText +
                "\n\n<b>" +
                node.questData.questName +
                "</b>\n" +
                node.questData.description;
        }
        else
        {
            dialogueText.text =
                node.npcText;
        }

        GenerateChoices();
        break;

    // =================================
    // QUEST CHECK
    // =================================

    case NodeType.QuestCheck:

        bool readyToTurnIn =
            false;

        if (
            node.questData != null &&
            questManager != null)
        {
            QuestInstance quest =
                questManager.activeQuests
                .Find(
                    q =>
                    q.questData ==
                    node.questData);

            if (
                quest != null &&
                quest.state ==
                QuestState.ReadyToTurnIn)
            {
                readyToTurnIn =
                    true;
            }
        }

        if (!readyToTurnIn)
        {
            dialogueText.text =
                node.npcText +
                "\n\n(Quest not complete yet)";

            ClearChoices();

            GameObject buttonObj =
                Instantiate(
                    choiceButtonPrefab,
                    choiceContainer);

            TMP_Text text =
                buttonObj
                .GetComponentInChildren
                <TMP_Text>();

            if (text != null)
            {
                text.text =
                    "I'll come back later.";
            }

            Button button =
                buttonObj
                .GetComponent<Button>();

            if (button != null)
            {
                button.onClick
                    .AddListener(
                        CloseDialogue);
            }

            return;
        }

        LoadNode(node);

        break;

    // =================================
    // QUEST TURN IN
    // =================================

    case NodeType.QuestTurnIn:

        if (
            node.questData != null &&
            questManager != null)
        {
            // ── QUEST NOT READY ───────────────

            bool readyForTurnIn =
                questManager
                    .HasQuestReadyToTurnIn(
                        node.questData.questID);

            if (!readyForTurnIn)
            {
                ShowBlockMessage(
                    node.npcText +
                    "\n\n<color=#AAAAAA><i>You haven't finished the quest yet — come back when you have.</i></color>",
                    "I'll come back when I'm done.");

                return;
            }

            // ── AWARD GOLD (inventory item) ───

            string goldMessage = "";
            bool goldOk = true;

            if (node.questData.goldReward > 0)
            {
                goldOk = questManager.TryAwardGold(
                    node.questData.goldReward,
                    out goldMessage);
            }

            if (!goldOk)
            {
                // Inventory full for gold coins
                ShowBlockMessage(
                    "<color=#E8A020>You can't carry your reward!</color>\n\n" +
                    goldMessage +
                    "\n\nClear some slots and speak to me again.",
                    "I'll make room first.");

                return;
            }

            // ── AWARD XP (goes to PlayerStats) ──

            questManager.CompleteQuest(
                node.questData.questID);

            PlayerStats stats =
                FindFirstObjectByType<PlayerStats>();

            if (stats != null &&
                node.questData.experienceReward > 0)
            {
                stats.GainExperience(
                    node.questData.experienceReward);
            }

            // ── SUCCESS FLAVOUR ───────────────

            string rewardLine = "";

            if (node.questData.goldReward > 0)
            {
                rewardLine +=
                    "<color=#FFD700>+" +
                    node.questData.goldReward +
                    "g</color>";
            }

            if (node.questData.experienceReward > 0)
            {
                if (rewardLine.Length > 0)
                {
                    rewardLine += "  ";
                }

                rewardLine +=
                    "<color=#88CCFF>+" +
                    node.questData.experienceReward +
                    " XP</color>";
            }

            dialogueText.text =
                node.npcText +
                (rewardLine.Length > 0
                    ? "\n\n" + rewardLine
                    : "");
        }

        GenerateChoices();

        break;

    // =================================
    // TRADE
    // =================================

    case NodeType.Trade:

        LoadNode(node);
        break;

    // =================================
    // CONDITION
    // =================================

    case NodeType.Condition:

        LoadNode(node);
        break;

    // =================================
    // END
    // =================================

    case NodeType.End:

        CloseDialogue();
        break;
}

}


    // =====================================================
    // CLEAR CHOICES
    // =====================================================

    private void ClearChoices()
    {
        foreach (
            Transform child
            in choiceContainer)
        {
            Destroy(
                child.gameObject);
        }
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);

        currentNPC = null;

        currentGraph = null;

        currentNode = null;

        ClearChoices();
    }
}