using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public static bool isCreated = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Stage") || !SceneManager.GetActiveScene().name.Contains("Boss")) { return; }
        //1つしか生成されないようにする
        if (!isCreated)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.tag = "DontDestroyOnLoad";
            isCreated = true;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        //ボスシーンに推移時にStage1で作成したオブジェクトを削除
        if (!SceneManager.GetActiveScene().name.Contains("Stage") || !SceneManager.GetActiveScene().name.Contains("Boss") && gameObject.CompareTag("DontDestroyOnLoad"))
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            Destroy(gameObject);
            isCreated = false;
        }
    }
}