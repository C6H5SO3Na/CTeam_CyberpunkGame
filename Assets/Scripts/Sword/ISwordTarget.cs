using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwordTarget 
{ 
    void OnTargetHit(TargetHitInfo _info,int _AttackType); 
}

//public class ISwordTarget : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
