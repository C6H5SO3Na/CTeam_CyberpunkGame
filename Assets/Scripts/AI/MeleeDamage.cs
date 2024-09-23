using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDamage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("PlayerHit");
        }
    }

    public void OnMeleeCollider()
    {
        CapsuleCollider[] colliders = GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider collider in colliders)
        {
            collider.enabled = true;
        }
    }

    public void OffMeleeCollider()
    {
        CapsuleCollider[] colliders = GetComponentsInChildren<CapsuleCollider>();
        foreach (CapsuleCollider collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
