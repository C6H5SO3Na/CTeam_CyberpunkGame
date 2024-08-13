using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventSource { }

public class TargetHitInfo : MonoBehaviour
{
    public TargetHitInfo(IEventSource _hitSource)
    {
        hitSource = _hitSource;
    }

    public IEventSource hitSource;
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
