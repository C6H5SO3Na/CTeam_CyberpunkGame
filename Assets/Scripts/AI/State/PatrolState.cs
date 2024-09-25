using UnityEngine.AI;
using UnityEngine;

public class PatrolState : IAIState
{
    private AIScript ai;
    private float idleTime = 2f;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private float stuckTimer = 0f;
    private const float stuckThreshold = 2f;
    private Bounds patrolBounds;

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isWalking = true;
        ai.isRunning = false;
        ai.isAttackingmelee = false;

        if (ai.patrolPoints == null || ai.patrolPoints.Count != 4)
        {
            Debug.LogError("パトロールポイントが正しく設定されていません。4つのパトロールポイントが必要です。");
            return;
        }

        ai.agent.speed = Random.Range(1.5f, 3f);
        CalculatePatrolBounds();
        MoveToNextPatrolPosition();
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
                MoveToNextPatrolPosition();
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
                        MoveToNextPatrolPosition();
                    }
                }
                else
                {
                    stuckTimer = 0f;
                }
            }
        }
    }

    private void MoveToNextPatrolPosition()
    {
        if (ai.patrolPoints.Count != 4)
            return;

        bool pathFound = false;
        int attempts = 0;
        int maxAttempts = 10; // Limit attempts to avoid infinite loop

        while (!pathFound && attempts < maxAttempts)
        {
            Vector3 randomPosition = GetRandomPositionWithinBounds();

            NavMeshPath path = new NavMeshPath();
            if (ai.agent.CalculatePath(randomPosition, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                ai.agent.SetDestination(randomPosition);
                pathFound = true;
            }
            attempts++;
        }

        if (!pathFound)
        {
            Debug.LogWarning("行く場所がない");
        }
    }

    private void CalculatePatrolBounds()
    {
        Vector3 min = ai.patrolPoints[0].transform.position;
        Vector3 max = ai.patrolPoints[0].transform.position;

        for (int i = 1; i < ai.patrolPoints.Count; i++)
        {
            Vector3 point = ai.patrolPoints[i].transform.position;
            min = Vector3.Min(min, point);
            max = Vector3.Max(max, point);
        }

        patrolBounds = new Bounds();
        patrolBounds.SetMinMax(min, max);
    }

    private Vector3 GetRandomPositionWithinBounds()
    {
        float randomX = Random.Range(patrolBounds.min.x, patrolBounds.max.x);
        float randomY = Random.Range(patrolBounds.min.y, patrolBounds.max.y);
        float randomZ = Random.Range(patrolBounds.min.z, patrolBounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
