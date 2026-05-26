using UnityEngine;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI instance;

    [Header("UI")]
    public GameObject window;

    public TMP_Text tooltipText;

    void Awake()
    {
        instance = this;

        Hide();
    }

    void Update()
    {
        transform.position =
            Input.mousePosition;
    }

    public void Show(ItemStack stack)
    {
        if (
            stack == null ||
            stack.item == null)
        {
            return;
        }

        window.SetActive(true);

        tooltipText.text =
            GetTooltipText(stack);
    }

    public void Hide()
    {
        window.SetActive(false);
    }

    string GetTooltipText(ItemStack stack)
    {
        if (
            stack == null ||
            stack.item == null)
        {
            return "";
        }

        ItemData item =
            stack.item;

        string text = "";

        // NAME
        text += item.itemName;

        if (stack.amount > 1)
        {
            text +=
                " x" +
                stack.amount;
        }

        // DESCRIPTION
        if (!string.IsNullOrEmpty(item.description))
        {
            text +=
                "\n\n" +
                item.description;
        }

        // DAMAGE
        if (item.damage > 0)
        {
            text +=
                "\n\nDamage: " +
                item.damage;
        }

        // ATTACK SPEED
        if (item.attackSpeed > 0)
        {
            text +=
                "\n\nAttack Speed: " +
                item.attackSpeed;
        }

        // REQUIREMENTS
        bool hasRequirements =
            item.requiredStrength > 0 ||
            item.requiredDexterity > 0 ||
            item.requiredIntelligence > 0;

        if (hasRequirements)
        {
            text +=
                "\n\nRequirements:";

            if (item.requiredStrength > 0)
            {
                text +=
                    "\n" +
                    item.requiredStrength +
                    " STR";
            }

            if (item.requiredDexterity > 0)
            {
                text +=
                    "\n" +
                    item.requiredDexterity +
                    " DEX";
            }

            if (item.requiredIntelligence > 0)
            {
                text +=
                    "\n" +
                    item.requiredIntelligence +
                    " INT";
            }
        }

        // VALUE
        text +=
            "\n\nValue: " +
            item.goldValue +
            " Gold";

        // WEIGHT
        text +=
            "\n\nWeight: " +
            item.weight;

        return text;
    }
}