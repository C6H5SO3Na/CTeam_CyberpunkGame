using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossLaser : MonoBehaviour
{
    public GameObject laserPrefab; // LineRenderer�R���|�[�l���g������Prefab
    public Transform[] startpoints; // �����̃��[�U�[�̔��˓_�̔z��
    public Transform player; // �v���C���[��Transform�ւ̎Q��
    public GameObject playerobj;

    private List<GameObject> instantiatedLasers = new List<GameObject>(); // �������ꂽPrefab���i�[���郊�X�g
    private List<Vector3> shotDirections = new List<Vector3>(); // �e���[�U�[�̕������i�[���郊�X�g
    private List<float> initialPlayerXPositions = new List<float>(); // �v���C���[�̏���X�ʒu���i�[���郊�X�g
    private List<float> initialPlayerZPositions = new List<float>(); // �v���C���[�̏���Z�ʒu���i�[���郊�X�g
    public float rotationSpeed = 0.0f; // ���[�U�[�̉�]���x
    public float sweepAngle = 45f; // ���[�U�[�����E�ɃX�C�[�v����p�x�̔���
    private List<float> currentRotationAngles = new List<float>(); // �e���[�U�[�̌��݂̉�]�p�x
    private List<bool> rotatingLeft = new List<bool>(); // �e���[�U�[�����ɉ�]���Ă��邩�ǂ���

    private Coroutine attackCoroutine; // �R���[�`�����i�[

    public GameObject bossHitBox;

    // �_���[�W�Ɋ֘A����ϐ�
    public float damageRate = 0.8f; // ���[�U�[���v���C���[�ɓ��������ۂɃ_���[�W���K�p�����Ԋu
    public int laserDamage = 10; // ���[�U�[���^����_���[�W��
    private List<float> lastDamageTimes = new List<float>(); // �e���[�U�[���Ō�Ƀ_���[�W��K�p�������Ԃ�ǐՂ��郊�X�g


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerobj = GameObject.FindGameObjectWithTag("Player");
    }
    void OnEnable()
    {
        GameObject activeHitBox = GetComponentInParent<BossController>().GetActiveHitBox();

        // Ensure the correct hitbox's collider is enabled
        if (activeHitBox != null)
        {
            activeHitBox.GetComponent<CapsuleCollider>().enabled = true;
        }
        // �X�N���v�g���L�������ꂽ�ۂɃ��X�g���ď�����
        initialPlayerXPositions.Clear();
        initialPlayerZPositions.Clear();
        lastDamageTimes.Clear(); // �Ō�̃_���[�W�K�p���Ԃ̒ǐՂ��N���A

        // �e���[�U�[�̉�]���x�����������APrefab�𐶐�
        rotationSpeed = Random.Range(30.0f, 40.0f);

        for (int i = 0; i < startpoints.Length; i++)
        {
            Transform startpoint = startpoints[i];

            // �e�X�^�[�g�|�C���g�Ń��[�U�[��Prefab�𐶐�
            GameObject laserInstance = Instantiate(laserPrefab, startpoint.position, startpoint.rotation);
            instantiatedLasers.Add(laserInstance);

            // �X�N���v�g���L�������ꂽ���_�Ń����_����X�ʒu�ƃv���C���[�̌��݂�Z�ʒu��ۑ�
            initialPlayerXPositions.Add(player.position.x);
            initialPlayerZPositions.Add(player.position.z);

            // �e���[�U�[�̃X�C�[�v�p�����[�^��������
            shotDirections.Add((player.position - startpoint.position).normalized);
            currentRotationAngles.Add(0f);

            // �e���[�U�[�̍Ō�̃_���[�W�K�p���Ԃ��������i0�ŃX�^�[�g�j
            lastDamageTimes.Add(0f);

            // ���[�U�[�̃C���f�b�N�X������������ŃX�C�[�v������ݒ�
            if (i % 2 == 0)
            {
                rotatingLeft.Add(true); // �����ԍ��̃��[�U�[�͍��ɃX�C�[�v
            }
            else
            {
                rotatingLeft.Add(false); // ��ԍ��̃��[�U�[�͉E�ɃX�C�[�v
            }
        }

        attackCoroutine = StartCoroutine(LaserRoutine());
    }

    void OnDisable()
    {
        GetComponentInParent<BossController>().DestroyHitableEffect();

        GameObject activeHitBox = GetComponentInParent<BossController>().GetActiveHitBox();
        if (activeHitBox != null)
        {
            activeHitBox.GetComponent<CapsuleCollider>().enabled = false;
        }
        

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        // �������ꂽ���ׂẴ��[�U�[��j�󂵁A���X�g���N���A
        foreach (GameObject laser in instantiatedLasers)
        {
            Destroy(laser);
        }
        instantiatedLasers.Clear();
    }

    IEnumerator LaserRoutine()
    {
        while (true)
        {
            for (int i = 0; i < startpoints.Length; i++)
            {
                SweepLaserOnGround(i); // �e���˓_�Ń��[�U�[���X�C�[�v
                UpdateLaser(i); // �e���˓_�Ń��[�U�[���X�V
            }
            yield return null;
        }
    }

    void SweepLaserOnGround(int index)
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        // Adjust the current rotation angle based on direction
        if (rotatingLeft[index])
        {
            currentRotationAngles[index] -= rotationThisFrame;
            if (currentRotationAngles[index] <= -sweepAngle)
            {
                rotatingLeft[index] = false; // Switch direction
            }
        }
        else
        {
            currentRotationAngles[index] += rotationThisFrame;
            if (currentRotationAngles[index] >= sweepAngle)
            {
                rotatingLeft[index] = true; // Switch direction
            }
        }

        // Calculate a direction vector that rotates around the laser's starting point
        Vector3 startpointPosition = startpoints[index].position;
        Vector3 targetPoint = new Vector3(
            player.position.x,  // Get the player's current position
            152, // Maintain the y-position
            initialPlayerZPositions[index]
        );


        Vector3 directionToPlayer = (targetPoint - startpointPosition).normalized;
        Vector3 sweepDirection = Quaternion.Euler(0, currentRotationAngles[index], 0) * directionToPlayer;

        shotDirections[index] = sweepDirection;
    }


    void UpdateLaser(int index)
    {
        // �������ꂽ���[�U�[Prefab����LineRenderer���擾
        LineRenderer lr = instantiatedLasers[index].GetComponent<LineRenderer>();

        // ���[�U�[�̎n�_�𔭎˓_�ɐݒ�
        lr.SetPosition(0, startpoints[index].position);

        RaycastHit hit;
        if (Physics.Raycast(startpoints[index].position, shotDirections[index], out hit))
        {
            // �I�u�W�F�N�g�i�n�ʂ܂��̓v���C���[�j�ɓ��������ʒu�Ƀ��[�U�[�̏I�_��ݒ�
            lr.SetPosition(1, hit.point);

            if (hit.transform.CompareTag("Player"))
            {
                ApplyLaserDamage(index); // �v���C���[�Ƀ_���[�W��K�p
            }
        }
        else
        {
            // ������Ȃ���΁A���[�U�[���V���b�g�����ɉ����܂ŉ���
            lr.SetPosition(1, startpoints[index].position + shotDirections[index] * 5000);
        }
    }

    void ApplyLaserDamage(int index)
    {
        // �Ō�̃_���[�W�K�p����w�莞�ԁidamageRate�j���o�߂��Ă��邩���m�F
        if (Time.time >= lastDamageTimes[index] + damageRate)
        {
            //player.GetComponent<PlayerHealth>().ApplyDamage(laserDamage);

            // �e���[�U�[�̍Ō�̃_���[�W�K�p���Ԃ��X�V
            lastDamageTimes[index] = Time.time;
            PlayerManager.PlayerDamage(laserDamage);//�v���C���[�̃_���[�W
            playerobj.GetComponent<PlayerCollision>().TriggerDamage("Damage", 1.033f + 0.5f);
            Debug.Log("���[�U�[���v���C���[�ɓ�����A" + laserDamage + " �_���[�W��K�p");
            Debug.Log("Player HP now: " + PlayerManager.nowHP);
        }
    }
}
