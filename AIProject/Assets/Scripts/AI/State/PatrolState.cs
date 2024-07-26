using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IAIState
{
    private AIScript ai;
    private List<Vector3> patrolPoints;
    private int currentPatrolIndex = 0;
    private float idleTime = 2f; // 各ポイントでの基本待機時間
    private float idleTimer = 0f;
    private bool isIdle = false;
    private float stuckTimer = 0f;
    private const float stuckThreshold = 2f; // Time to consider the agent stuck

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isWalking = true;
        ai.isRunning = false;
        ai.isAttackingmelee = false;

        // パトロールポイントが設定されていない場合は初期化する
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            InitializePatrolPoints();
        }

        ai.agent.speed = Random.Range(1.5f, 3f); // 歩行速度をランダムに設定する
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

    private void InitializePatrolPoints()
    {
        patrolPoints = new List<Vector3>();

        patrolPoints.Add(new Vector3(10, 0, 10));
        patrolPoints.Add(new Vector3(20, 0, 10));
        patrolPoints.Add(new Vector3(20, 0, 20));
        patrolPoints.Add(new Vector3(10, 0, 20));

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Vector3 temp = patrolPoints[i];
            int randomIndex = Random.Range(0, patrolPoints.Count);
            patrolPoints[i] = patrolPoints[randomIndex];
            patrolPoints[randomIndex] = temp;
        }
    }

    private void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Count == 0)
            return;

        Vector3 nextPatrolPoint = patrolPoints[currentPatrolIndex];

        NavMeshPath path = new NavMeshPath();
        if (ai.agent.CalculatePath(nextPatrolPoint, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            ai.agent.SetDestination(nextPatrolPoint);
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
        else
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            MoveToNextPatrolPoint();
        }
    }
}
