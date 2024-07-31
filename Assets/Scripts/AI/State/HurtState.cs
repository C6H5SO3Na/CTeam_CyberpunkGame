using UnityEngine;
using UnityEngine.AI;

public class HurtState : IAIState
{
    private AIScript ai;
    private float hurtDuration = 0.5f; // Duration of hurt state
    private float hurtTimer = 0f;
    private Vector3 knockbackDirection;
    private float knockbackSpeed = 10f; // Speed of knockback

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isHurt = true;
        ai.agent.isStopped = true;
        ai.isAttackingmelee = false;
        ai.isRunning = false;
        ai.isWalking = false;

        // Play hurt animation
        //ai.enemyAnim.SetTrigger("Hurt");

        // Calculate knockback direction
        knockbackDirection = (ai.transform.position - ai.lastHitPosition).normalized;

        // Reset the hurt timer
        hurtTimer = 0f;

        // Apply an immediate knockback force
        ai.agent.Move(knockbackDirection * knockbackSpeed * Time.deltaTime);
    }

    public void Execute(AIScript ai)
    {
        hurtTimer += Time.deltaTime;

        // Continue moving the AI back during the hurt duration
        if (hurtTimer < hurtDuration)
        {
            ai.agent.Move(knockbackDirection * knockbackSpeed * Time.deltaTime);
        }
        else
        {
            ai.ChangeState(new ChaseState()); // Change to the appropriate state after hurt state ends
        }
    }

    public void Exit(AIScript ai)
    {
        ai.isHurt = false;
        ai.agent.isStopped = false;
    }
}
