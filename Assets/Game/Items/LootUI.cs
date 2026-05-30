using UnityEngine;

public class LootUI : MonoBehaviour
{
    public static LootUI instance;

    [Header("UI")]
    public GameObject window;

    public Transform slotContainer;

    public GameObject slotPrefab;

    private Container currentContainer;

    // =====================================
    // AWAKE
    // =====================================

    void Awake()
    {
        instance = this;

        Close();
    }

    // =====================================
    // OPEN
    // =====================================

    public void Open(Container container)
    {
        currentContainer = container;

        window.SetActive(true);

        Refresh();
    }

    // =====================================
    // CLOSE
    // =====================================

    public void Close()
    {
        currentContainer = null;

        window.SetActive(false);
    }

    // =====================================
    // REFRESH
    // =====================================

    public void Refresh()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        if (currentContainer == null)
        {
            return;
        }

        for (int i = 0; i < currentContainer.slots.Count; i++)
        {
            GameObject slot =
                Instantiate(
                    slotPrefab,
                    slotContainer);

            LootSlotUI slotUI =
                slot.GetComponent<LootSlotUI>();

            if (slotUI == null)
            {
                continue;
            }

            slotUI.slotIndex = i;

            slotUI.parentContainer =
                currentContainer;

            slotUI.Refresh();
        }
    }
}