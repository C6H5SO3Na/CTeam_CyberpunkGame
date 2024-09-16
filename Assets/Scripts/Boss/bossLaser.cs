using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossLaser : MonoBehaviour
{
    public GameObject laserPrefab; // Prefab with the LineRenderer component
    public Transform[] startpoints; // Array of startpoints for multiple lasers
    public Transform player; // Reference to the player's transform

    private List<GameObject> instantiatedLasers = new List<GameObject>(); // List to store instantiated prefabs
    private List<Vector3> shotDirections = new List<Vector3>(); // List to store directions for each laser
    private List<float> initialPlayerXPositions = new List<float>(); // List to store the initial randomized X position of the player
    private List<float> initialPlayerZPositions = new List<float>(); // List to store the initial Z position of the player
    public float rotationSpeed = 0.0f; // Speed at which the laser rotates
    public float sweepAngle = 45f; // Half-angle for the sweep (left and right)
    private List<float> currentRotationAngles = new List<float>(); // Current rotation angles for each laser
    private List<bool> rotatingLeft = new List<bool>(); // Whether each laser is rotating left

    private Coroutine attackCoroutine; // Store the coroutine

    public GameObject bossHitBox;

    // Damage related variables
    public float damageRate = 0.5f; // How often damage is applied when player is hit by the laser
    public int laserDamage = 10; // Amount of damage dealt by the laser
    private List<float> lastDamageTimes = new List<float>(); // Track the last time damage was applied for each laser

    void OnEnable()
    {
        // Clear the lists as we will re-initialize them when the script is enabled
        initialPlayerXPositions.Clear();
        initialPlayerZPositions.Clear();
        lastDamageTimes.Clear(); // Clear last damage time tracking

        // Initialize rotation speed for each laser and instantiate the prefabs
        rotationSpeed = Random.Range(5.0f, 8.0f);

        for (int i = 0; i < startpoints.Length; i++)
        {
            Transform startpoint = startpoints[i];

            // Instantiate the laser prefab at each startpoint
            GameObject laserInstance = Instantiate(laserPrefab, startpoint.position, startpoint.rotation);
            instantiatedLasers.Add(laserInstance);

            // Store a random X position (randomized once when script is enabled) and player's current Z position
            initialPlayerXPositions.Add(Random.Range(player.position.x - 5.0f, player.position.x + 5.0f));
            initialPlayerZPositions.Add(player.position.z);

            // Initialize sweeping parameters for each laser
            shotDirections.Add((player.position - startpoint.position).normalized);
            currentRotationAngles.Add(0f);

            // Initialize the last damage time for each laser (start at 0)
            lastDamageTimes.Add(0f);

            // Set the sweeping direction based on whether the laser index is even or odd
            if (i % 2 == 0)
            {
                rotatingLeft.Add(true); // Even-numbered lasers start by sweeping left
            }
            else
            {
                rotatingLeft.Add(false); // Odd-numbered lasers start by sweeping right
            }
        }

        attackCoroutine = StartCoroutine(LaserRoutine());
    }

    void OnDisable()
    {
        if (bossHitBox != null)
        {
            bossHitBox.GetComponent<MeshCollider>().enabled = false;
        }

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        // Destroy all instantiated lasers and clear the list
        foreach (GameObject laser in instantiatedLasers)
        {
            Destroy(laser);
        }
        instantiatedLasers.Clear();
    }

    IEnumerator LaserRoutine()
    {
        while (true)
        {
            for (int i = 0; i < startpoints.Length; i++)
            {
                SweepLaserOnGround(i); // Sweep the laser for each startpoint
                UpdateLaser(i); // Update the laser for each startpoint
            }
            yield return null;
        }
    }

    void SweepLaserOnGround(int index)
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        // Rotate left and right within the sweep angle for each startpoint
        if (rotatingLeft[index])
        {
            currentRotationAngles[index] -= rotationThisFrame;
            if (currentRotationAngles[index] <= -sweepAngle)
            {
                rotatingLeft[index] = false; // Switch direction
            }
        }
        else
        {
            currentRotationAngles[index] += rotationThisFrame;
            if (currentRotationAngles[index] >= sweepAngle)
            {
                rotatingLeft[index] = true; // Switch direction
            }
        }

        // Calculate the target point for each laser based on the stored X and Z positions
        Vector3 targetPoint = new Vector3(
            initialPlayerXPositions[index], // Use the stored randomized X position (fixed when first enabled)
            0, // Ground level (y = 0)
            initialPlayerZPositions[index] // Use the stored Z position (fixed when first enabled)
        );

        // Apply the sweep rotation to the target point
        Vector3 sweepDirection = Quaternion.Euler(0, currentRotationAngles[index], 0) * (targetPoint - startpoints[index].position).normalized;
        shotDirections[index] = sweepDirection;
    }

    void UpdateLaser(int index)
    {
        // Get the LineRenderer from the instantiated laser prefab
        LineRenderer lr = instantiatedLasers[index].GetComponent<LineRenderer>();

        // Set the start of the laser at the startpoint
        lr.SetPosition(0, startpoints[index].position);

        RaycastHit hit;
        if (Physics.Raycast(startpoints[index].position, shotDirections[index], out hit))
        {
            // Set the end of the laser at the point where it hits an object (ground or player)
            lr.SetPosition(1, hit.point);

            if (hit.transform.CompareTag("Player"))
            {
                ApplyLaserDamage(index); // Apply damage to the player
            }
        }
        else
        {
            // If no hit, extend the laser far in the shot direction
            lr.SetPosition(1, startpoints[index].position + shotDirections[index] * 5000);
        }
    }

    void ApplyLaserDamage(int index)
    {
        // Check if enough time has passed since the last damage application for this laser
        if (Time.time >= lastDamageTimes[index] + damageRate)
        {
            //player.GetComponent<PlayerHealth>().ApplyDamage(laserDamage);

            // Update the last damage time for this laser
            lastDamageTimes[index] = Time.time;

            Debug.Log("Player hit by laser, applying " + laserDamage + " damage.");
        }
    }
}
