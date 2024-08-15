using System.Collections;
using UnityEngine;

public class slidingPunch : MonoBehaviour
{
    public GameObject leftArmPrefab;
    public GameObject rightArmPrefab;
    public float speed = 0.5f; // Speed of the slapping motion
    public float distance = 20.0f; // Distance to move for the slapping motion

    private Coroutine attackCoroutine;
    private GameObject leftArmInstance;
    private GameObject rightArmInstance;
    private Vector3 leftStartPosition;
    private Vector3 rightStartPosition;

    public GameObject bossHitBox;

    void OnEnable()
    {
        leftArmInstance = Instantiate(leftArmPrefab);
        rightArmInstance = Instantiate(rightArmPrefab);

        leftStartPosition = leftArmInstance.transform.position;
        rightStartPosition = rightArmInstance.transform.position;
        attackCoroutine = StartCoroutine(SlappingMotion(leftArmInstance.transform, rightArmInstance.transform));
    }

    void OnDisable()
    {
        bossHitBox.GetComponent<MeshCollider>().enabled = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator SlappingMotion(Transform leftArmTransform, Transform rightArmTransform)
    {
        float time = 0;
        float duration = 1.0f / speed;

        // Slap motion: move away from start positions
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Sin(time * Mathf.PI);

            // Left arm moves to the right
            leftArmTransform.position = leftStartPosition - new Vector3(t * distance, 0, 0);

            // Right arm moves to the left
            rightArmTransform.position = rightStartPosition + new Vector3(t * distance, 0, 0);

            yield return null;
        }

        // Destroy the arms after the attack is done and they have returned to the start position
        Destroy(leftArmInstance);
        Destroy(rightArmInstance);

        this.enabled = false; // Disable the script after completing the attack
    }
}
