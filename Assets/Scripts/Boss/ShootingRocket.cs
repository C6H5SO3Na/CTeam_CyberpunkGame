using System.Collections;
using UnityEngine;

public class ShootingRocket : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform[] launchPoints; // Array of points from where rockets will be launched
    public float heightAbovePlayer = 30.0f; // Height at which the rockets will begin to fall
    public float spreadRadius = 5.0f; // The radius around the player where rockets will spread
    public float ascentSpeed = 10.0f; // Speed at which the rockets ascend
    public float descentDelay = 1.0f; // Delay before rockets start falling after reaching the peak

    private Coroutine attackCoroutine;

    public GameObject bossHitBox;

    void OnEnable()
    {
        attackCoroutine = StartCoroutine(HandleShootingRocket());
    }

    void OnDisable()
    {
        bossHitBox.GetComponent<MeshCollider>().enabled = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator HandleShootingRocket()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < launchPoints.Length; i++)
        {
            // Calculate a random position around the player within the spread radius
            Vector3 randomOffset = new Vector3(
                Random.Range(-spreadRadius, spreadRadius),
                0,
                Random.Range(-spreadRadius, spreadRadius)
            );

            // If it's the first launch point, set offset to zero
            if (i == 0)
            {
                randomOffset = Vector3.zero;
            }

            Vector3 targetPosition = player.transform.position + randomOffset;

            // Instantiate the rocket at the launch point
            GameObject rocketInstance = Instantiate(rocketPrefab, launchPoints[i].position, Quaternion.identity);

            // Add a RocketCollisionHandler script to handle collisions
            RocketCollisionHandler collisionHandler = rocketInstance.AddComponent<RocketCollisionHandler>();
            collisionHandler.SetDestroyOnCollision();

            // Move the rocket upwards to a point directly above the target position
            StartCoroutine(MoveRocketUpwards(rocketInstance, targetPosition));

            // Destroy the rocket after a set time if it hasn't already been destroyed
            Destroy(rocketInstance, 10.0f);
        }

        // Ensure the script is disabled after the attack
        yield return new WaitForSeconds(10.0f);
        this.enabled = false;
    }

    IEnumerator MoveRocketUpwards(GameObject rocket, Vector3 targetPosition)
    {
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        rb.useGravity = false;

        Vector3 peakPosition = targetPosition + Vector3.up * heightAbovePlayer;

        // Ascend to the peak position
        while (rocket != null && Vector3.Distance(rocket.transform.position, peakPosition) > 0.1f)
        {
            rocket.transform.position = Vector3.MoveTowards(rocket.transform.position, peakPosition, ascentSpeed * Time.deltaTime);
            yield return null;
        }

        // Wait for a moment before falling down
        yield return new WaitForSeconds(descentDelay);

        if (rocket != null)
        {
            // Rotate the rocket by 180 degrees on the Z-axis
            rocket.transform.Rotate(0, 0, 180);

            rb.useGravity = true;
            rb.velocity = Vector3.zero;  // Reset velocity to allow natural fall
        }
    }
}

// This script handles collisions for the rockets
public class RocketCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the rocket hits the ground or player
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // Destroy the rocket
        }
    }

    public void SetDestroyOnCollision()
    {
        // Ensure the rocket has a collider component and it's set as a trigger if using OnTriggerEnter
        Collider collider = GetComponent<CapsuleCollider>();
    }
}
