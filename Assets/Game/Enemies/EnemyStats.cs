using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EnemyBehaviour
{
    Passive,
    Hostile
}

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Info")]
    public string enemyName = "Skeleton";

    [Header("Behaviour")]
    public EnemyBehaviour behaviour =
        EnemyBehaviour.Hostile;

    public float aggroRange = 8f;

    [Header("Combat")]
    public int maxHealth = 250;

    public int currentHealth;

    public int damage = 5;

    public float attackRange = 2f;

    // 1 = once per second
    public float attackSpeed = 1f;

    [Header("Rewards")]
    public int experienceReward = 25;

    [Header("Loot")]
    public GameObject lootBagPrefab;

    public List<LootDrop> lootTable =
    new List<LootDrop>();

    [Header("UI")]
    public Image healthFill;

    [Header("Damage Text")]
    public GameObject damageTextPrefab;

    public Transform damageTextSpawnPoint;

    void SpawnLootBag()
    {
        if (lootBagPrefab == null)
        {
            return;
        }

        // SPAWN BAG
        GameObject bagObj =
    Instantiate(
        lootBagPrefab,
        transform.position,
        Quaternion.Euler(
            90f,
            0f,
            0f));

        // GET LOOT BAG
        LootBag lootBag =
            bagObj.GetComponent<LootBag>();

        if (lootBag == null)
        {
            return;
        }

        // ROLL LOOT TABLE
        foreach (LootDrop drop in lootTable)
        {
            double roll =
                Random.Range(0f, 100f);

            if (roll <= drop.dropChance)
            {
                ItemStack stack =
                    new ItemStack();

                stack.item =
                    drop.item;

                stack.amount =
                    Random.Range(
                        drop.minAmount,
                        drop.maxAmount + 1);

                lootBag.items.Add(stack);
            }
        }

        // DESTROY EMPTY BAG
        if (lootBag.items.Count <= 0)
        {
            Destroy(bagObj);
        }
    }

    [Header("Respawn")]
    public float respawnTime = 5f;

    void Start()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0,
                maxHealth);

        Debug.Log(
            enemyName +
            " took " +
            amount +
            " damage.");

        UpdateHealthBar();

        SpawnDamageText(amount);

        // PASSIVE ENEMIES BECOME AGGRESSIVE
        EnemyAI ai =
            GetComponent<EnemyAI>();

        if (ai != null)
        {
            ai.Engage();
        }

        // DEAD
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount =
                (float)currentHealth /
                maxHealth;
        }
    }

    void SpawnDamageText(int amount)
    {
        if (
            damageTextPrefab == null ||
            damageTextSpawnPoint == null)
        {
            return;
        }

        // RANDOM OFFSET
        Vector3 randomOffset =
            new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(0f, 0.3f),
                0f);

        // SPAWN POSITION
        Vector3 spawnPosition =
            damageTextSpawnPoint.position +
            randomOffset;

        GameObject textObj =
            Instantiate(
                damageTextPrefab,
                spawnPosition,
                Quaternion.identity);

        // MAKE TEXT RED
        TMPro.TextMeshPro text =
            textObj.GetComponent
            <TMPro.TextMeshPro>();

        if (text != null)
        {
            text.color = Color.red;
        }

        DamageText damageText =
            textObj.GetComponent<DamageText>();

        if (damageText != null)
        {
            damageText.SetDamage(amount);
        }
    }

    void Die()
    {
        Debug.Log(
            enemyName + " died.");

        // SPAWN LOOT BAG
        SpawnLootBag();

        // GIVE PLAYER EXP
        PlayerStats player =
            FindFirstObjectByType<PlayerStats>();

        if (player != null)
        {
            player.GainExperience(
                experienceReward);
        }

        gameObject.SetActive(false);

        Invoke(
            nameof(Respawn),
            respawnTime);
    }


    void Respawn()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();

        gameObject.SetActive(true);
    }
}