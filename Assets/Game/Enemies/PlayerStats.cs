using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int strength = 5;

    public int agility = 5;

    public int intellect = 5;

    [Header("Combat")]
    public int weaponDamage = 5;

    public int bonusDamage = 0;

    // LOWER = FASTER
    public float attackSpeed = 1f;

    [Header("Combat Range")]
    public float attackRange = 2f;

    [Header("Health")]
    public int maxHealth = 100;

    public int currentHealth = 100;

    [Header("Mana")]
    public int maxMana = 100;

    public int currentMana = 100;

    private PlayerHUD hud;

    void Start()
    {
        currentHealth = maxHealth;

        currentMana = maxMana;

        hud =
            FindObjectOfType<PlayerHUD>();

        UpdateHUD();
    }

    // DAMAGE CALCULATION
    public int CalculateDamage()
    {
        int finalDamage =
            weaponDamage +
            strength +
            bonusDamage;

        return finalDamage;
    }

    // TAKE DAMAGE
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
            "Player took " +
            amount +
            " damage."
        );

        UpdateHUD();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // UPDATE UI
    void UpdateHUD()
    {
        if (hud != null)
        {
            hud.UpdateHealth(
                currentHealth,
                maxHealth
            );
        }
    }

    // PLAYER DEATH
    void Die()
    {
        Debug.Log("Player Died");
    }
}