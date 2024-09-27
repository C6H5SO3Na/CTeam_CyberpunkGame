using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearColliderController : MonoBehaviour
{
    [SerializeField] GameManager manager;

    bool AreEnemiesRemaining()
    {
 
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        return enemies.Length > 0;
    }

    void OnTriggerEnter(Collider other)
    {

        if (manager.isClear) { return; }

        if (other.gameObject.CompareTag("Player"))
        {
            if (!AreEnemiesRemaining())
            {
                manager.isClear = true;
            }
            else
            {
                return;
            }
        }
    }
}
