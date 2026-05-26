using UnityEngine;

public class LootBag : Container
{
    [Header("Loot Bag")]
    public float despawnTime = 30f;

    protected override void Awake()
    {
        base.Awake();

        containerName = "Loot Bag";
    }

    void Start()
    {
        Destroy(
            gameObject,
            despawnTime);
    }
}