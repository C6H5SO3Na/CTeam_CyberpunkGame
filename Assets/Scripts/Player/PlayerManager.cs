using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("PlayerManager instance is not initialized yet.");
            return _instance;
        }
    }

    public static int maxHP = 100;
    public static int maxSP = 100;

    public static int nowHP { get; private set; }
    public static int nowSP { get; private set; }

    void Awake()
    {
        Debug.Log("pmw");
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        ResetPlayerStates();
    }

    public static void ResetPlayerStates()
    {
        nowHP = maxHP;
        PlayerPrefs.SetInt("nowHP", nowHP);
        nowSP = 0;
        PlayerPrefs.SetInt("nowSP", 0);
    }
    public static void PlayerDamage(int damage)
    {
        nowHP -= damage;
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

    private static void GameOver()
    {
        Debug.Log("GameOver");
        //éÄñSèàóù
    }
}
