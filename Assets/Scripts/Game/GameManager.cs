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
    [SerializeField] GameObject clearImg;
    SoundGenerator sound;
    public static int stage = 1;
    //public int nowStage;
    public const int maxStage = 3;
    public bool isClear = false;
    public bool isDead = false;
    public static　bool isBoss = false;
    float timeSecCnt = 0.0f;//秒単位

    public enum Phase
    {
        Fadein, Game, AfterClear, Fadeout, NextStage, AfterDead, ToGameOver
    }
    Phase phase;
    // Start is called before the first frame update
    void Start()
    {
        phase = Phase.Fadein;
        timeSecCnt = 0.0f;
        sound = GameObject.Find("SoundManager").GetComponent<SoundGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            isClear = true;
        }


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
                    sound.GenerateSoundByID("302");
                    phase = Phase.AfterClear;
                }
                else if (isDead)
                {
                    phase = Phase.AfterDead;
                }
                break;

            case Phase.AfterClear:
                clearImg.gameObject.SetActive(true);
                timeSecCnt += Time.deltaTime;
                if (timeSecCnt >= 2.0f)
                {
                    phase = Phase.Fadeout;
                }
                break;

            case Phase.AfterDead:
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
                    if (isDead)
                    {
                        phase = Phase.ToGameOver;
                    }
                    else
                    {
                        phase = Phase.NextStage;
                        sound.DeleteSoundByID("302");
                    }
                }
                break;
            case Phase.NextStage:
                NextStage();
                break;

            case Phase.ToGameOver:
                ToGameOver();
                break;
        }
    }

    void NextStage()
    {
        ++stage;
        if (isBoss)
        {
            SceneManager.LoadScene("Ending");//エンディングへ
            isBoss = false;
            stage = 1;
        }
        else if (stage > maxStage)
        {
            SceneManager.LoadScene("Boss");
            isBoss = true;
        }
        else
        {
            //nowStage = stage;
            SceneManager.LoadScene("Stage" + stage);
        }
    }

    void ToGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
