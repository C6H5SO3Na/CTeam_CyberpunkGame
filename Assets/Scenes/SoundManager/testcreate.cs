using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcreate : MonoBehaviour
{
    public UIManager uiManager;
    public SoundGanerator soundGanerator;
    // Start is called before the first frame update
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
        
    }
}
