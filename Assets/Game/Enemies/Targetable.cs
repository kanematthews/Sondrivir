using UnityEngine;

public class Targetable : MonoBehaviour
{
    public static Targetable CurrentTarget;

    [Header("Target Visuals")]
    public GameObject targetRing;

    private void OnMouseDown()
    {
        // LEFT CLICK = TARGET
        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget();
        }
    }

    public void SelectTarget()
    {
        // Disable previous target ring
        if (CurrentTarget != null &&
            CurrentTarget.targetRing != null)
        {
            CurrentTarget.targetRing.SetActive(false);
        }

        // Set new target
        CurrentTarget = this;

        // Enable new target ring
        if (targetRing != null)
        {
            targetRing.SetActive(true);
        }

        Debug.Log("Targeted: " + gameObject.name);
    }

    private void OnDisable()
    {
        if (CurrentTarget == this)
        {
            CurrentTarget = null;
        }

        if (targetRing != null)
        {
            targetRing.SetActive(false);
        }
    }
}