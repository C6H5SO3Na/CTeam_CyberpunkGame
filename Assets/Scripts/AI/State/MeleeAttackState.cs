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
        attackCooldown = 0.4f; // Cooldown duration
        currentAttackCooldown = 0f;
    }

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.isAttackingmelee = true;
        ai.isRunning = false;
        ai.isWalking = false;

        // Trigger attack animation
        ai.enemyAnim.SetTrigger("MeleeAttack");

        // Reset cooldown to ensure it starts fresh
        currentAttackCooldown = 0f; // Reset it here for proper timing
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
        // Face the player before attacking
        Vector3 direction = (ai.player.position - ai.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Make sure the attack can only happen when the cooldown has finished
        if (currentAttackCooldown <= 0)
        {
            // Play attack animation
            ai.enemyAnim.SetTrigger("MeleeAttack");

            // Reset cooldown after attack
            currentAttackCooldown = attackCooldown;

            // Perform attack logic (e.g., damage the player)
            Debug.Log("Melee attack performed!");
        }
        else
        {
            // Reduce the cooldown timer
            currentAttackCooldown -= Time.deltaTime;
        }

        // After the attack, wait for the animation to finish before transitioning
        AnimatorStateInfo stateInfo = ai.enemyAnim.GetCurrentAnimatorStateInfo(0);

        // Only play sound when the attack is within a specific point in the animation
        if (stateInfo.IsName("MeleeAttack") && stateInfo.normalizedTime >= 0.4f && stateInfo.normalizedTime < 0.5f)
        {
            // Ensure the sound only plays once during this section of the animation
            if (!ai.audioSource.isPlaying)
            {
                ai.audioSource.PlayOneShot(ai.meleeSound);
            }
        }
        if (stateInfo.IsName("MeleeAttack") && stateInfo.normalizedTime >= 1.0f)
        {
            ai.ChangeState(new ChaseState());
        }
    }

}
