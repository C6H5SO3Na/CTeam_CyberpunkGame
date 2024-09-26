using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHPUI : MonoBehaviour
{
    public RectTransform HPRectTransform;
    float longPerPercent = 3.5f; //1%Ç≤Ç∆Ç…UIÇÃïœçXó 
    int HPpercent;
    [SerializeField]
    BossController bossController;
    // Start is called before the first frame update
    void Start()
    {
        HPpercent = 200;
    }

    // Update is called once per frame
    void Update()
    {
        HPpercent = bossController.HPMiddle * 2;
        if (HPpercent < 0 ) { HPpercent = 0; }
        changeHP(HPpercent);
    }
    public void changeHP(float HPpercent)
    {
        HPRectTransform.sizeDelta = new Vector2(HPpercent * longPerPercent, HPRectTransform.sizeDelta.y);
    }
}
