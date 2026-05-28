using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCDialogueUI : MonoBehaviour
{
    public static NPCDialogueUI instance;

    [Header("Panel")]
    public GameObject dialoguePanel;

    [Header("Text")]
    public TMP_Text npcNameText;

    public TMP_Text dialogueText;

    [Header("Input")]
    public TMP_InputField playerInputField;

    [Header("Distance")]
    public float maxDialogueDistance = 5f;

    [Header("Buttons")]
    public Button submitButton;

    public Button backButton;

    [Header("Dialogue Choices")]
    public Transform choiceContainer;

    public GameObject choiceButtonPrefab;

    // CURRENT NPC

    private NPCInteraction currentNPC;

    // CURRENT NODE

    private DialogueNode currentNode;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        instance = this;

        dialoguePanel.SetActive(false);
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        // NO ACTIVE DIALOGUE

        if (
            currentNPC == null ||
            !dialoguePanel.activeSelf)
        {
            return;
        }

        // =================================
        // SPACE CLOSES DIALOGUE
        // =================================

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CloseDialogue();

            return;
        }

        // =================================
        // FIND PLAYER
        // =================================

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player");

        if (player == null)
        {
            return;
        }

        // =================================
        // DISTANCE CHECK
        // =================================

        float distance =
            Vector3.Distance(
                player.transform.position,
                currentNPC.transform.position);

        // TOO FAR

        if (distance > maxDialogueDistance)
        {
            CloseDialogue();
        }
    }

    // =====================================
    // OPEN DIALOGUE
    // =====================================

    public void OpenDialogue(
        NPCInteraction npc)
    {
        // INVALID

        if (
            npc == null ||
            npc.dialogueData == null)
        {
            return;
        }

        currentNPC = npc;

        dialoguePanel.SetActive(true);

        npcNameText.text =
            npc.dialogueData.npcName;

        // LOAD START NODE

        LoadNode(
            npc.dialogueData.startingNodeID);

        playerInputField.text = "";
    }

    // =====================================
    // LOAD NODE
    // =====================================

    void LoadNode(
        string nodeID)
    {
        if (
            currentNPC == null ||
            currentNPC.dialogueData == null)
        {
            return;
        }

        foreach (
            DialogueNode node
            in currentNPC.dialogueData.nodes)
        {
            if (node.nodeID == nodeID)
            {
                currentNode = node;

                dialogueText.text =
                    node.npcText;

                GenerateChoices();

                return;
            }
        }

        Debug.LogWarning(
            "Node not found: " +
            nodeID);
    }

    // =====================================
    // CLOSE
    // =====================================

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);

        currentNPC = null;

        currentNode = null;

        ClearChoices();
    }

    // =====================================
    // GENERATE CHOICES
    // =====================================

    void GenerateChoices()
    {
        ClearChoices();

        if (
            currentNode == null ||
            currentNode.choices == null)
        {
            return;
        }

        foreach (
            DialogueChoice choice
            in currentNode.choices)
        {
            // =================================
            // QUEST REQUIREMENTS
            // =================================

            if (
                !string.IsNullOrWhiteSpace(
                    choice.requiredCompletedQuestID))
            {
                GameObject player =
                    GameObject.FindGameObjectWithTag(
                        "Player");

                if (player != null)
                {
                    QuestManager manager =
                        player.GetComponent
                        <QuestManager>();

                    if (manager != null)
                    {
                        bool completed =
                            manager.HasCompletedQuest(
                                choice
                                .requiredCompletedQuestID);

                        if (!completed)
                        {
                            continue;
                        }
                    }
                }
            }

            // =================================
            // CREATE BUTTON
            // =================================

            GameObject obj =
                Instantiate(
                    choiceButtonPrefab,
                    choiceContainer);

            DialogueChoiceButton button =
                obj.GetComponent
                <DialogueChoiceButton>();

            if (button != null)
            {
                button.Setup(choice);
            }
        }
    }

    // =====================================
    // CLEAR CHOICES
    // =====================================

    void ClearChoices()
    {
        for (
            int i =
            choiceContainer.childCount - 1;
            i >= 0;
            i--)
        {
            Destroy(
                choiceContainer
                    .GetChild(i)
                    .gameObject);
        }
    }

    // =====================================
    // SELECT CHOICE
    // =====================================

    public void SelectChoice(
        DialogueChoice choice)
    {
        if (choice == null)
        {
            return;
        }

        // =================================
        // CLOSE
        // =================================

        if (choice.closesDialogue)
        {
            CloseDialogue();

            return;
        }

        // =================================
        // TRADE
        // =================================

        if (choice.opensTrade)
        {
            Debug.Log(
                "OPEN TRADE WINDOW");
        }

        // =================================
        // QUEST
        // =================================

        if (
            choice.startsQuest &&
            choice.questToStart != null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag(
                    "Player");

            if (player != null)
            {
                QuestManager manager =
                    player.GetComponent
                    <QuestManager>();

                if (manager != null)
                {
                    manager.StartQuest(
                        choice.questToStart);
                }
            }
        }

        // =================================
        // NEXT NODE
        // =================================

        LoadNode(
            choice.nextNodeID);
    }

    // =====================================
    // PLAYER INPUT
    // =====================================

    public void SubmitPlayerInput()
    {
        if (currentNPC == null)
        {
            return;
        }

        string input =
            playerInputField.text
                .ToLower()
                .Trim();

        // EMPTY

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        // SECRET KEYWORDS

        if (input.Contains("cult"))
        {
            dialogueText.text =
                "Keep your voice down...";
        }
        else if (input.Contains("ruins"))
        {
            dialogueText.text =
                "The eastern ruins are cursed.";
        }
        else
        {
            dialogueText.text =
                "I don't know much about that.";
        }

        playerInputField.text = "";
    }
}