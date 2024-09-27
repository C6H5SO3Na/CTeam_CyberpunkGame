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
        //1������������Ȃ��悤�ɂ���
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
        //�{�X�V�[���ɐ��ڎ���Stage1�ō쐬�����I�u�W�F�N�g���폜
        if (!SceneManager.GetActiveScene().name.Contains("Stage") || !SceneManager.GetActiveScene().name.Contains("Boss") && gameObject.CompareTag("DontDestroyOnLoad"))
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            Destroy(gameObject);
            isCreated = false;
        }
    }
}