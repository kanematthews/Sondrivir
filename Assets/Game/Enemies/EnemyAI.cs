using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;

    private EnemyStats enemyStats;

    private EnemyAnimator enemyAnimator;

    private PlayerStats player;

    private float attackTimer;

    private bool attacking;

    private bool engaged;

    void Start()
    {
        agent =
            GetComponent<NavMeshAgent>();

        // IMPORTANT
        // STOP NAVMESH ROTATION FIGHTING
        if (agent != null)
        {
            agent.updateRotation = false;
        }

        enemyStats =
            GetComponent<EnemyStats>();

        enemyAnimator =
            GetComponent<EnemyAnimator>();

        player =
            FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.transform.position
            );

        // HOSTILE ENEMIES AUTO AGGRO
        if (
            enemyStats.behaviour ==
            EnemyBehaviour.Hostile
        )
        {
            if (distance <= enemyStats.aggroRange)
            {
                engaged = true;
            }
        }

        // PASSIVE ENEMIES WAIT
        if (!engaged)
        {
            if (agent != null)
            {
                agent.isStopped = true;
            }

            return;
        }

        // FACE PLAYER
        FaceTarget();

        // MOVE INTO RANGE
        if (distance > enemyStats.attackRange)
        {
            attacking = false;

            if (agent != null)
            {
                agent.isStopped = false;

                agent.SetDestination(
                    player.transform.position
                );
            }
        }
        else
        {
            // STOP MOVING
            if (agent != null)
            {
                agent.isStopped = true;
            }

            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= enemyStats.attackSpeed)
        {
            attackTimer = 0f;

            attacking = true;

            // PLAY ATTACK
            if (enemyAnimator != null)
            {
                enemyAnimator.PlayAttack();
            }

            // HIT DELAY
            Invoke(nameof(ApplyDamage), 0.25f);
        }
    }

    void ApplyDamage()
    {
        if (player == null)
            return;

        player.TakeDamage(
            enemyStats.damage
        );

        Debug.Log(
            enemyStats.enemyName +
            " hit player for " +
            enemyStats.damage
        );
    }

    void FaceTarget()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.transform.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 8f
            );
    }

    public void Engage()
    {
        engaged = true;
    }
}