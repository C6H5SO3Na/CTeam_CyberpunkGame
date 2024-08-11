using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMoving : MonoBehaviour
{
    public float moveSpeed = 10.0f; // Speed at which the hand moves down
    public float targetYPosition = 0.0f; // Y position where the hand will stop moving

    public bool isMoving = true;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            MoveHandDown();
        }
    }

    void MoveHandDown()
    {
        // Move the hand down at the specified speed
        transform.position -= new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // Check if the hand has reached or gone below the target Y position
        if (transform.position.y <= targetYPosition)
        {
            isMoving = false;
            OnHandStop(); // Optional: Call a method when the hand stops moving
        }
    }

    void OnHandStop()
    {
        // Do something when the hand stops moving (e.g., play a sound, trigger an animation)
        Debug.Log("Hand has stopped moving.");
    }
}
