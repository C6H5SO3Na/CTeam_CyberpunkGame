using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PressButtonController : MonoBehaviour
{
    TextMeshProUGUI text;//‰¼
    //Image image;–{”Ô—p
    float alpha;
    public float changeAmount;//1•b–ˆ‚Ì•Ï‰»—Ê
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        //image = GetComponent<Image>();
        alpha = 1.0f;
        changeAmount = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        alpha -= changeAmount * Time.deltaTime;
        if (alpha <= 0.0f || alpha >= 1.0f)
        {
            alpha = Mathf.Clamp01(alpha);
            changeAmount = -changeAmount;
        }
        //image.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        text.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }
}
