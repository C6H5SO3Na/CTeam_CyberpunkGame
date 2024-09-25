using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //public  PlayerManager instance;

    public static int maxHP = 200;
    public static int maxSP = 100;

    public static int nowHP { get; private set; } = 200;
    public static int nowSP { get; private set; } = 0;

    public SoundGenerator soundGenerator;

    void Start()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(instance);
        //}
        ResetPlayerStates();
    }

    public static void ResetPlayerStates()
    {
        nowHP = maxHP;
        PlayerPrefs.SetInt("nowHP", nowHP);
        Debug.Log(nowHP + "nowHPis");
        nowSP = 0;
        PlayerPrefs.SetInt("nowSP", nowSP);
    }
    public static void PlayerDamage(int damage)
    {
        nowHP -= damage;
        Debug.Log("HPUI Change");
        PlayerPrefs.SetInt("nowHP", nowHP);
        //Debug.Log(nowHP + "nowHPis");
        //Debug.Log(PlayerPrefs.GetInt("nowHP")+"nowHPis");
        if (nowHP <= 0)
        {
            nowHP = 0;
            GameOver();
        }
    }

    public static void PlayerSPAdd(int SP)
    {
        nowSP += SP;
        if(nowSP > 100) { nowSP = 100; }
        PlayerPrefs.SetInt("nowSP", nowSP);
        Debug.Log("nowSP="+nowSP);
        if (nowSP > maxSP)
        {
            nowSP = maxSP;
            //ëÂãZÇ™Ç≈Ç´ÇÈ
        }
    }

    public static void PlayerSPReset()
    {
        nowSP = 0;
        PlayerPrefs.SetInt("nowSP", nowSP);
    }

    public static void GameOver()
    {
        Debug.Log("GameOver");
        //éÄñSèàóù
        //soundGenerator.GenerateSoundByID("902");
        //StartCoroutine(SEEnding(1f));      
    }

    //private IEnumerator SEEnding(float Time)
    //{
    //    yield return new WaitForSeconds(Time);
    //    soundGenerator.DeleteSoundByID("902");

    //}
}
