using UnityEngine;
using UnityEngine.UI;

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

    [Header("Stats")]
    public int maxHealth = 250;

    public int currentHealth;

    [Header("Combat")]
    public int damage = 5;

    // LOWER = FASTER
    public float attackSpeed = 1.5f;

    // MELEE RANGE
    public float attackRange = 2f;

    [Header("UI")]
    public Image healthFill;

    [Header("Damage Text")]
    public GameObject damageTextPrefab;

    public Transform damageTextSpawnPoint;

    [Header("Respawn")]
    public float respawnTime = 5f;

    private void Start()
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
                maxHealth
            );

        Debug.Log(
            enemyName +
            " took " +
            amount +
            " damage."
        );

        UpdateHealthBar();

        SpawnDamageText(amount);

        // ENGAGE ENEMY WHEN HIT
        EnemyAI ai =
            GetComponent<EnemyAI>();

        if (ai != null)
        {
            ai.Engage();
        }

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
            damageTextSpawnPoint == null
        )
            return;

        Vector3 randomOffset =
            new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(0f, 0.5f),
                0
            );

        GameObject textObj =
            Instantiate(
                damageTextPrefab,
                damageTextSpawnPoint.position +
                randomOffset,
                Quaternion.identity
            );

        DamageText damageText =
            textObj.GetComponent<DamageText>();

        if (damageText != null)
        {
            damageText.SetDamage(amount);
        }
    }

    void Die()
    {
        Debug.Log(enemyName + " died.");

        gameObject.SetActive(false);

        Invoke(
            nameof(Respawn),
            respawnTime
        );
    }

    void Respawn()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();

        gameObject.SetActive(true);
    }
}