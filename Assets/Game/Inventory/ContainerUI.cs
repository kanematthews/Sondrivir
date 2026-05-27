using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : MonoBehaviour
{
    [Header("Container")]
    public Container container;

    [Header("UI")]
    public Transform slotParent;

    public GameObject slotPrefab;

    // =====================================
    // GRID SETTINGS
    // =====================================

    [Header("Grid")]
    public int columns = 4;

    public float cellSize = 40f;

    public float spacing = 4f;

    public float padding = 16f;

    // =====================================
    // START
    // =====================================

    void Start()
    {
        Refresh();
    }

    // =====================================
    // REFRESH
    // =====================================

    public void Refresh()
    {
        // CLEAR OLD SLOTS

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // INVALID

        if (
            container == null ||
            container.slots == null)
        {
            return;
        }

        // CREATE SLOTS

        for (int i = 0; i < container.slots.Count; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotParent);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            if (slotUI == null)
            {
                continue;
            }

            // SLOT INFO

            slotUI.slotIndex = i;

            slotUI.parentContainer =
                container;

            // REFRESH SLOT

            slotUI.Refresh();
        }

        ResizeWindow();
    }

    // =====================================
    // RESIZE WINDOW
    // =====================================

    void ResizeWindow()
    {
        RectTransform rect =
            GetComponent<RectTransform>();

        if (
            rect == null ||
            container == null ||
            container.slots == null)
        {
            return;
        }

        // CALCULATE ROWS

        int rows =
            Mathf.CeilToInt(
                (float)container.slots.Count /
                columns);

        // CALCULATE HEIGHT

        float height =
            padding +
            (rows * cellSize) +
            ((rows - 1) * spacing) +
            padding;

        // APPLY

        rect.sizeDelta =
            new Vector2(
                rect.sizeDelta.x,
                height);
    }
}