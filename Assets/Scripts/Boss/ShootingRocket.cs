using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRocket : MonoBehaviour
{
    public GameObject rocketPrefab;
    public GameObject explosionPrefab; // �����̃v���n�u
    public float heightAbovePlayer = 30.0f; // �v���C���[�̏�Ƀ��P�b�g���J�n���鍂��
    public float spreadRadius = 7.0f; // �v���C���[����Ƀ��P�b�g���L���锼�a
    public float delayBeforeSpawn = 3.0f; // ���P�b�g���X�|�[������܂ł̒x������
    public int rocketCount = 6; // �X�|�[�����郍�P�b�g�̐�
    public float rocketLifetime = 10.0f; // ���P�b�g�̎����i�j�󂳂��܂ł̎��ԁj

    private Coroutine attackCoroutine;

    public GameObject bossHitBox;
    public List<GameObject> bossArm; // �{�X�̘r�̃��X�g

    void OnEnable()
    {
        attackCoroutine = StartCoroutine(HandleShootingRocket());
    }

    void OnDisable()
    {
        bossHitBox.GetComponent<MeshCollider>().enabled = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator HandleShootingRocket()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        List<Vector3> spawnPositions = new List<Vector3>(); // �X�|�[�������ʒu��ǐ�

        // �{�X�̘r�̃A�j���[�^�[�ɑ΂��ăg���K�[��ݒ�
        foreach (GameObject arm in bossArm)
        {
            Animator armAnimator = arm.GetComponent<Animator>();
            if (armAnimator != null)
            {
                armAnimator.SetTrigger("Shoot");
            }
        }

        // �X�|�[������܂ł̒x��
        yield return new WaitForSeconds(delayBeforeSpawn);

        for (int i = 0; i < rocketCount; i++)
        {
            Vector3 spawnPosition;
            bool validPosition = false;
            int maxAttempts = 10; // �ő厎�s��
            int attempt = 0;

            // ���P�b�g���\�����ꂽ�ʒu�ɃX�|�[������܂Ŏ��s
            do
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spreadRadius, spreadRadius),
                    0,
                    Random.Range(-spreadRadius, spreadRadius)
                );

                spawnPosition = player.transform.position + randomOffset + Vector3.up * heightAbovePlayer;
                validPosition = true;

                // �����̃��P�b�g�Ƃ̋������`�F�b�N
                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(pos, spawnPosition) < 3.0f) // �ŏ�������ݒ�i��: 3.0f�j
                    {
                        validPosition = false;
                        break;
                    }
                }

                attempt++;
            } while (!validPosition && attempt < maxAttempts);

            // �L���ȃX�|�[���ʒu��ǉ�
            spawnPositions.Add(spawnPosition);

            GameObject rocketInstance = Instantiate(rocketPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));

            // �Փˏ����p��RocketCollisionHandler�X�N���v�g��ǉ�
            RocketCollisionHandler collisionHandler = rocketInstance.AddComponent<RocketCollisionHandler>();
            collisionHandler.explosionPrefab = explosionPrefab; // �����v���n�u��ݒ�
            collisionHandler.SetDestroyOnCollision();

            // ���P�b�g�̎������s����܂ő҂��Ă���j�󂷂�
            Destroy(rocketInstance, rocketLifetime);
        }

        yield return new WaitForSeconds(10.0f);
        this.enabled = false;
    }
}



    public class RocketCollisionHandler : MonoBehaviour
{
    private bool isDestroyed = false;
    public GameObject explosionPrefab; // �����̃v���n�u

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDestroyed && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground")))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("�v���C���[���q�b�g�����_���[�W");
            }

            // �Փˈʒu���擾
            Vector3 collisionPoint = collision.contacts[0].point;
            collisionPoint.y -= 2f;

            // �����G�t�F�N�g���Փˈʒu�ɃX�|�[��
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, collisionPoint, Quaternion.identity);
            }

            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public void SetDestroyOnCollision()
    {
        Collider collider = GetComponent<CapsuleCollider>();
    }
}



