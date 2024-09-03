using UnityEngine;

public class RangedAttackState : IAIState
{
    private AIScript ai;
    private float projectileCooldown;
    public RangedAttackState()
    {
        projectileCooldown = 4.0f;
    }

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.canRangeattack = false;
        ai.Invoke(nameof(ai.ResetRangeCD), projectileCooldown);
        ai.isAttackingmelee = false;
        ai.isRunning = false;
        ai.isWalking = false;
        ai.isAttackingranged = true;
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            ai.ChangeState(new HurtState());
        }
        else if (!ai.playerInSightRange)
        {
            ai.ChangeState(new PatrolState());
        }
        else
        {
            ai.enemyAnim.SetTrigger("isAttack");
            //RangedAttack();
            ai.ChangeState(new ChaseState());
        }
    }

    public void Exit(AIScript ai)
    {
    }

    //public void RangedAttack()
    //{
    //    ai.agent.SetDestination(ai.transform.position);
    //    Vector3 direction = (ai.player.position - ai.transform.position).normalized;
    //    Quaternion lookRotation = Quaternion.LookRotation(direction);
    //    ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, lookRotation, Time.deltaTime * 5f);

    //    Vector3 projectileSpawnOffset = ai.transform.forward * 2.0f;
    //    Vector3 projectileSpawnPosition = ai.Firepoint.transform.position;
    //    GameObject projectile = GameObject.Instantiate(ai.projectilePrefab, projectileSpawnPosition, Quaternion.identity);
    //    Vector3 directionToPlayer = (ai.player.position - projectileSpawnPosition);
    //    projectile.transform.forward = directionToPlayer;
    //    Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
    //    projectileRb.velocity = directionToPlayer * ai.projectileSpeed;
    //    float forceFactor = 1f;
    //    projectileRb.AddForce(Vector3.up * forceFactor, ForceMode.Impulse);
    //}
}

