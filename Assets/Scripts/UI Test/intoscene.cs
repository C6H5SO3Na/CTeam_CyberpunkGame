using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class intoscene : MonoBehaviour
{
    public UIManager uiManager;
    //public SoundGenerator soundGanerator;
    // Start is called before the first frame update
    //int i = 0;
    void Start()
    {
        uiManager.GenerateUIByID("102");
        PlayerPrefs.SetInt("nowHP", 85);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
