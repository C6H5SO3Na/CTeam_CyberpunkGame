using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetect : MonoBehaviour, ISwordTarget
{
    public void OnTargetHit(TargetHitInfo _info, int _AttackType)
    {
        Debug.Log("Hit Target");
        Debug.Log(_AttackType);
        return;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�G�������Ă���󋵂ɃZ�b�g����
        GetComponent<Rigidbody>().MovePosition(transform.position + new Vector3(0, 0, -0.001f));
    }
}
