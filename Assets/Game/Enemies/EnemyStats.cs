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

    [Header("UI")]
    public Image healthFill;

    [Header("Damage Text")]
    public GameObject damageTextPrefab;

    public Transform damageTextSpawnPoint;

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
            healthFill == null)
        {
            return;
        }

        // SPAWN FROM HEALTH BAR
        Vector3 spawnPosition =
            healthFill.transform.position;

        // RANDOM OFFSET
        spawnPosition +=
            new Vector3(
                Random.Range(-15f, 15f),
                Random.Range(-5f, 10f),
                0f);

        GameObject textObj =
            Instantiate(
                damageTextPrefab,
                spawnPosition,
                Quaternion.identity);

        // PARENT TO CANVAS
        Canvas canvas =
            FindFirstObjectByType<Canvas>();

        if (canvas != null)
        {
            textObj.transform.SetParent(
                canvas.transform,
                false);
        }

        // MAKE TEXT RED
        TextMeshProUGUI text =
            textObj.GetComponent<TextMeshProUGUI>();

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