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
        DeathExplosion(); // �����G�t�F�N�g�𐶐�
    }

    // ������AI�̎q�Ƃ��Đ�������
    private void DeathExplosion()
    {
        ai.audioSource.PlayOneShot(ai.deathSound);
        // �����G�t�F�N�g��AI�̈ʒu�Ɖ�]�Ɋ�Â��Đ������AAI�̎q�I�u�W�F�N�g�Ƃ��Đݒ肷��
        GameObject explosionInstance = Object.Instantiate(ai.explosionEffect, ai.transform.position, ai.transform.rotation);
        explosionInstance.transform.SetParent(ai.transform); // AI�̎q�I�u�W�F�N�g�Ƃ��Đݒ肷��
    }

    public void Execute(AIScript ai)
    {
        
    }

    public void Exit(AIScript ai)
    {
    }
}
