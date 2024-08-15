using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slidingPunch : MonoBehaviour
{
    public GameObject LeftArm;
    public GameObject RightArm;
    public float speed = 2.0f; // Speed of the slapping motion
    public float distance = 5.0f; // Distance to move for the slapping motion

    private Vector3 leftStartPosition;
    private Vector3 rightStartPosition;
    private bool isLeftSlapping = true;
    private bool isRightSlapping = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject leftArm = Instantiate(LeftArm);
        GameObject rightArm = Instantiate(RightArm);

        leftStartPosition = leftArm.transform.position;
        rightStartPosition = rightArm.transform.position;

        StartCoroutine(SlappingMotion(leftArm.transform, rightArm.transform));
    }

    IEnumerator SlappingMotion(Transform leftArm, Transform rightArm)
    {
        while (true)
        {
            float time = 0;
            float duration = 1.0f / speed;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Sin(time * Mathf.PI);

                // Left arm: move from left to right
                if (isLeftSlapping)
                {
                    leftArm.position = leftStartPosition - new Vector3(t * distance, 0, 0);
                }
                else
                {
                    leftArm.position = leftStartPosition + new Vector3(t * distance, 0, 0);
                }

                // Right arm: move from right to left
                if (isRightSlapping)
                {
                    rightArm.position = rightStartPosition + new Vector3(t * distance, 0, 0);
                }
                else
                {
                    rightArm.position = rightStartPosition - new Vector3(t * distance, 0, 0);
                }

                yield return null;
            }

            // Toggle the slapping direction
            isLeftSlapping = !isLeftSlapping;
            isRightSlapping = !isRightSlapping;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
