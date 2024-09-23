﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPandSPUI : MonoBehaviour
{
    public RectTransform HPRectTransform;
    public RectTransform SPRectTransform;
    float longPerPercent = 7f; //1%ごとにUIの変更量
    int HPpercent;
    int SPpercent;
    
    
    // Start is called before the first frame update
    void Start()
    {
        HPpercent = PlayerPrefs.GetInt("nowHP");
        SPpercent = PlayerPrefs.GetInt("nowSP");
    }

    void Update()
    {
        //テスト
        if (Input.GetKey(KeyCode.Space))
        {
            HPpercent = PlayerPrefs.GetInt("nowHP");
            SPpercent = PlayerPrefs.GetInt("nowSP");
            Debug.Log(PlayerPrefs.GetInt("nowHP"));
            changeHPSP(HPpercent, SPpercent);
        }
    }

    public void changeHPSP(float HPpercent, float SPpercent)
    {
        HPRectTransform.sizeDelta = new Vector2(HPpercent*longPerPercent, HPRectTransform.sizeDelta.y);
        SPRectTransform.sizeDelta = new Vector2(SPpercent * longPerPercent, SPRectTransform.sizeDelta.y);
    }
}
