using UnityEngine;

public class LootBagInteract : MonoBehaviour
{
    private LootBag lootBag;

    void Start()
    {
        lootBag =
            GetComponent<LootBag>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray =
                Camera.main.ScreenPointToRay(
                    Input.mousePosition);

            RaycastHit hit;

            if (
                Physics.Raycast(
                    ray,
                    out hit,
                    100f))
            {
                Debug.Log(
                    "HIT OBJECT: " +
                    hit.collider.name);

                if (
                    hit.collider.gameObject ==
                    gameObject)
                {
                    Debug.Log(
                        "RIGHT CLICKED BAG");

                    if (LootUI.instance == null)
                    {
                        Debug.LogError(
                            "LootUI.instance is NULL");

                        return;
                    }

                    if (lootBag == null)
                    {
                        Debug.LogError(
                            "LootBag reference is NULL");

                        return;
                    }

                    LootUI.instance.Open(
                        lootBag);
                }
            }
        }
    }
}