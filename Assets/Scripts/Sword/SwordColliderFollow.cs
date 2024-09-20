using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordColliderFollow : MonoBehaviour
{
    private Collider swordCollider;

    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
