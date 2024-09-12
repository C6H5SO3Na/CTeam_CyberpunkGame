using UnityEngine;
using System.Collections;

public class RangedAttackState : IAIState
{
    private AIScript ai;
    private float projectileCooldown;
    private bool isFiringLaser = false;
    private float laserDamageTimer = 0f;

    // Laser-related variables
    private float laserDuration = 2f;
    private float laserDamageRate = 0.5f;
    private float predictionFactor = 0.5f;
    private float laserRotationSpeed = 3f;

    public RangedAttackState()
    {
        projectileCooldown = 4.0f; // Cooldown between attacks
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

        if (ai.laser != null)
        {
            ai.laser.enabled = false;
        }

        ai.enemyAnim.SetTrigger("isAttack");
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
        else if (ai.canRangeattack && !isFiringLaser)
        {
            FireLaser();
        }
    }

    public void Exit(AIScript ai)
    {
        // Logic to handle exit from RangedAttackState
        ai.isAttackingranged = false;
    }

    // Method to be triggered by the animation event
    public void FireLaser()
    {
        if (!isFiringLaser)
        {
            ai.StartCoroutine(FireLaserCoroutine());
        }
    }

    private IEnumerator FireLaserCoroutine()
    {
        isFiringLaser = true;

        // Enable the pre-existing laser (LineRenderer)
        if (ai.laser != null)
        {
            ai.laser.enabled = true;
        }

        // Instantiate the beamsource prefab as a child of the Firepoint, so it moves with the Firepoint
        GameObject spawnedBeamSource = null;
        if (ai.beamsource != null)
        {
            // Instantiate and set it as a child of Firepoint to follow its movement
            spawnedBeamSource = GameObject.Instantiate(ai.beamsource, ai.Firepoint.transform.position, ai.Firepoint.transform.rotation, ai.Firepoint.transform);

            // Activate particle system inside the instantiated object
            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Play(); // Play the particle effect
            }
        }

        Vector3 predictedPlayerPosition = PredictPlayerPosition();

        float timer = 0f;
        while (timer < laserDuration)
        {
            Vector3 laserDirection = (predictedPlayerPosition - ai.Firepoint.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(laserDirection);
            ai.Firepoint.transform.rotation = Quaternion.Slerp(ai.Firepoint.transform.rotation, targetRotation, Time.deltaTime * laserRotationSpeed);

            if (ai.laser != null)
            {
                ai.laser.SetPosition(0, ai.Firepoint.transform.position); // Laser start point
                ai.laser.SetPosition(1, predictedPlayerPosition);          // Laser end point
            }

            // Apply laser damage periodically
            laserDamageTimer += Time.deltaTime;
            if (laserDamageTimer >= laserDamageRate)
            {
                ApplyLaserDamage();
                laserDamageTimer = 0f;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        
        if (ai.laser != null)
        {
            ai.laser.enabled = false;
        }
        if (spawnedBeamSource != null)
        {
            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Stop(); 
            }
            GameObject.Destroy(spawnedBeamSource); 
        }

        isFiringLaser = false;

        ai.ChangeState(new IdleState());
    }



    private Vector3 PredictPlayerPosition()
    {
        Rigidbody playerRb = ai.player.GetComponent<Rigidbody>();
        Vector3 predictedPosition = ai.player.position + playerRb.velocity * predictionFactor;

        float playerHeight = ai.player.GetComponent<Collider>().bounds.extents.y;
        predictedPosition.y += playerHeight * 2f; 

        return Vector3.Lerp(ai.player.position, predictedPosition, predictionFactor);
    }

    private void ApplyLaserDamage()
    {
        Vector3 laserStartPoint = ai.Firepoint.transform.position;
        Vector3 laserDirection = (ai.player.position - laserStartPoint).normalized;
        float laserMaxDistance = 100f;

        if (Physics.Raycast(laserStartPoint, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("PlayerHit");
                // Apply damage to the player here
            }
        }
    }
}
