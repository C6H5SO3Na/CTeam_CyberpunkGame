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
    private float laserDamageRate = 0.1f;


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
        ai.isAttackingranged = false;

    }

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

        if (ai.laser != null)
        {
            ai.laser.enabled = true;  // Turn on the laser only when firing starts
        }

        GameObject spawnedBeamSource = null;
        if (ai.beamsource != null)
        {
            // Create the beam source at the firepoint
            spawnedBeamSource = GameObject.Instantiate(ai.beamsource, ai.Firepoint.transform.position, ai.Firepoint.transform.rotation, ai.Firepoint.transform);

            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Play();  // Play particle effects for the laser
            }
        }

        // Calculate the laser direction only once at the start
        Vector3 playerPosition = SmoothPredictPlayerPosition();
        Vector3 laserDirection = (playerPosition - ai.Firepoint.transform.position).normalized;

        // Set the laser's direction/rotation based on the calculated direction
        Quaternion targetRotation = Quaternion.LookRotation(laserDirection);
        ai.Firepoint.transform.rotation = targetRotation;

        float timer = 0f;
        while (timer < laserDuration)
        {
            // The laser will keep firing in the same direction
            Vector3 laserEndPoint = CalculateLaserEndPoint(laserDirection);

            if (ai.laser != null)
            {
                ai.laser.SetPosition(0, ai.Firepoint.transform.position);  // Set laser start position
                ai.laser.SetPosition(1, laserEndPoint);  // Set laser end position
            }

            // Apply damage if the laser is hitting the player
            if (HitPlayer(laserDirection))
            {
                laserDamageTimer += Time.deltaTime;
                if (laserDamageTimer >= laserDamageRate)
                {
                    ApplyLaserDamage();
                    laserDamageTimer = 0f;  // Reset damage timer after applying damage
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Turn the laser off and clean up after firing
        if (ai.laser != null)
        {
            ai.laser.enabled = false;  // Disable the laser after the attack is over
        }

        // Destroy the spawned beam source (if any) after the laser attack is done
        if (spawnedBeamSource != null)
        {
            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Stop();  // Stop beam particle effects
            }
            GameObject.Destroy(spawnedBeamSource);  // Destroy the beam source
        }

        // Reset the firing state and return to idle
        isFiringLaser = false;
        ai.ChangeState(new IdleState());  // Switch to IdleState after the laser attack is over
    }

    // Check if the laser is hitting the player
    private bool HitPlayer(Vector3 laserDirection)
    {
        float laserMaxDistance = 150f;
        if (Physics.Raycast(ai.Firepoint.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 SmoothPredictPlayerPosition()
    {
        Vector3 currentPosition = ai.player.position;

        float playerHeight = ai.player.GetComponent<Collider>().bounds.extents.y;
        currentPosition.y = ai.player.position.y + playerHeight;

        return currentPosition;
    }

    private Vector3 CalculateLaserEndPoint(Vector3 laserDirection)
    {
        float laserMaxDistance = 150f;
        if (Physics.Raycast(ai.Firepoint.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return ai.player.position + Vector3.up * hit.collider.bounds.extents.y;
            }
            else
            {
                return hit.point;
            }
        }
        return ai.Firepoint.transform.position + laserDirection * laserMaxDistance;
    }

    private void ApplyLaserDamage()
    {
        Debug.Log("PlayerHit! Applying damage...");
    }
}
