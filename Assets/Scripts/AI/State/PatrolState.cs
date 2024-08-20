using UnityEngine.AI;
using UnityEngine;

public class PatrolState : IAIState
{
    private AIScript ai;
    private int currentPatrolIndex = 0;
    private float idleTime = 2f;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private float stuckTimer = 0f;
    private const float stuckThreshold = 2f;

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isWalking = true;
        ai.isRunning = false;
        ai.isAttackingmelee = false;

        if (ai.patrolPoints == null || ai.patrolPoints.Count == 0)
        {
            Debug.LogError("パトロールポイントはまだ設定しない。");
            return;
        }

        ai.agent.speed = Random.Range(1.5f, 3f);
        MoveToNextPatrolPoint();
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            ai.ChangeState(new HurtState());
        }
        else if (ai.playerInSightRange)
        {
            ai.ChangeState(new ChaseState());
        }
        else
        {
            Patrol();
        }
    }

    public void Exit(AIScript ai)
    {
        ai.isWalking = false;
        isIdle = false;
        idleTimer = 0f;
    }

    private void Patrol()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                isIdle = false;
                MoveToNextPatrolPoint();
            }
        }
        else
        {
            if (!ai.agent.pathPending && ai.agent.remainingDistance < 0.5f)
            {
                isIdle = true;
                idleTime = Random.Range(1f, 3f);
                idleTimer = 0f;
            }
            else
            {
                if (ai.agent.velocity.sqrMagnitude < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                    if (stuckTimer > stuckThreshold)
                    {
                        stuckTimer = 0f;
                        MoveToNextPatrolPoint();
                    }
                }
                else
                {
                    stuckTimer = 0f;
                }
            }
        }
    }

    private void MoveToNextPatrolPoint()
    {
        if (ai.patrolPoints.Count == 0)
            return;

        bool pathFound = false;
        int attempts = 0;
        int maxAttempts = ai.patrolPoints.Count;

        while (!pathFound && attempts < maxAttempts)
        {
            GameObject nextPatrolPoint = ai.patrolPoints[currentPatrolIndex];

            NavMeshPath path = new NavMeshPath();
            if (ai.agent.CalculatePath(nextPatrolPoint.transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                ai.agent.SetDestination(nextPatrolPoint.transform.position);
                pathFound = true;
            }
            else
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % ai.patrolPoints.Count;
                attempts++;
            }
        }

        if (!pathFound)
        {
            Debug.LogWarning("行く場所がない");
        }
        else
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % ai.patrolPoints.Count;
        }
    }
}
