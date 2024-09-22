using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerController_New : MonoBehaviour, IEventSource //IEventSource���p������
{
    Animator animator;
    public LayerMask floorLayer;
    public Vector3 moveDirection = Vector3.zero;

    public float gravity = 8f;
    public float rotateForce = 200f;   //��]��
    public float runForce = 3f;        //�O�i��
    public float moveSpeed = 5f;
    //public float jumpForce = 7f;
    public float slopeLimit = 45f;
    public float staircaseHeight = 0.5f;

    Quaternion defaultCameraDir;    //�f�t�H���g�̃J��������
    Vector3 defaultCameraOffset;    //�f�t�H���g�̃J�����␳�ʒu
    float charaDir = 0;             //�L�����N�^�[�̕���

    Vector3 defaultPosition;        //���Z�b�g���̃L�����N�^�[�ʒu
    SwordComponent swordComponent;  //swordComponent�ɃA�N�Z�X�ł���悤�ɂ���

    private Rigidbody RigidBd;
    private Collider Colli;
    private bool isFloor;
    private bool isRun;
    private bool isAttacking;
    private PlayerManager playerManager;
    //private float AnimTime;

    // Start is called before the first frame update
    void Start()
    {
        Colli = GetComponent<Collider>();
        RigidBd = GetComponent<Rigidbody>();
        RigidBd.freezeRotation = true; //�v���C���[�̉�]�h�~
        animator = GetComponent<Animator>();
        defaultCameraDir = Camera.main.transform.rotation;
        defaultCameraOffset = Camera.main.transform.position - transform.position;
        defaultPosition = transform.position;
        swordComponent = GetComponent<SwordComponent>();
        isAttacking = false;
        playerManager = GetComponent<PlayerManager>();
}

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            PlayerInput();
            PlayerMovement();
        }
        CheckFloor();
        PlayerSlopeMovement();
        SetAnimationSpeed();
    }
    private void PlayerInput()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * moveSpeed;

        //if (Input.GetKey(KeyCode.Q))        //���Z�b�g�{�^��
        //{
        //    transform.position = defaultPosition;
        //}

        //���ʍU��
        if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Normal_Attack"))
        {
            swordComponent.SetSwordActive(1);
            isAttacking = true;
            TriggerAttack("Attack", 1.417f);
            
            //playerManager.PlayerSPAdd(1);
        }

        //��Z
        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Special_Attack"))
        {
            swordComponent.SetSwordActive(2);
            isAttacking = true;
            TriggerAttack("Attack2", 2.133f);
            playerManager.PlayerSPReset();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Damage");
           // playerManager.PlayerDamage(1);
        }

    }

    private void PlayerMovement()
    {
        //������̓��͂Ői��
        isRun = false; //���͂����邩�ǂ���
        if (Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)
        {
            isRun = true;
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (input.magnitude > 1.0f)
                input = input.normalized; //������1.0�ȓ��Ɏ��߂�
            //moveDirection.z = input.z * runForce; //���̌��Move���\�b�h��Time.deltaTime���s��
            //moveDirection.x = input.x * runForce;
            moveDirection = input * moveSpeed;

            //�J�����̌�������ɃL�����̌�����ς���
            float Dir = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, charaDir + Dir, 0);
        }
        else
        {
            moveDirection.z = 0;
            moveDirection.x = 0;
        }

        //�d�͂��v�Z����
        moveDirection.y -= gravity * Time.deltaTime;

        //�ړ����s��
        //�L�����N�^�[�̌����ɑO�i����
        Vector3 globalDirection = Quaternion.Euler(0, charaDir, 0) * moveDirection;
        RigidBd.MovePosition(RigidBd.position + globalDirection * Time.deltaTime);

        //�J�����ʒu�����݂̃L�����N�^�[�ʒu��ɐݒ肷��
        //Camera.main.transform.position = transform.position + Quaternion.Euler(0, charaDir, 0) * defaultCameraOffset;

        //�����Ă��邩�ǂ����̃A�j���[�V�����ݒ�
        animator.SetBool("Run", isRun);
    }
    private void PlayerSlopeMovement()
    {
        //�K�i��Ζʂ�o��
        if (isFloor)
        {
            Vector3 moveVelocity = moveDirection;

            if (OnSlope()) // Handle slope movement
            {
                moveVelocity = Vector3.ProjectOnPlane(moveDirection, GetSlopeNormal());
            }

            RigidBd.velocity = new Vector3(moveVelocity.x, RigidBd.velocity.y, moveVelocity.z);
        }

        // �d�͌v�Z
        if (!isFloor)
        {
            RigidBd.velocity += Vector3.down * gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = 0;
        }
    }

    private void SetAnimationSpeed()
    {
        //�A�j���[�V�������x�𒲐��i�A�j���[�V�������Ŕ��ʁj
        string anim_name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (anim_name.Contains("Run") && isRun)
        {
            Vector3 vel_xz = new Vector3(moveDirection.x, 0, moveDirection.z);
            animator.speed = vel_xz.magnitude / runForce * 0.7f;
        }
        else
        {
            animator.speed = 1.0f;
        }
    }
    private void CheckFloor()
    {
        //�n�ʃ`�F�b�N
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, floorLayer))
        {
            isFloor = true;
        }
        else
        {
            isFloor = false;
        }
    }
    //�Ζʃ`�F�b�N
    private bool OnSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > 0 && angle <= slopeLimit;
        }
        return false;
    }

    //slope normal�`�F�b�N
    private Vector3 GetSlopeNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            return hit.normal;
        }
        return Vector3.up;
    }
    private void TriggerAttack(string attackTrigger, float AnimTime)
    {
        isAttacking = true;
        animator.SetTrigger(attackTrigger);
        StartCoroutine(EndAttack(AnimTime));
    }
    private IEnumerator EndAttack(float AnimTime)
    {
        yield return new WaitForSeconds(AnimTime);
        isAttacking = false;
    }
}
