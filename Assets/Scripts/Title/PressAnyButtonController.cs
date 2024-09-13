using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PressAnyButtonController : MonoBehaviour
{
    Image image;
    float alpha;
    public float changeAmount;//1�b���̕ω���
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
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
        image.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }
}
