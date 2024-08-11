using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaiPan : MonoBehaviour
{
    public GameObject leftHandPrefab;  // Prefab of the left hand
    public GameObject rightHandPrefab; // Prefab of the right hand
    public float impactForce = 100f;   // Force of the DaiPan impact

    private GameObject leftHand;
    private GameObject rightHand;
    private bool isDaiPanTriggered = false;


    // Start is called before the first frame update
    void Start()
    {
        // Optionally trigger DaiPan on start or based on some other event
        TriggerDaiPan();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TriggerDaiPan()
    {
        if (!isDaiPanTriggered)
        {
            // Define the fixed distance between the left and right hands on the X axis
            float distance = 20f;  // Adjust this value as needed

            // Generate a random Z position
            float randomZ = Random.Range(-20f, 60f);

            // Define fixed X positions for the left and right hands
            float leftHandX = Random.Range(-20f, 60f);  // Example fixed X position for the left hand
            float rightHandX = leftHandX + distance;  // Calculate the right hand X position

            // Fixed Y position
            float yPosition = 60f;

            // Calculate positions for the left and right hands
            Vector3 leftHandPosition = new Vector3(leftHandX, yPosition, randomZ);
            Vector3 rightHandPosition = new Vector3(rightHandX, yPosition, randomZ);

            // Spawn the left hand
            leftHand = Instantiate(leftHandPrefab, leftHandPosition, Quaternion.identity);

            // Spawn the right hand
            rightHand = Instantiate(rightHandPrefab, rightHandPosition, Quaternion.identity);

            // Enable the HandMoving script to start movement
            leftHand.GetComponent<HandMoving>().enabled = true;
            rightHand.GetComponent<HandMoving>().enabled = true;

            // Set the DaiPan as triggered
            isDaiPanTriggered = true;
        }
    }


    bool AreHandsStopped()
    {
        // Check if both hands have stopped moving
        HandMoving leftHandScript = leftHand.GetComponent<HandMoving>();
        HandMoving rightHandScript = rightHand.GetComponent<HandMoving>();

        return leftHandScript != null && rightHandScript != null && !leftHandScript.isMoving && !rightHandScript.isMoving;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Playerdetsukeru
        if (other.gameObject.CompareTag("Player") && !AreHandsStopped())
        {
            Debug.Log("DaiPanHit");

            // Apply force to the player or other effects
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.AddForce(Vector3.down * impactForce, ForceMode.Impulse);
            }
        }
    }
}
