using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0; // 時間を止める
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1; // 時間を再開する
        }
    }
}
