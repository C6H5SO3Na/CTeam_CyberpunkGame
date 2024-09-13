using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Image pressAnyButton;
    [SerializeField] Fade fade;
    float timeSecCnt = 0.0f;//秒単位

    enum Phase
    {
        BeforePressButton, AfterPressButton
    }

    Phase phase;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        timeSecCnt = 0.0f;
        phase = Phase.BeforePressButton;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            EndGame();
        }

        switch (phase)
        {
            case Phase.BeforePressButton:
                if (Input.anyKeyDown)
                {
                    //ボタンが押されると点滅が速くなる
                    pressAnyButton.GetComponent<PressAnyButtonController>().changeAmount = 8.0f;
                    phase = Phase.AfterPressButton;
                }
                break;

            case Phase.AfterPressButton:
                ToGameScene();
                break;
        }
    }

    /// <summary>
    /// ゲームシーンへ推移するときの処理
    /// </summary>
    void ToGameScene()
    {
        timeSecCnt += Time.deltaTime;
        if (timeSecCnt < 2.0f) { return; }
        if (fade.IsFade()) { return; }
        fade.Fadeout();
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// アプリケーション終了
    /// </summary>
    void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
