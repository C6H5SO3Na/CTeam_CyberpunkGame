using UnityEngine;
using System.Collections;

public class RangedAttackState : IAIState
{
    private AIScript ai;
    private float projectileCooldown;
    private bool isFiringLaser = false;
    private float laserDamageTimer = 0f;

    // ���[�U�[�֘A�̕ϐ�
    private float laserDuration = 2f; // ���[�U�[�̌p������
    private float laserDamageRate = 0.5f; // ���[�U�[�̃_���[�W��^����p�x

    public RangedAttackState()
    {
        projectileCooldown = 4.0f; // �ˌ��N�[���_�E������
    }



    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.canRangeattack = false;
        ai.Invoke(nameof(ai.ResetRangeCD), projectileCooldown);
        ai.isAttackingmelee = false;
        ai.isRunning = false;
        ai.isWalking = false;
        ai.isAttackingranged = true;

        if (ai.laser != null)
        {
            ai.laser.enabled = false; // ���[�U�[�𖳌��ɂ���
        }

        ai.enemyAnim.SetTrigger("isAttack"); // �U���A�j���[�V�������J�n����
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            StopLaser(); // Stop laser firing when hurt
            ai.ChangeState(new HurtState());
        }
        else if (!ai.playerInSightRange)
        {
            StopLaser(); // Stop laser firing when the player is no longer in sight
            ai.ChangeState(new PatrolState());
        }
        else if (ai.canRangeattack && !isFiringLaser)
        {
            if (IsFacingPlayer())
            {
                FireLaser(); // �v���C���[�Ɍ����Ă���ꍇ�̂݃��[�U�[�U�����J�n
            }
            else
            {
                RotateTowardsPlayer(); // �v���C���[�̕����������悤�ɉ�]
            }
        }
    }

    public void Exit(AIScript ai)
    {
        ai.isAttackingranged = false; //�������U���̏�Ԃ��I��

        // Stop the laser if it's still firing
        if (ai.laser != null && ai.laser.enabled)
        {
            ai.laser.enabled = false; // ���[�U�[�𖳌��ɂ���
        }

        // Stop firing laser coroutine if it's running
        if (isFiringLaser)
        {
            isFiringLaser = false; // ���ˏ�Ԃ����Z�b�g
        }
    }

    private void RotateTowardsPlayer()
    {
        // �v���C���[�̈ʒu�ւ̕������擾
        Vector3 directionToPlayer = (ai.player.position - ai.transform.position).normalized;
        directionToPlayer.y = 0; // ���������݂̂̉�]�ɂ��邽��Y���𖳎�

        // ���݂̌�������v���C���[�ւ̌����ւ������Ɖ�]
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, targetRotation, Time.deltaTime * 1f); // ��]���x�𒲐��i2f�͑��x�j
    }

    private void StopLaser()
    {
        // Stop the laser and related coroutine if it's still active
        if (isFiringLaser)
        {
            isFiringLaser = false; // Stop firing laser
            if (ai.laser != null)
            {
                ai.laser.enabled = false; // Disable laser
            }
        }
    }
    public void FireLaser()
    {
        //���[�U�[�𔭎�
        if (!isFiringLaser)
        {
            ai.StartCoroutine(FireLaserCoroutine()); // ���[�U�[���˃R���[�`�����J�n
            ai.audioSource.PlayOneShot(ai.laserSound);
        }
    }
    

    private bool IsFacingPlayer()
    {
        // AI����v���C���[�ւ̕����x�N�g�����擾
        Vector3 directionToPlayer = (ai.player.position - ai.transform.position).normalized;

        // AI�̑O���x�N�g���ƃv���C���[�ւ̕����x�N�g���Ƃ̃h�b�g�ς��v�Z
        float dotProduct = Vector3.Dot(ai.transform.forward, directionToPlayer);

        // �h�b�g�ς�0.5���傫����΁AAI���v���C���[�̕����������Ă���Ɣ��f
        return dotProduct > 0.5f;
    }

    private IEnumerator FireLaserCoroutine()
    {
        isFiringLaser = true;

        if (ai.laser != null)
        {
            ai.laser.enabled = true;  // ���[�U�[��L���ɂ���i���ˎ��̂݁j
        }

        GameObject spawnedBeamSource = null;
        if (ai.beamsource != null)
        {
            // ���˃|�C���g�Ƀr�[���\�[�X�𐶐�����
            spawnedBeamSource = GameObject.Instantiate(ai.beamsource, ai.Firepoint.transform.position, ai.Firepoint.transform.rotation, ai.Firepoint.transform);

            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Play();  // ���[�U�[�̃p�[�e�B�N���G�t�F�N�g���Đ�
            }
        }

        // �v���C���[�̈ʒu����x�����v�Z����
        Vector3 playerPosition = SmoothPredictPlayerPosition();
        Vector3 laserDirection = (playerPosition - ai.Firepoint.transform.position).normalized;

        // �v�Z���ꂽ�����Ɋ�Â��ă��[�U�[�̕����Ɖ�]��ݒ�
        Quaternion targetRotation = Quaternion.LookRotation(laserDirection);
        ai.Firepoint.transform.rotation = targetRotation;

        float timer = 0f;
        while (timer < laserDuration)
        {
            // ���[�U�[�𓯂������ɔ��˂�������
            Vector3 laserEndPoint = CalculateLaserEndPoint(laserDirection);

            if (ai.laser != null)
            {
                ai.laser.SetPosition(0, ai.Firepoint.transform.position);  // ���[�U�[�̊J�n�ʒu��ݒ�
                ai.laser.SetPosition(1, laserEndPoint);  // ���[�U�[�̏I���ʒu��ݒ�
            }

            // ���[�U�[���v���C���[�Ƀq�b�g���Ă���ꍇ�A�_���[�W��^����
            if (HitPlayer(laserDirection))
            {
                laserDamageTimer += Time.deltaTime;
                if (laserDamageTimer >= laserDamageRate)
                {
                    ApplyLaserDamage(); // �_���[�W��K�p
                    laserDamageTimer = 0f;  // �_���[�W�^�C�}�[�����Z�b�g
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // ���[�U�[�𖳌��ɂ��A�U����ɃN���[���A�b�v
        if (ai.laser != null)
        {
            ai.laser.enabled = false;  // �U�����I�������烌�[�U�[�𖳌��ɂ���
        }

        // ���[�U�[�U�����I��������A�r�[���\�[�X��j�󂷂�
        if (spawnedBeamSource != null)
        {
            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Stop();  // �r�[���̃p�[�e�B�N���G�t�F�N�g���~
            }
            GameObject.Destroy(spawnedBeamSource);  // �r�[���\�[�X��j��
        }

        // ���ˏ�Ԃ����Z�b�g���A�A�C�h����Ԃɖ߂�
        isFiringLaser = false;
        ai.ChangeState(new IdleState());  // ���[�U�[�U���I����AIdleState�Ɉڍs
    }

    // ���[�U�[���v���C���[�Ƀq�b�g���Ă��邩�m�F
    private bool HitPlayer(Vector3 laserDirection)
    {
        float laserMaxDistance = 150f;
        if (Physics.Raycast(ai.Firepoint.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // �v���C���[�̈ʒu��\�����ăX���[�Y�ɕ␳
    private Vector3 SmoothPredictPlayerPosition()
    {
        Vector3 currentPosition = ai.player.position;

        float playerHeight = ai.player.GetComponent<Collider>().bounds.extents.y;
        currentPosition.y = ai.player.position.y + playerHeight;

        return currentPosition;
    }

    // ���[�U�[�̏I�_���v�Z
    private Vector3 CalculateLaserEndPoint(Vector3 laserDirection)
    {
        float laserMaxDistance = 150f;
        if (Physics.Raycast(ai.Firepoint.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return ai.player.position + Vector3.up * hit.collider.bounds.extents.y; // �v���C���[�̒��S�_��_��
            }
            else
            {
                return hit.point; // �v���C���[�ȊO�̃I�u�W�F�N�g�Ƀq�b�g�����ꍇ
            }
        }
        return ai.Firepoint.transform.position + laserDirection * laserMaxDistance; // �q�b�g���Ȃ��ꍇ�A�ő勗���܂Ń��[�U�[���΂�
    }

    // ���[�U�[�̃_���[�W��K�p
    private void ApplyLaserDamage()
    {
        PlayerManager.PlayerDamage(10);//�v���C���[�̃_���[�W
        ai.playerobj.GetComponent<PlayerCollision>().TriggerDamage("Damage", 1.033f + 0.5f);
        Debug.Log("�v���C���[�Ƀq�b�g! 10�_���[�W��K�p��...");
        Debug.Log("Player HP now: " + PlayerManager.nowHP);
    }
}
