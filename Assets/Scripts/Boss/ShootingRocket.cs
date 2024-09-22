using System.Collections;
using UnityEngine;

public class ShootingRocket : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform[] launchPoints; // ���P�b�g�𔭎˂���|�C���g�̔z��
    public float heightAbovePlayer = 30.0f; // �v���C���[�̏�Ƀ��P�b�g���J�n���鍂��
    public float spreadRadius = 5.0f; // �v���C���[����Ƀ��P�b�g���L���锼�a
    public float ascentSpeed = 20.0f; // ���P�b�g���㏸���鑬�x
    public float descentDelay = 1.0f; // ���P�b�g�����_�ɓ��B������A���~���J�n����܂ł̒x������
    public float rocketLifetime = 10.0f; // ���P�b�g�̎����i�j�󂳂��܂ł̎��ԁj
    public float rotationSpeed = 360.0f; // ���P�b�g���������ɉ�]���鑬�x

    private Coroutine attackCoroutine;

    public GameObject bossHitBox;

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

        for (int i = 0; i < launchPoints.Length; i++)
        {
            // �v���C���[�̎���Ń����_���ȃI�t�Z�b�g�ʒu���v�Z���A���P�b�g���g�U������
            Vector3 randomOffset = new Vector3(
                Random.Range(-spreadRadius, spreadRadius),
                0,
                Random.Range(-spreadRadius, spreadRadius)
            );

            // �ŏ��̔��˃|�C���g�̏ꍇ�́A�I�t�Z�b�g���[���ɐݒ肷��
            if (i == 0)
            {
                randomOffset = Vector3.zero;
            }

            Vector3 targetPosition = player.transform.position + randomOffset;

            // ���˃|�C���g�Ń��P�b�g�𐶐����AX����-90�x�ɐݒ肵�Đ�������
            GameObject rocketInstance = Instantiate(rocketPrefab, launchPoints[i].position, Quaternion.Euler(-90, 0, 0));

            // �Փˏ����p��RocketCollisionHandler�X�N���v�g��ǉ�
            RocketCollisionHandler collisionHandler = rocketInstance.AddComponent<RocketCollisionHandler>();
            collisionHandler.SetDestroyOnCollision();

            // ���P�b�g������Ɉړ������A�ڕW�ʒu�̏�ɔz�u����
            StartCoroutine(MoveRocketUpwardsAndRotate(rocketInstance, targetPosition));

            // ���P�b�g�̔j��͂����ɂ͍s�킸�A��ŏ�������
        }

        // �U�����I��������A�X�N���v�g�𖳌��ɂ���
        yield return new WaitForSeconds(10.0f);
        this.enabled = false;
    }

    IEnumerator MoveRocketUpwardsAndRotate(GameObject rocket, Vector3 targetPosition)
    {
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        rb.useGravity = false;

        MeshRenderer rocketRenderer = rocket.GetComponentInChildren<MeshRenderer>();
        if (rocketRenderer != null)
        {
            // ���P�b�g�����˂���A�㏸���̓V���h�E�𖳌��ɂ���
            rocketRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        Vector3 peakPosition = targetPosition + Vector3.up * heightAbovePlayer;

        // ���P�b�g�𒸓_�ʒu�Ɍ������ď㏸������
        while (rocket != null && Vector3.Distance(rocket.transform.position, peakPosition) > 0.1f)
        {
            Vector3 moveDirection = (peakPosition - rocket.transform.position).normalized;
            rocket.transform.position += moveDirection * ascentSpeed * Time.deltaTime;

            RotateRocketTowardsDirection(rocket, moveDirection);

            yield return null;
        }

        // ���~���J�n����O�ɏ����҂�
        yield return new WaitForSeconds(descentDelay);

        // ���P�b�g���������ɉ�]������
        if (rocket != null)
        {
            yield return StartCoroutine(RotateRocketToFaceDown(rocket));

            // ���P�b�g����]���A���~���J�n������ɃV���h�E��L���ɂ���
            if (rocketRenderer != null)
            {
                rocketRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }

            // �d�͂�L���ɂ��ă��P�b�g�𗎉�������
            if (rocket != null)
            {
                rb = rocket.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                }

                // ���P�b�g�̎������s����܂ő҂��Ă���j�󂷂�
                yield return new WaitForSeconds(rocketLifetime - descentDelay);
                if (rocket != null)
                {
                    Destroy(rocket);
                }
            }
        }
    }


    IEnumerator RotateRocketToFaceDown(GameObject rocket)
    {
        Vector3 downwardDirection = Vector3.down;

        // ���P�b�g���������ɂȂ�܂ŉ�]�𑱂���
        while (rocket != null && Vector3.Angle(rocket.transform.forward, downwardDirection) > 1f)
        {
            // ���P�b�g�����炩�ɉ������ɉ�]������
            Quaternion targetRotation = Quaternion.LookRotation(downwardDirection);
            rocket.transform.rotation = Quaternion.RotateTowards(rocket.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null; // ���̃t���[���܂ő҂�
        }
    }

    void RotateRocketTowardsDirection(GameObject rocket, Vector3 direction)
    {
        // ���P�b�g���ړ������ɍ��킹�邽�߂̉�]���v�Z����
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rocket.transform.rotation = Quaternion.RotateTowards(rocket.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

// ���̃X�N���v�g�̓��P�b�g�̏Փ˂��������܂�
public class RocketCollisionHandler : MonoBehaviour
{
    private bool isDestroyed = false;

    private PlayerManager playerManager; //�v���C���[��HP���

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //�v���C���[��T��
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();

            if (playerManager == null)
            {
                Debug.LogError("Player component not found on GameObject with 'Player' tag.");
            }
        }
        else
        {
            Debug.LogError("GameObject with 'Player' tag not found.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���P�b�g���n�ʂ܂��̓v���C���[�ɏՓ˂����ꍇ
        if (!isDestroyed && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground")))
        {
           if(collision.gameObject.CompareTag("Player"))
           {
                playerManager.PlayerDamage(10);//�v���C���[�̃_���[�W
                Debug.Log("�v���C���[���q�b�g10�_���[�W");
                Debug.Log("Player HP now: " + playerManager.nowHP);
            }
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public void SetDestroyOnCollision()
    {
        // ���P�b�g�ɃR���C�_�[������AOnTriggerEnter���g�p����ꍇ�̓g���K�[�Ƃ��Đݒ肳��Ă��邱�Ƃ��m�F����
        Collider collider = GetComponent<CapsuleCollider>();
    }
}
