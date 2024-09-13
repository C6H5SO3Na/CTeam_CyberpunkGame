using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class Fade : MonoBehaviour
{
    Image image;
    [SerializeField] float changeAmount;//1•b–ˆ‚Ì•Ï‰»—Ê
    bool isFade = false;

    public enum State
    {
        Idle, Fadein, Fadeout
    }
    State state;
    void Start()
    {
        image = GetComponent<Image>();
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:

                break;

            case State.Fadein:
                image.color += new Color(0.0f, 0.0f, 0.0f, -changeAmount * Time.deltaTime);

                if (image.color.a <= 0.0f)
                {
                    image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    state = State.Idle;
                    isFade = false;
                }
                break;

            case State.Fadeout:
                image.color += new Color(0.0f, 0.0f, 0.0f, changeAmount * Time.deltaTime);

                if (image.color.a >= 1.0f)
                {
                    image.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    state = State.Idle;
                    isFade = false;
                }
                break;
        }
    }

    public void Fadein()
    {
        if (image.color == new Color(0.0f, 0.0f, 0.0f, 0.0f)) { return; }
        state = State.Fadein;
        isFade = true;
    }
    public void Fadeout()
    {
        if (image.color == new Color(0.0f, 0.0f, 0.0f, 1.0f)) { return; }
        state = State.Fadeout;
        isFade = true;
    }
    public bool IsFade()
    {
        return isFade;
    }
}
