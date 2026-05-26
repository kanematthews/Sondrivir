using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [Header("Container")]
    public string containerName =
        "Container";

    public int slotCount = 8;

    public List<ItemStack> items =
        new List<ItemStack>();
}