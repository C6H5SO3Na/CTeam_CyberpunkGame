using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] Image selectImage;
    [SerializeField] Fade fade;
    float timeSecCnt = 0.0f;//�b�P��
    int select = 0;

    enum Phase
    {
        Fadein, BeforePressButton, AfterPressButton, Fadeout, ToOtherScene
    }

    Phase phase;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.visible = false;
        timeSecCnt = 0.0f;
        phase = Phase.Fadein;
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
                if (Input.GetButtonDown("Vertical"))
                {
                    select = select == 0 ? 1 : 0;//�O�����Z�q
                    selectImage.gameObject.transform.DOMoveY(328.0f + select * -120.0f, 0.5f).SetEase(Ease.InOutSine);
                }
                if (Input.GetButtonDown("Normal_Attack"))
                {
                    //�{�^�����������Ɠ_�ł������Ȃ�
                    selectImage.GetComponent<PressAnyButtonController>().changeAmount = 8.0f;
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
                    phase = Phase.ToOtherScene;
                }
                break;

            case Phase.ToOtherScene:
                if (select == 0)
                {
                    ToStage();
                }
                else
                {
                    ToTitle();
                }
                break;
        }
    }

    /// <summary>
    /// �Q�[���V�[���֐��ڂ���Ƃ��̏���
    /// </summary>
    void ToStage()
    {
        SceneManager.LoadScene("Stage" + GameManager.stage);
    }

    /// <summary>
    /// �^�C�g���V�[���֐��ڂ���Ƃ��̏���
    /// </summary>
    void ToTitle()
    {
        SceneManager.LoadScene("Title");
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
