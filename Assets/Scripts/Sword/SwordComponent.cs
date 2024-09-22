using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public interface MeleeSwordTrail{}
public class SwordComponent : MonoBehaviour
{
    public float swordActiveTime;
    int AttackType;

    SwordColliderController swordCollider;
    //MeleeSwordTrail swordTrail;

    public float swordActiveTimer;

    public List<TargetHitInfo> targetList = new List<TargetHitInfo>();

    private PlayerManager playerManager;

    private int SwordPower;

    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GetComponentInChildren<SwordColliderController>();
        //swordTrail = GetComponentInChildren<MeleeSwordTrail>();
        playerManager = GetComponentInParent<PlayerManager>();

        if (swordCollider!=null)
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

    public void SetSwordActive(int _AttactType)
    {
        swordActiveTimer = swordActiveTime;
        AttackType = _AttactType;
        if (AttackType == 1)
        {
            SwordPower = 10;
        }
        if (AttackType == 2)
        {
            SwordPower = 20;
        }
    }

    public void OnSwordCollision(IEventSource _source, ISwordTarget _target)
    {
        Debug.Log("OnSwordCollision called");

        swordActiveTime = 3f;
        if (swordActiveTimer > 0)
        {
            TargetHitInfo hitInfo = new TargetHitInfo(_source);
            if (!targetList.Contains(hitInfo))
            {
                _target.OnTargetHit(hitInfo, AttackType);
                targetList.Add(hitInfo);
                Debug.Log("Target hit processed");

                if (AttackType == 1 && playerManager != null)
                {
                    playerManager.PlayerSPAdd(20);
                    Debug.Log("SP added to player.");
                }
            }
        }
        else
        {
            targetList.Clear();
        }
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