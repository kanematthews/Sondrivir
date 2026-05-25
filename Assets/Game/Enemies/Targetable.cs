using UnityEngine;

public class Targetable : MonoBehaviour
{
    public static Targetable CurrentTarget;

    private void OnMouseDown()
    {
        CurrentTarget = this;

        Debug.Log("Targeted: " + gameObject.name);
    }
}