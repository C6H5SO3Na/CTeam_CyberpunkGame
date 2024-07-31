using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IAIState
{
    private float strafeDirection = 1f; // 右に移動する場合は1、左に移動する場合は-1
    private float strafeChangeInterval = 1f; // ストレイフの方向を変更する間隔
    private float lastStrafeChangeTime = 0f; // 最後にストレイフの方向を変更した時間

    private float angularSpeed = 30f; // 毎秒の回転速度（度）

    private float currentAngle = 0f; // プレイヤーの周りの現在の角度

    public void Enter(AIScript ai)
    {
        ai.isRunning = true;
        ai.isWalking = false;
        ai.isAttackingmelee = false;
        lastStrafeChangeTime = Time.time; // 最後の方向変更時間を初期化
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            ai.ChangeState(new HurtState());
        }
        else if (ai.playerInMeleeRange && ai.canMeleeAttack)
        {
            ai.ChangeState(new MeleeAttackState());
        }
        else if (ai.playerInAttackRange && ai.canRangedAttack && ai.canRangeattack)
        {
            ai.ChangeState(new RangedAttackState());
        }
        else if (!ai.playerInSightRange)
        {
            ai.ChangeState(new PatrolState());
        }
        else if (!ai.canRangeattack)
        {
            MaintainRangedDistanceAndStrafe(ai);
        }
        else
        {
            ai.agent.SetDestination(ai.player.position);
        }
    }


    public void Exit(AIScript ai)
    {
        ai.isRunning = false;
    }

    private void MaintainRangedDistanceAndStrafe(AIScript ai)
    {
        float distanceToPlayer = Vector3.Distance(ai.transform.position, ai.player.position);
        float desiredDistance = ai.attackRange * 0.8f; // 望ましい距離（例）

        Vector3 directionToPlayer = (ai.player.position - ai.transform.position).normalized;
        Vector3 destination = ai.transform.position;

        // プレイヤーの周りを円形に移動する
        currentAngle += angularSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;

        // 新しい位置を円周上に計算する
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * desiredDistance;

        if (distanceToPlayer > ai.attackRange)
        {
            // 望ましい距離に近づく
            destination = ai.player.position;
        }
        else if (distanceToPlayer < desiredDistance)
        {
            // 望ましい距離を保つために後退する
            destination = ai.transform.position - directionToPlayer * (desiredDistance - distanceToPlayer);
        }

        // ストレイフのロジック
        if (Time.time - lastStrafeChangeTime > strafeChangeInterval)
        {
            strafeDirection *= -1; // ストレイフの方向を変更
            lastStrafeChangeTime = Time.time; // 最後の方向変更時間を更新
        }

        Vector3 strafeDirectionVector = Vector3.Cross(directionToPlayer, Vector3.up) * strafeDirection;

        // ストレイフを含めた新しい目的地を計算
        destination += strafeDirectionVector * ai.strafeSpeed * Time.deltaTime + offset;

        // NavMeshAgentに新しい目的地を設定
        ai.agent.SetDestination(destination);

        // AIがプレイヤーの方向を向くようにする
        Vector3 lookDirection = ai.player.position - ai.transform.position;
        lookDirection.y = 0; // 水平面での回転を維持する
        ai.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
