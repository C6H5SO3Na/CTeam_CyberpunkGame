using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IAIState
{
    private float strafeDirection = 1f; // �E�Ɉړ�����ꍇ��1�A���Ɉړ�����ꍇ��-1
    private float strafeChangeInterval = 1f; // �X�g���C�t�̕�����ύX����Ԋu
    private float lastStrafeChangeTime = 0f; // �Ō�ɃX�g���C�t�̕�����ύX��������

    private float angularSpeed = 30f; // ���b�̉�]���x�i�x�j

    private float currentAngle = 0f; // �v���C���[�̎���̌��݂̊p�x

    public void Enter(AIScript ai)
    {
        ai.isRunning = true;
        ai.isWalking = false;
        ai.isAttackingmelee = false;
        lastStrafeChangeTime = Time.time; // �Ō�̕����ύX���Ԃ�������
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            ai.ChangeState(new HurtState());
        }
        else if (ai.playerInMeleeRange && ai.canMeleeAttack)
        {
            ai.ChangeState(new MeleeAttackState());
        }
        else if (ai.playerInAttackRange && ai.canRangedAttack && ai.canRangeattack)
        {
            ai.ChangeState(new RangedAttackState());
        }
        else if (!ai.playerInSightRange)
        {
            ai.ChangeState(new PatrolState());
        }
        else if (!ai.canRangeattack)
        {
            MaintainRangedDistanceAndStrafe(ai);
        }
        else
        {
            ai.agent.SetDestination(ai.player.position);
        }
    }


    public void Exit(AIScript ai)
    {
        ai.isRunning = false;
    }

    private void MaintainRangedDistanceAndStrafe(AIScript ai)
    {
        float distanceToPlayer = Vector3.Distance(ai.transform.position, ai.player.position);
        float desiredDistance = ai.attackRange * 0.8f; // �]�܂��������i��j

        Vector3 directionToPlayer = (ai.player.position - ai.transform.position).normalized;
        Vector3 destination = ai.transform.position;

        // �v���C���[�̎�����~�`�Ɉړ�����
        currentAngle += angularSpeed * Time.deltaTime;
        if (currentAngle > 360f) currentAngle -= 360f;

        // �V�����ʒu���~����Ɍv�Z����
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * desiredDistance;

        if (distanceToPlayer > ai.attackRange)
        {
            // �]�܂��������ɋ߂Â�
            destination = ai.player.position;
        }
        else if (distanceToPlayer < desiredDistance)
        {
            // �]�܂���������ۂ��߂Ɍ�ނ���
            destination = ai.transform.position - directionToPlayer * (desiredDistance - distanceToPlayer);
        }

        // �X�g���C�t�̃��W�b�N
        if (Time.time - lastStrafeChangeTime > strafeChangeInterval)
        {
            strafeDirection *= -1; // �X�g���C�t�̕�����ύX
            lastStrafeChangeTime = Time.time; // �Ō�̕����ύX���Ԃ��X�V
        }

        Vector3 strafeDirectionVector = Vector3.Cross(directionToPlayer, Vector3.up) * strafeDirection;

        // �X�g���C�t���܂߂��V�����ړI�n���v�Z
        destination += strafeDirectionVector * ai.strafeSpeed * Time.deltaTime + offset;

        // NavMeshAgent�ɐV�����ړI�n��ݒ�
        ai.agent.SetDestination(destination);

        // AI���v���C���[�̕����������悤�ɂ���
        Vector3 lookDirection = ai.player.position - ai.transform.position;
        lookDirection.y = 0; // �����ʂł̉�]���ێ�����
        ai.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}
