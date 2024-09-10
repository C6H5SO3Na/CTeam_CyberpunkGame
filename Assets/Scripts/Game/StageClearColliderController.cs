using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearColliderController : MonoBehaviour
{
    public static int stage = 1;
    const int maxStage = 3;
    bool isClear = false;

    void OnTriggerEnter(Collider other)
    {
        if (isClear) { return; }
        if (other.gameObject.CompareTag("Player"))
        {
            isClear = true;
            Invoke("NextStage", 2.0f);
        }
    }

    void NextStage()
    {
        ++stage;
        if (stage > maxStage)
        {
            SceneManager.LoadScene("BossStage");
        }
        else
        {
            SceneManager.LoadScene("Stage" + stage);
        }
    }
}
