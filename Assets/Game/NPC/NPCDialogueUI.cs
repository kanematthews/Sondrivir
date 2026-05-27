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
    // OPEN
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

        // NPC INFO

        npcNameText.text =
            npc.dialogueData.npcName;

        dialogueText.text =
            npc.dialogueData.greeting;

        // CLEAR INPUT

        playerInputField.text = "";

        // GENERATE CHOICES

        GenerateChoices();
    }

    // =====================================
    // CLOSE
    // =====================================

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);

        currentNPC = null;

        ClearChoices();
    }

    // =====================================
    // GENERATE CHOICES
    // =====================================

    void GenerateChoices()
    {
        ClearChoices();

        // INVALID

        if (
            currentNPC == null ||
            currentNPC.dialogueData == null ||
            currentNPC.dialogueData.dialogueChoices == null)
        {
            return;
        }

        // CREATE BUTTONS

        foreach (
            DialogueChoice choice
            in currentNPC
                .dialogueData
                .dialogueChoices)
        {
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

        // NPC RESPONSE

        dialogueText.text =
            choice.npcResponse;

        // CLOSE

        if (choice.closesDialogue)
        {
            CloseDialogue();
        }
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

        // BASIC KEYWORDS

        if (input.Contains("quest"))
        {
            dialogueText.text =
                "I may have work for you soon...";
        }
        else if (input.Contains("trade"))
        {
            dialogueText.text =
                "Take a look at my wares.";
        }
        else
        {
            dialogueText.text =
                "I don't know much about that.";
        }

        // CLEAR INPUT

        playerInputField.text = "";
    }
}