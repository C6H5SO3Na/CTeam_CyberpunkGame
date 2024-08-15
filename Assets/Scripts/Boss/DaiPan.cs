using System.Collections;
using UnityEngine;

public class DaiPan : MonoBehaviour
{
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public float impactForce = 100f;
    public float distance = 23.0f;
    public float offset = 20.0f;
    public float yPosition = 60f;
    public float groundPosition = 0f;
    public float waitTime = 3.0f;
    public float maxLifetime = 10.0f;  // Maximum lifetime for the hands

    private GameObject leftHandInstance;
    private GameObject rightHandInstance;
    private Coroutine attackCoroutine;

    public GameObject bossHitBox;
    void OnEnable()
    {
        attackCoroutine = StartCoroutine(HandleDaiPanOnce());
    }

    void OnDisable()
    {
        bossHitBox.GetComponent<MeshCollider>().enabled = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator HandleDaiPanOnce()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3 leftHandPosition, rightHandPosition;
        CalculateHandPositions(player, out leftHandPosition, out rightHandPosition);

        leftHandInstance = Instantiate(leftHandPrefab, leftHandPosition, Quaternion.identity);
        rightHandInstance = Instantiate(rightHandPrefab, rightHandPosition, Quaternion.identity);

        leftHandInstance.GetComponent<HandMoving>().MoveTo(new Vector3(leftHandInstance.transform.position.x, groundPosition, leftHandInstance.transform.position.z));
        rightHandInstance.GetComponent<HandMoving>().MoveTo(new Vector3(rightHandInstance.transform.position.x, groundPosition, rightHandInstance.transform.position.z));

        // Wait until hands stop moving or a timeout occurs
        float timeout = 5.0f; // Timeout duration in seconds
        float elapsed = 0f;

        while (!AreHandsStopped() && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(waitTime);


        // Ensure the script is disabled after the attack
        this.enabled = false;
    }

    void CalculateHandPositions(GameObject player, out Vector3 leftHandPosition, out Vector3 rightHandPosition)
    {
        float playerZ = player.transform.position.z + offset;

        float leftHandX = player.transform.position.x + offset;
        float rightHandX = leftHandX - distance;

        leftHandPosition = new Vector3(leftHandX, yPosition, playerZ);
        rightHandPosition = new Vector3(rightHandX, yPosition, playerZ);
    }

    bool AreHandsStopped()
    {
        if (leftHandInstance == null || rightHandInstance == null)
        {
            return true;
        }

        HandMoving leftHandScript = leftHandInstance.GetComponent<HandMoving>();
        HandMoving rightHandScript = rightHandInstance.GetComponent<HandMoving>();

        if (leftHandScript == null || rightHandScript == null)
        {
            Debug.LogWarning("HandMoving script not found on one or both hands.");
            return true;
        }

        return !leftHandScript.isMoving && !rightHandScript.isMoving;
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

    private void Start()
    {

    }
}
