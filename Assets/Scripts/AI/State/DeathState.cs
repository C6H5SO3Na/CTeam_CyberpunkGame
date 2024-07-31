using UnityEngine;

public class DeathState : IAIState
{
    private AIScript ai;

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.meDead = true;
        ai.agent.isStopped = true;
        ai.isAttackingmelee = false;
        ai.isRunning = false;
        ai.isWalking = false;
    }

    public void Execute(AIScript ai)
    {
        ai.Invoke(nameof(ai.DestroyEnemy), 0.5f);
    }

    public void Exit(AIScript ai)
    {
    }
}
