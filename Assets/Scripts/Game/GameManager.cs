using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public static int remaining;
    [SerializeField] Fade fade;
    [SerializeField] TextMeshProUGUI text;
    public static int stage = 1;
    public const int maxStage = 3;
    public bool isClear = false;
    float timeSecCnt = 0.0f;//•b’PˆÊ
    public enum Phase
    {
        Fadein, Game, AfterClear, Fadeout, NextStage
    }
    Phase phase;
    // Start is called before the first frame update
    void Start()
    {
        phase = Phase.Fadein;
        timeSecCnt = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (phase)
        {
            case Phase.Fadein:
                fade.Fadein();

                if (!fade.IsFade())
                {
                    phase = Phase.Game;
                }
                break;

            case Phase.Game:
                if (isClear)
                {
                    phase = Phase.AfterClear;
                }
                break;

            case Phase.AfterClear:
                text.gameObject.SetActive(true);
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
                    phase = Phase.NextStage;
                }
                break;
            case Phase.NextStage:
                NextStage();
                break;
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
