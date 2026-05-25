using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Health")]
    public Image healthFill;

    public void UpdateHealth(
        int currentHealth,
        int maxHealth)
    {
        healthFill.fillAmount =
            (float)currentHealth /
            maxHealth;
    }
}