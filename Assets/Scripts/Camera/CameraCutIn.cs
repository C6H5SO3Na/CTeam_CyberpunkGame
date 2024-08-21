using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraCutIn : MonoBehaviour
{
    bool flag = false;
    Camera cam;
    [SerializeField] GameObject player;
    Vector3 playerDirection;
    Vector3 preAngle;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        playerDirection = cam.transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = cam.transform.position;
        Vector3 cameraAngle = cam.transform.localEulerAngles;
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerDirection = cam.transform.position - player.transform.position;
            flag = !flag;
            preAngle = cam.transform.localEulerAngles;
        }

        if (flag)
        {
            cameraPos.x = player.transform.position.x - playerDirection.x * 2;
            cameraPos.z = player.transform.position.z - playerDirection.z * 2;
            cameraAngle.y = preAngle.y - 180.0f;
        }
        else
        {

        }
        cam.transform.position = cameraPos;
        cam.transform.localEulerAngles = cameraAngle;
    }
}
