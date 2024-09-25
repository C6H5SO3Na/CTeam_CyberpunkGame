using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossHitBox : MonoBehaviour
{
    public GameObject boss;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Handle sword collision and apply damage to the boss
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object that collided with the boss is tagged as "PlayerWeapon"
        if (collision.gameObject.CompareTag("Sword"))
        {
            // Try to get the SwordComponent from the weapon (or its parent object)
            SwordComponent sword = collision.gameObject.GetComponentInParent<SwordComponent>();

            // If the SwordComponent is found, reduce the boss's HP based on SwordPower
            if (sword != null)
            {
                // Apply damage to the boss's HP
                boss.GetComponent<BossController>().HP -= sword.SwordPower;

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided with the boss is tagged as "PlayerWeapon"
        if (other.gameObject.CompareTag("Sword"))
        {
            // Try to get the SwordComponent from the weapon (or its parent object)
            SwordComponent sword = other.gameObject.GetComponentInParent<SwordComponent>();

            // If the SwordComponent is found, reduce the boss's HP based on SwordPower
            if (sword != null)
            {
                // Apply damage to the boss's HP
                boss.GetComponent<BossController>().HP -= sword.SwordPower;

            }
        }
    }
}
