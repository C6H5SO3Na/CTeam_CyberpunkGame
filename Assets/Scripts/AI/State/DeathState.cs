using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathState : IAIState
{
    private AIScript ai;

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.meDead = true;
        ai.agent.isStopped = true;
        ai.isAttackingmelee = false;
        ai.isRunning = false;
        ai.isWalking = false;

        ai.StartCoroutine(ai.FadeOutAndDestroy());
        DeathExplosion(); // 爆発エフェクトを生成
    }

    // 爆発をAIの子として生成する
    private void DeathExplosion()
    {
        ai.audioSource.PlayOneShot(ai.deathSound);
        // 爆発エフェクトをAIの位置と回転に基づいて生成し、AIの子オブジェクトとして設定する
        GameObject explosionInstance = Object.Instantiate(ai.explosionEffect, ai.transform.position, ai.transform.rotation);
        explosionInstance.transform.SetParent(ai.transform); // AIの子オブジェクトとして設定する
    }

    public void Execute(AIScript ai)
    {
        
    }

    public void Exit(AIScript ai)
    {
    }
}
