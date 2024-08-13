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
    }

    void OnCollisionEnter(Collision _collision)
    {
        Debug.Log("OnCollisionEnter");
        ISwordTarget target = _collision.gameObject.GetComponent<ISwordTarget>();
        if (target != null)
        {
            Debug.Log("target != null");
            if (eventSource != null)
            {
                Debug.Log("eventSource != null");
                OnSwordCollisionEvent?.Invoke(eventSource, target);
            }
            else Debug.LogWarning("SwordCollider hit a SwordTarget, but there was no source - Collision will be ignored");
        }
    }
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
