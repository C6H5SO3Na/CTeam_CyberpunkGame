using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] public int maxHP;
    [SerializeField] public int maxSP;
    [SerializeField] public int nowHP;
    [SerializeField] public int nowSP;

    private void Start()
    {
        {
            nowHP = maxHP;
            nowSP = 0;
        }
    }

    public void PlayerDamage(int damage)
    {
        nowHP -= damage;
        if(nowHP <= 0)
        {
            nowHP = 0;
            GameOver();
        }
    }

    public void PlayerSPAdd(int SP)
    {
        nowSP += SP;
        if (nowHP <= maxSP)
        {
            nowHP = maxSP;
            //‘å‹Z‚ª‚Å‚«‚é
        }
    }

    public void PlayerSPReset()
    {
        nowSP = 0;
    }

    void GameOver()
    {
        Debug.Log("GameOver");
        //Ž€–Sˆ—
    }
}
