using UnityEngine;

public class LootUI : MonoBehaviour
{
    public static LootUI instance;

    [Header("UI")]
    public GameObject window;

    public Transform slotContainer;

    public GameObject slotPrefab;

    private LootBag currentBag;

    void Awake()
    {
        instance = this;

        Close();
    }

    public void Open(
        LootBag bag)
    {
        currentBag = bag;

        window.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        currentBag = null;

        window.SetActive(false);
    }

    public void Refresh()
    {
        // CLEAR OLD SLOTS
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (currentBag == null)
        {
            return;
        }

        // CREATE SLOTS
        for (int i = 0; i < currentBag.slots.Count; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotContainer);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            ItemStack stack =
                currentBag.slots[i];

            // SLOT INFO
            slotUI.slotIndex = i;

            slotUI.parentContainer =
                currentBag;

            // EMPTY SLOT
            if (stack == null)
            {
                if (
                    slotUI.icon != null)
                {
                    slotUI.icon.enabled = false;
                }

                continue;
            }

            // ICON
            slotUI.icon.enabled = true;

            slotUI.icon.sprite =
                stack.item.icon;

            slotUI.stack =
                stack;

            // STACK TEXT
            if (slotUI.stackText != null)
            {
                if (stack.amount > 1)
                {
                    slotUI.stackText.text =
                        stack.amount.ToString();
                }
                else
                {
                    slotUI.stackText.text = "";
                }
            }

            // TOOLTIP
            LootSlotHover hover =
                slot.GetComponent<LootSlotHover>();

            if (hover != null)
            {
                hover.stack =
                    stack;
            }
        }
    }
}