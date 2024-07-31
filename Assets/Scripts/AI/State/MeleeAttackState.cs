using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : IAIState
{
    private AIScript ai;
    private float attackCooldown;
    private float currentAttackCooldown;

    public MeleeAttackState()
    {
        attackCooldown = 0.4f;
        currentAttackCooldown = 0f;
    }

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isAttackingmelee = true;
        ai.isRunning = false;
        ai.isWalking = false;
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            ai.ChangeState(new HurtState());
        }
        else if (!ai.playerInMeleeRange)
        {
            ai.ChangeState(new ChaseState());
        }
        else
        {
            Attack();
        }
    }


    public void Exit(AIScript ai)
    {
        ai.isAttackingmelee = false;
    }

    private void Attack()
    {
        Vector3 direction = (ai.player.position - ai.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (currentAttackCooldown <= 0)
        {
            // Perform melee attack logic here
            currentAttackCooldown = attackCooldown;
        }
        else if (currentAttackCooldown > 0)
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }
}

