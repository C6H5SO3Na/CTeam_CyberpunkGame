using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface MeleeSwordTrail{}
public class SwordComponent : MonoBehaviour
{
    public float swordActiveTime;

    SwordColliderController swordCollider;
    //MeleeSwordTrail swordTrail;

    public float swordActiveTimer;

    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponentInChildren<SwordColliderController>();
        //swordTrail = GetComponentInChildren<MeleeSwordTrail>();

        if(swordCollider!=null)
        {
            Debug.Log("Start");
            swordCollider.OnSwordCollisionEvent += OnSwordCollision;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (swordActiveTimer > 0)
        {
            swordActiveTimer -= Time.deltaTime;
        }

        ////effect appear time
        //if(swordActiveTimer > 0)
        //{
        //    if(!swordTrail._emit)
        //    {
        //        swordTrail._emit = true;
        //    }
        //    swordActiveTimer -= Time.deltaTime;
        //}
        //else if (swordTrail._emit)
        //{
        //    swordTrail._emit = false;
        //}
    }

    public void SetSwordActive()
    {
        swordActiveTimer = swordActiveTime;
    }

    public void OnSwordCollision(IEventSource _source, ISwordTarget _target)
    {
        //if (swordActiveTimer > 0)
        //{
            TargetHitInfo hitInfo = new TargetHitInfo(_source);
            _target.OnTargetHit(hitInfo);
        //}
    }

    private void OnDestroy()
    {
        if (swordCollider != null)
        {
            swordCollider.OnSwordCollisionEvent -= OnSwordCollision;
            Debug.Log("Destroy");
        }
    }
}