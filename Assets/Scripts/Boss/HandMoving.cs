using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMoving : MonoBehaviour
{
    public float moveSpeed = 10f;  // Speed of the hand movement
    public bool isMoving = false;

    public void MoveTo(Vector3 targetPosition)
    {
        StopAllCoroutines();  // Stop any previous movement
        StartCoroutine(MoveHand(targetPosition));
    }

    IEnumerator MoveHand(Vector3 targetPosition)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
