using UnityEngine;

public class Targetable : MonoBehaviour
{
    public static Targetable CurrentTarget;

    [Header("Target Visuals")]
    public GameObject targetRing;

    public CombatTargetVisual combatVisual;

    // =========================================
    // SELECT TARGET
    // =========================================

    public void SelectTarget()
    {
        // =====================================
        // CLEAR PREVIOUS TARGET
        // =====================================

        if (CurrentTarget != null)
        {
            // RING
            if (
                CurrentTarget.targetRing != null)
            {
                CurrentTarget
                    .targetRing
                    .SetActive(false);
            }

            // BODY PULSE
            if (
                CurrentTarget.combatVisual != null)
            {
                CurrentTarget
                    .combatVisual
                    .StopPulse();
            }
        }

        // =====================================
        // SET NEW TARGET
        // =====================================

        CurrentTarget = this;

        // =====================================
        // ENABLE RING
        // =====================================

        if (targetRing != null)
        {
            targetRing.SetActive(true);
        }

        // =====================================
        // START BODY PULSE
        // =====================================

        if (combatVisual != null)
        {
            combatVisual.StartPulse();
        }

        Debug.Log(
            "Targeted: " +
            gameObject.name);
    }

    // =========================================
    // ON DISABLE
    // =========================================

    private void OnDisable()
    {
        if (CurrentTarget == this)
        {
            CurrentTarget = null;
        }

        // DISABLE RING
        if (targetRing != null)
        {
            targetRing.SetActive(false);
        }

        // STOP PULSE
        if (combatVisual != null)
        {
            combatVisual.StopPulse();
        }
    }
}