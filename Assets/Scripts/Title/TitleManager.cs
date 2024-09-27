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
    [SerializeField] public SoundGenerator sound;
    //[SerializeField] private SceneLoaderAnimation sceneLoaderAnimetion;
    [SerializeField] private GameObject sceneLoader;
    enum Phase
    {
        Fadein, BeforePressButton, AfterPressButton, Fadeout, ToGameScene
    }

    Phase phase;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.visible = false;
        timeSecCnt = 0.0f;
        phase = Phase.Fadein;
        sound.GenerateSoundByID("101");
        GameManager.stage = 1;
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
            case Phase.Fadein:
                fade.Fadein();
                Debug.Log(fade.IsFade());
                if (!fade.IsFade())
                {
                    phase = Phase.BeforePressButton;
                }
                break;

            case Phase.BeforePressButton:
                if (Input.anyKeyDown)
                {
                    //ボタンが押されると点滅が速くなる
                    pressAnyButton.GetComponent<PressAnyButtonController>().changeAmount = 8.0f;
                    sound.GenerateSoundByID("301");
                    phase = Phase.AfterPressButton;
                }
                break;

            case Phase.AfterPressButton:
                timeSecCnt += Time.deltaTime;
                if (timeSecCnt >= 2.0f)
                {
                    phase = Phase.Fadeout;
                    pressAnyButton.gameObject.SetActive(false);
                }
                break;

            case Phase.Fadeout:
                fade.Fadeout();
                if (!fade.IsFade())
                {
                    phase = Phase.ToGameScene;
                }
                break;

            case Phase.ToGameScene:
                ToGameScene();
                break;
        }
    }

    /// <summary>
    /// ゲームシーンへ推移するときの処理
    /// </summary>
    void ToGameScene()
    {
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
