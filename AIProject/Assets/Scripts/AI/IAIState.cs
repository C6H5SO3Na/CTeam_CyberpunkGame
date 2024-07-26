using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IAIState
{
    void Enter(AIScript ai);
    void Execute(AIScript ai);
    void Exit(AIScript ai);
}

