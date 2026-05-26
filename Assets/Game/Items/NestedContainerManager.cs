using System.Collections.Generic;
using UnityEngine;

public class NestedContainerManager : MonoBehaviour
{
    public static NestedContainerManager instance;

    [Header("UI")]
    public Transform containerParent;

    public GameObject containerWindowPrefab;

    // TRACK OPEN WINDOWS
    private Dictionary<
        ItemContainerInstance,
        ContainerUI> openContainers =
            new Dictionary<
                ItemContainerInstance,
                ContainerUI>();

    void Awake()
    {
        instance = this;
    }

    // =========================================
    // TOGGLE CONTAINER
    // =========================================

    public void ToggleContainer(
        ItemContainerInstance container)
    {
        if (container == null)
        {
            return;
        }

        // ALREADY OPEN
        if (openContainers.ContainsKey(container))
        {
            ContainerUI ui =
                openContainers[container];

            if (ui != null)
            {
                Destroy(ui.gameObject);
            }

            openContainers.Remove(container);

            return;
        }

        // OPEN NEW WINDOW
        GameObject obj =
            Instantiate(
                containerWindowPrefab,
                containerParent);

        ContainerUI newUI =
            obj.GetComponent<ContainerUI>();

        newUI.container =
            container;

        newUI.Refresh();

        openContainers.Add(
            container,
            newUI);
    }

    // =========================================
    // REFRESH ALL
    // =========================================

    public void RefreshAll()
    {
        List<ItemContainerInstance> dead =
            new List<ItemContainerInstance>();

        foreach (
            KeyValuePair<
                ItemContainerInstance,
                ContainerUI> pair
            in openContainers)
        {
            if (pair.Value == null)
            {
                dead.Add(pair.Key);

                continue;
            }

            pair.Value.Refresh();
        }

        // CLEANUP
        foreach (
            ItemContainerInstance key
            in dead)
        {
            openContainers.Remove(key);
        }
    }
}