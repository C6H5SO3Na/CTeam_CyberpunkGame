using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Image image;
    [SerializeField] float changeAmount;//1•b–ˆ‚Ì•Ï‰»—Ê

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
                }
                break;

            case State.Fadeout:
                image.color += new Color(0.0f, 0.0f, 0.0f, changeAmount * Time.deltaTime);

                if (image.color.a >= 1.0f)
                {
                    image.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                    state = State.Idle;
                }
                break;
        }
        Debug.Log(image.color);
    }

    public void Fadein()
    {
        if (state == State.Fadein) { return; }
        state = State.Fadein;
    }
    public void Fadeout()
    {
        if (state == State.Fadeout) { return; }
        state = State.Fadeout;
    }
    public bool IsFade()
    {
        return state != State.Idle;
    }
}
