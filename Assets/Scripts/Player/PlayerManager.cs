using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //public  PlayerManager instance;

    public static int maxHP = 100;
    public static int maxSP = 100;

    public static int nowHP { get; private set; }
    public static int nowSP { get; private set; }

    void Awake()
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
        nowSP = 0;
    }
    public static void PlayerDamage(int damage)
    {
        nowHP -= damage;
        Debug.Log("nowHP");
        PlayerPrefs.SetInt("nowHP", nowHP);
        if (nowHP <= 0)
        {
            nowHP = 0;
            GameOver();
        }
    }

    public static void PlayerSPAdd(int SP)
    {
        nowSP += SP;
        PlayerPrefs.SetInt("nowSP", nowSP);
        Debug.Log("nowSP");
        if (nowSP > maxSP)
        {
            nowSP = maxSP;
            //ëÂãZÇ™Ç≈Ç´ÇÈ
        }
    }

    public static void PlayerSPReset()
    {
        nowSP = 0;
    }

    public static void GameOver()
    {
        Debug.Log("GameOver");
        //éÄñSèàóù
    }
}
