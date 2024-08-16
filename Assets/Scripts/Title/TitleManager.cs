using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pressButton;
    float timeSecCnt = 0.0f;//�b�P��

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
                if (Input.GetKeyDown(KeyCode.A))
                {
                    //�{�^�����������Ɠ_�ł������Ȃ�
                    pressButton.GetComponent<PressButtonController>().changeAmount = 8.0f;
                    phase = Phase.AfterPressButton;
                }
                break;

            case Phase.AfterPressButton:
                ToGameScene();
                break;
        }
    }

    /// <summary>
    /// �Q�[���V�[���֐��ڂ���Ƃ��̏���
    /// </summary>
    void ToGameScene()
    {
        timeSecCnt += Time.deltaTime;
        if (timeSecCnt < 2.0f) { return; }
        SceneManager.LoadScene("Map");
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
