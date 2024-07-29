using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signboard_05_Rotate_Y : MonoBehaviour

{

    public float Speed;
    
    void Update()
    {

        transform.Rotate(new Vector3(0f, Speed, 0f) * Time.deltaTime);
    }
}