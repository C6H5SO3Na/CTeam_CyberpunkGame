using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearColliderController : MonoBehaviour
{
    [SerializeField] GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (manager.isClear) { return; }
        if (other.gameObject.CompareTag("Player"))
        {
            manager.isClear = true;
        }
    }
}
