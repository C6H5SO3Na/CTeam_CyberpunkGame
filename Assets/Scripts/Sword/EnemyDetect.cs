using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour, ISwordTarget
{
    public void OnTargetHit(TargetHitInfo _info)
    {
        Debug.Log("Ouch");
        return;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
