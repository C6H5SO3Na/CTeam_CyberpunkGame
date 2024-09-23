using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class IdleState : IAIState
{
    private AIScript ai;

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isWalking = false;
        ai.isRunning = false;
        ai.isAttackingmelee = false;
    }

    public void Execute(AIScript ai)
    {
        if (ai.playerInSightRange)
        {
            ai.ChangeState(new ChaseState());
        }
        else if (ai.playerInMeleeRange)
        {
            ai.ChangeState(new MeleeAttackState());
        }
        else
        {
            ai.ChangeState(new PatrolState());
        }
    }

    public void Exit(AIScript ai)
    {
    }
}

