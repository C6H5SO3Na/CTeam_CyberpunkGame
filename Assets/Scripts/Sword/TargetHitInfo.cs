using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventSource { }

public class TargetHitInfo
{
    public IEventSource hitSource;
    public TargetHitInfo(IEventSource _hitSource)
    {
        hitSource = _hitSource;
    }

    
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
