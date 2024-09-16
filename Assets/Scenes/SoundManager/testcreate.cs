using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcreate : MonoBehaviour
{
    public UIManager uiManager;
    public SoundGenerator soundGanerator;
    // Start is called before the first frame update
    int i = 0;
    void Start()
    {
        uiManager.GenerateUIByID("101");
        soundGanerator.GenerateSoundByID("201");
        soundGanerator.GenerateSoundByID("202");
        soundGanerator.GenerateSoundByID("203");
    }

    // Update is called once per frame
    void Update()
    {
        i += 1;
        if (i > 120)
        {
            soundGanerator.DeleteSoundByID("202");
        }
    }
}
