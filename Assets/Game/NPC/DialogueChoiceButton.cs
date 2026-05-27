using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueChoiceButton
    : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text buttonText;

    private DialogueChoice choice;

    private Button button;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        button =
            GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(
                SelectChoice);
        }
    }

    // =====================================
    // SETUP
    // =====================================

    public void Setup(
        DialogueChoice newChoice)
    {
        choice = newChoice;

        if (buttonText != null)
        {
            buttonText.text =
                choice.playerText;
        }
    }

    // =====================================
    // SELECT
    // =====================================

    public void SelectChoice()
    {
        if (choice == null)
        {
            return;
        }

        NPCDialogueUI.instance
            .SelectChoice(choice);
    }
}