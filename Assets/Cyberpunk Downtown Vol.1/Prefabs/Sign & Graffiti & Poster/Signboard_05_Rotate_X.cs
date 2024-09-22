using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signboard_05_Rotate_X : MonoBehaviour

{

    public float Speed;
    
    void Update()
    {

        transform.Rotate(new Vector3(Speed, 0f, 0f) * Time.deltaTime);
    }
}