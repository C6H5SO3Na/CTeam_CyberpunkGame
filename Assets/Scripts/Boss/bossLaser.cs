using System.Collections;
using UnityEngine;

public class bossLaser : MonoBehaviour
{
    private LineRenderer lr;
    public Transform startpoint;
    public Transform player; // Reference to the player's transform

    private Vector3 initialShotDirection; // Direction towards the player when the laser is first fired
    private bool laserFired = false; // Flag to check if the laser has been fired

    public float rotationSpeed = 0.0f; // Speed at which the laser rotates
    public float sweepAngle = 45f; // Half-angle for the sweep (left and right)
    private float currentRotationAngle = 0f;
    private bool rotatingLeft = true; // Direction of rotation

    private float playerDistance; // Distance from startpoint to player

    private Coroutine attackCoroutine; // Store the coroutine

    public GameObject bossHitBox;

    void OnEnable()
    {
        lr = GetComponent<LineRenderer>();
        rotationSpeed = Random.Range(40.0f, 70.0f);

        // Calculate the initial direction and distance to the player
        initialShotDirection = (player.position - startpoint.position).normalized;
        playerDistance = Vector3.Distance(startpoint.position, player.position);

        lr.enabled = true;
        laserFired = false; // Reset the laserFired flag when the attack starts
        attackCoroutine = StartCoroutine(LaserRoutine());
    }

    void OnDisable()
    {
        bossHitBox.GetComponent<MeshCollider>().enabled = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        lr.enabled = false; // Disable the laser line renderer when the attack stops
    }

    IEnumerator LaserRoutine()
    {
        while (true)
        {
            if (!laserFired)
            {
                FireLaserAtPlayer(); // Fire at the player first
            }
            else
            {
                SweepLaserOnGround(); // Rotate the laser to shoot at different ground points
                UpdateLaser();
            }

            yield return null;
        }

        this.enabled = false; // Disable the script after completing the attack (though this is unlikely to run in the current loop structure)
    }

    void FireLaserAtPlayer()
    {
        // Calculate the initial direction to the player
        initialShotDirection = (player.position - startpoint.position).normalized;
        playerDistance = Vector3.Distance(startpoint.position, player.position); // Calculate the distance to the player
        laserFired = true; // Set the flag to true so this only happens once

        lr.SetPosition(0, startpoint.position);
        UpdateLaser(); // Initial update for the laser position
    }

    void SweepLaserOnGround()
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        // Rotate left and right within the sweep angle in the front
        if (rotatingLeft)
        {
            currentRotationAngle -= rotationThisFrame;
            if (currentRotationAngle <= (-sweepAngle + 45))
            {
                rotatingLeft = false; // Switch direction
                
            }
        }
        else
        {
            currentRotationAngle += rotationThisFrame;
            if (currentRotationAngle >= (sweepAngle + 45))
            {
                rotatingLeft = true; // Switch direction
                
            }
        }

        // Calculate the next ground target point based on front-facing rotation
        Vector3 groundTargetPoint = startpoint.position + Quaternion.Euler(0, currentRotationAngle, 0) * startpoint.forward * playerDistance;
        groundTargetPoint.y = 0; // Ensure the target point is on the ground

        // Update the shot direction to point towards the ground target
        initialShotDirection = (groundTargetPoint - startpoint.position).normalized;
    }

    void UpdateLaser()
    {
        RaycastHit hit;

        if (Physics.Raycast(startpoint.position, initialShotDirection, out hit))
        {
            lr.SetPosition(1, hit.point);

            if (hit.transform.CompareTag("Player"))
            {
                // Optionally destroy the player or apply damage
                // Destroy(hit.transform.gameObject);
            }
        }
        else
        {
            lr.SetPosition(1, startpoint.position + initialShotDirection * 5000);
        }
    }
}
