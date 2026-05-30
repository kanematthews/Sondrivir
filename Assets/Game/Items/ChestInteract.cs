using UnityEngine;

public class ChestInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 4f;

    private Chest chest;

    // =====================================
    // START
    // =====================================

    void Start()
    {
        chest = GetComponent<Chest>();
    }

    // =====================================
    // UPDATE
    // =====================================

    void Update()
    {
        if (!Input.GetMouseButtonDown(1))
        {
            return;
        }

        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition);

        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100f))
        {
            return;
        }

        if (hit.collider.gameObject != gameObject)
        {
            return;
        }

        // RANGE CHECK

        GameObject player =
            GameObject.FindWithTag("Player");

        if (player != null)
        {
            float dist =
                Vector3.Distance(
                    player.transform.position,
                    transform.position);

            if (dist > interactRange)
            {
                Debug.Log("Too far from chest.");

                return;
            }
        }

        // OPEN LOOT UI

        if (LootUI.instance == null)
        {
            Debug.LogError("LootUI instance is null.");

            return;
        }

        if (chest == null)
        {
            Debug.LogError("No Chest component found.");

            return;
        }

        chest.opened = true;

        LootUI.instance.Open(chest);
    }
}
