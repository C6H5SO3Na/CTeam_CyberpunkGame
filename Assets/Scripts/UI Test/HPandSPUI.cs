using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPandSPUI : MonoBehaviour
{
    public RectTransform HPRectTransform;
    public RectTransform SPRectTransform;
    public float longPerPercent = 7f; //1%ごとにUIの変更量
    public float HPpercent;
    public float SPpercent;
    
    // Start is called before the first frame update
    void Start()
    {
        HPpercent = 80;
        SPpercent = 60;
    }

    void Update()
    {
        //テスト
        if (Input.GetKey(KeyCode.Space))
        {
            changeHPSP(HPpercent, SPpercent);
        }
    }

    public void changeHPSP(float HPpercent, float SPpercent)
    {
        HPRectTransform.sizeDelta = new Vector2(HPpercent*longPerPercent, HPRectTransform.sizeDelta.y);
        SPRectTransform.sizeDelta = new Vector2(SPpercent * longPerPercent, SPRectTransform.sizeDelta.y);
    }
}
