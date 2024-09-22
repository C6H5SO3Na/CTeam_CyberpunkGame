using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signboard_05_Rotate_Z : MonoBehaviour

{

    public float Speed;
    
    void Update()
    {

        transform.Rotate(new Vector3(0f, 0f, Speed) * Time.deltaTime);
    }
}