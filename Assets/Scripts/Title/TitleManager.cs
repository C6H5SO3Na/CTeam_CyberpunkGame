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
    float timeSecCnt = 0.0f;//�b�P��
    [SerializeField] public SoundGenerator sound;
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

                if (!fade.IsFade())
                {
                    phase = Phase.BeforePressButton;
                }
                break;

            case Phase.BeforePressButton:
                if (Input.anyKeyDown)
                {
                    //�{�^�����������Ɠ_�ł������Ȃ�
                    pressAnyButton.GetComponent<PressAnyButtonController>().changeAmount = 8.0f;
                    phase = Phase.AfterPressButton;
                }
                break;

            case Phase.AfterPressButton:
                timeSecCnt += Time.deltaTime;
                if (timeSecCnt >= 2.0f)
                {
                    phase = Phase.Fadeout;
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
    /// �Q�[���V�[���֐��ڂ���Ƃ��̏���
    /// </summary>
    void ToGameScene()
    {
        SceneManager.LoadScene("Stage1");
    }

    /// <summary>
    /// �A�v���P�[�V�����I��
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
