using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private NavMeshAgent agent;

    void Start()
    {
        animator =
            GetComponentInChildren<Animator>();

        agent =
            GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (animator == null || agent == null)
            return;

        animator.SetFloat(
            "Speed",
            agent.velocity.magnitude
        );
    }

    public void PlayAttack()
    {
        animator.ResetTrigger("Attack");

        animator.SetTrigger("Attack");
    }
}