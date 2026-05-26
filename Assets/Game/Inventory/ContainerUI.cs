using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : MonoBehaviour
{
    [Header("UI")]
    public Transform slotContainer;

    public GameObject slotPrefab;

    [HideInInspector]
    public Container container;

    // GRID SETTINGS
    private int columns = 4;

    private float cellSize = 40f;

    private float spacing = 4f;

    private float padding = 16f;

    public void Refresh()
    {
        // CLEAR OLD
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (container == null)
        {
            return;
        }

        // CREATE SLOTS
        for (int i = 0; i < container.slotCount; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotContainer);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            ItemStack stack =
                container.slots[i];

            // SLOT INFO
            slotUI.slotIndex = i;

            slotUI.parentContainer =
                container;

            // EMPTY SLOT
            if (stack == null)
            {
                slotUI.icon.enabled = false;

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
                hover.stack = stack;
            }
        }

        ResizeWindow();
    }

    // =========================================
    // RESIZE WINDOW
    // =========================================

    void ResizeWindow()
    {
        RectTransform rect =
            GetComponent<RectTransform>();

        if (rect == null)
        {
            return;
        }

        // CALCULATE ROWS
        int rows =
            Mathf.CeilToInt(
                (float)container.slotCount /
                columns);

        // CALCULATE HEIGHT
        float height =
            padding +
            (rows * cellSize) +
            ((rows - 1) * spacing);

        // APPLY HEIGHT
        rect.sizeDelta =
            new Vector2(
                rect.sizeDelta.x,
                height);
    }
}