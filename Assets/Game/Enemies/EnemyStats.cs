using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Info")]
    public string enemyName = "Skeleton";

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    public int damage = 5;

    [Header("UI")]
    public Image healthFill;

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
            Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(enemyName + " took " + amount + " damage.");

        UpdateHealthBar();

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
                (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log(enemyName + " died.");

        gameObject.SetActive(false);

        Invoke(nameof(Respawn), respawnTime);
    }

    void Respawn()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();

        gameObject.SetActive(true);
    }
}