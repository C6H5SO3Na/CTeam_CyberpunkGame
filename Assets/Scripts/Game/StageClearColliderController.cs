using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearColliderController : MonoBehaviour
{
    public bool isClear = false;

    [SerializeField] TextMeshProUGUI text;

    void OnTriggerEnter(Collider other)
    {
        text.gameObject.SetActive(true);
        if (isClear) { return; }
        if (other.gameObject.CompareTag("Player"))
        {
            isClear = true;
            Invoke("NextStage", 2.0f);
        }
    }

    void NextStage()
    {
        ++GameManager.stage;
        if (GameManager.stage > GameManager.maxStage)
        {
            SceneManager.LoadScene("BossStage");
        }
        else
        {
            SceneManager.LoadScene("Stage" + GameManager.stage);
        }
    }
}
