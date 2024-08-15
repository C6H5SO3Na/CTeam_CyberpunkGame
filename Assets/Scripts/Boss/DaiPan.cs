using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaiPan : MonoBehaviour
{
    public GameObject leftHandPrefab;  // Prefab of the left hand
    public GameObject rightHandPrefab; // Prefab of the right hand
    public float impactForce = 100f;   // Force of the DaiPan impact
    public float distance = 23.0f;     // Distance between the hands
    public float offset = 20.0f;       // Offset from the player
    public float yPosition = 60f;      // Starting Y position of the hands
    public float groundPosition = 0f;  // Ground Y position where the hands stop
    public float waitTime = 3.0f;      // Time to wait before going back to the top

    private GameObject leftHand;
    private GameObject rightHand;
    private bool isDaiPanTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        TriggerDaiPan();
    }

    // Update is called once per frame
    void Update()
    {
        // No need to restart the coroutine here since we're handling it in the coroutine itself.
    }

    void TriggerDaiPan()
    {
        if (!isDaiPanTriggered)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // Calculate the initial positions based on the player's position
            Vector3 leftHandPosition, rightHandPosition;
            CalculateHandPositions(player, out leftHandPosition, out rightHandPosition);

            // Instantiate the hands at the calculated positions
            leftHand = Instantiate(leftHandPrefab, leftHandPosition, Quaternion.identity);
            rightHand = Instantiate(rightHandPrefab, rightHandPosition, Quaternion.identity);

            StartCoroutine(HandleDaiPanLoop());

            isDaiPanTriggered = true;
        }
    }

    void CalculateHandPositions(GameObject player, out Vector3 leftHandPosition, out Vector3 rightHandPosition)
    {
        float playerZ = player.transform.position.z + offset;

        float leftHandX = player.transform.position.x + offset;
        float rightHandX = leftHandX - distance;

        leftHandPosition = new Vector3(leftHandX, yPosition, playerZ);
        rightHandPosition = new Vector3(rightHandX, yPosition, playerZ);
    }

    IEnumerator HandleDaiPanLoop()
    {
        while (true) // Infinite loop until explicitly stopped or object destroyed
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            // Move hands down to the ground
            leftHand.GetComponent<HandMoving>().MoveTo(new Vector3(leftHand.transform.position.x, groundPosition, leftHand.transform.position.z));
            rightHand.GetComponent<HandMoving>().MoveTo(new Vector3(rightHand.transform.position.x, groundPosition, rightHand.transform.position.z));

            yield return new WaitUntil(() => AreHandsStopped());

            // Wait for 3 seconds at the ground position
            yield return new WaitForSeconds(waitTime);

            // Calculate new positions based on player's current position
            Vector3 leftHandPosition, rightHandPosition;
            CalculateHandPositions(player, out leftHandPosition, out rightHandPosition);

            // Move hands back to the top position
            leftHand.GetComponent<HandMoving>().MoveTo(leftHandPosition);
            rightHand.GetComponent<HandMoving>().MoveTo(rightHandPosition);

            yield return new WaitUntil(() => AreHandsStopped());

            // Wait for a short period before restarting the loop
            yield return new WaitForSeconds(1f);
        }
    }

    bool AreHandsStopped()
    {
        HandMoving leftHandScript = leftHand.GetComponent<HandMoving>();
        HandMoving rightHandScript = rightHand.GetComponent<HandMoving>();

        return leftHandScript != null && rightHandScript != null && !leftHandScript.isMoving && !rightHandScript.isMoving;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !AreHandsStopped())
        {
            Debug.Log("DaiPanHit");

            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.AddForce(Vector3.down * impactForce, ForceMode.Impulse);
            }
        }
    }
}
