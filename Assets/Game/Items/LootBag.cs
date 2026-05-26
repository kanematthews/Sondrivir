using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    [Header("Loot")]
    public List<ItemStack> items =
        new List<ItemStack>();

    [Header("Lifetime")]
    public float despawnTime = 120f;

    void Start()
    {
        Destroy(
            gameObject,
            despawnTime);
    }

    // REMOVE ITEM FROM BAG
    public void RemoveItem(
        ItemStack stack)
    {
        if (items.Contains(stack))
        {
            items.Remove(stack);
        }

        // DESTROY BAG IF EMPTY
        if (items.Count <= 0)
        {
            Destroy(gameObject);
        }
    }
}