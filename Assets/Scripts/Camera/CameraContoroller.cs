using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CameraContoroller : MonoBehaviour
{
    [SerializeField]GameObject player;
    Vector3 direction;//ƒvƒŒƒCƒ„‚ÆƒJƒƒ‰‚Ì‹——£
    // Start is called before the first frame update
    void Start()
    {
        direction = player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position += player.transform.position - direction;
        //direction = player.transform.position;
    }
}
