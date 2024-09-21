using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDmg : MonoBehaviour
{
    private bool DebugActivate;
    // Start is called before the first frame update
    void Start()
    {
        DebugActivate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && DebugActivate)
        {
            Debug.Log("DebugOff");
            GetComponentInChildren<BoxCollider>().enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.L) && !DebugActivate)
        {
            Debug.Log("DebugOn");
            GetComponentInChildren<BoxCollider>().enabled = true;
        }
    }
}
