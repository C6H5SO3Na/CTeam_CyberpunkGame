using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IEventSource //IEventSource���p������
{
    CharacterController controller;
    Animator animator;
    public Vector3 moveDirection = Vector3.zero;
    public float gravity = 8;
    public float rotateForce = 200;   //��]��
    public float runForce = 3;        //�O�i��

    Quaternion defaultCameraDir;    //�f�t�H���g�̃J��������
    Vector3 defaultCameraOffset;    //�f�t�H���g�̃J�����␳�ʒu
    float charaDir = 0;             //�L�����N�^�[�̕���

    Vector3 defaultPosition;        //���Z�b�g���̃L�����N�^�[�ʒu
    SwordComponent swordComponent;  //swordComponent�ɃA�N�Z�X�ł���悤�ɂ���

    // Start is called before the first frame update
    void Start()
    {
        // �K�v�ȃR���|�[�l���g�������擾
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        defaultCameraDir = Camera.main.transform.rotation;
        defaultCameraOffset = Camera.main.transform.position - transform.position;

        defaultPosition = transform.position;
        swordComponent = GetComponent<SwordComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null) return;  //�L�����R���g���[���[�������Ă��Ȃ��ꍇ�͏I��

        if (Input.GetKey(KeyCode.Q))        //���Z�b�g�{�^��
        {
            transform.position = defaultPosition;
        }

        //�U��
        if (Input.GetKeyDown(KeyCode.Z))
        {
            swordComponent.SetSwordActive();  //�U������swordComponent�̊֐����Ă�
            animator.SetTrigger("Hit");
        }

        //������̓��͂Ői��
        bool isRun = false; //���͂����邩�ǂ���
        if (Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)
        {
            isRun = true;
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (input.magnitude > 1.0f)
                input = input.normalized; //������1.0�ȓ��Ɏ��߂�
            moveDirection.z = input.z * runForce; //���̌��Move���\�b�h��Time.deltaTime���s��
            moveDirection.x = input.x * runForce;

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
        //Vector3 globalDirection = transform.TransformDirection(moveDirection);
        //�L�����N�^�[�̌����ɑO�i����
        Vector3 globalDirection = Quaternion.Euler(0, charaDir, 0) * moveDirection;
        controller.Move(globalDirection * Time.deltaTime);

        //�n�ʂɒ��n���Ă�����y�����ړ������Z�b�g����
        if (controller.isGrounded) moveDirection.y = 0;

        //�J�����ʒu�����݂̃L�����N�^�[�ʒu��ɐݒ肷��
        Camera.main.transform.position = transform.position + Quaternion.Euler(0, charaDir, 0) * defaultCameraOffset;

        //�����Ă��邩�ǂ����̃A�j���[�V�����ݒ�
        animator.SetBool("Run", isRun);

        //�A�j���[�V�������x�𒲐��i�A�j���[�V�������Ŕ��ʁj
        string anim_name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (anim_name.Contains("Run") && isRun)
        {
            Vector3 vel_xz = new Vector3(moveDirection.x, 0, moveDirection.z);
            animator.speed = vel_xz.magnitude / runForce;
        }
        else
        {
            animator.speed = 1.0f;
        }
    }

}
