using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//public interface IEventSource { }

public delegate void OnSwordCollision(IEventSource _source, ISwordTarget _weaponTarget);

public class SwordColliderController : MonoBehaviour
{
    public event OnSwordCollision OnSwordCollisionEvent;
    IEventSource eventSource;
    // Start is called before the first frame update

    void Awake()
    {
        eventSource = GetComponentInParent<IEventSource>();
        if (eventSource == null)
        {
            Debug.LogWarning("No IEventSource found in parent. SwordColliderController will not function properly.");
        }
    }

    //void OnCollisionEnter(Collision _collision)
    //{
    //    Debug.Log("OnCollisionEnter");
    //    ISwordTarget target = _collision.gameObject.GetComponent<ISwordTarget>();
    //    if (target != null)
    //    {
    //        Debug.Log("Detected Target");
    //        if (eventSource != null)
    //        {
    //            Debug.Log("Start Collision Event");
    //            OnSwordCollisionEvent?.Invoke(eventSource, target);
    //        }
    //        else Debug.LogWarning("SwordCollider hit a SwordTarget, but there was no source - Collision will be ignored");
    //    }
    //}

    void OnTriggerEnter(Collider _trigger)
    {
        Debug.Log("OnTriggerEnter");
        ISwordTarget target = _trigger.gameObject.GetComponent<ISwordTarget>();
        if (target != null)
        {
            Debug.Log("Detected Target");
            if (eventSource != null)
            {
                OnSwordCollisionEvent?.Invoke(eventSource, target);
                Debug.Log("Collision Event Triggered");
            }
            else Debug.LogWarning("SwordCollider hit a SwordTarget, but there was no source - Collision will be ignored");
        }
    }
}
