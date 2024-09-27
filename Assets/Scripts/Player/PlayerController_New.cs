using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

public class PlayerController_New : MonoBehaviour, IEventSource //IEventSource���p������
{
    Animator animator;
    public LayerMask floorLayer;
    public LayerMask stairLayer;
    public Vector3 moveDirection = Vector3.zero;
    public GameObject raycastOj;

    public float gravity = 8f;
    public float rotateForce = 200f;   //��]��
    public float runForce = 3f;        //�O�i��
    public float moveSpeed = 5f;
    //public float jumpForce = 7f;
    public float slopeLimit = 45f;
    public float staircaseHeight = 0.5f;

    Quaternion defaultCameraDir;    //�f�t�H���g�̃J��������
    Vector3 defaultCameraOffset;    //�f�t�H���g�̃J�����␳�ʒu
    Vector2 playerDirection;             //�L�����N�^�[�̕���

    Vector3 defaultPosition;        //���Z�b�g���̃L�����N�^�[�ʒu
    SwordComponent swordComponent;  //swordComponent�ɃA�N�Z�X�ł���悤�ɂ���
    [SerializeField] GameManager manager;   //GameManager

    private Rigidbody RigidBd;
    private Collider Colli;
    private bool isFloor;
    private bool isRun;
    private bool isAttacking;
    public bool isDamaged;
    public PlayerManager playerManager;
    private int deadTime;

    bool isAbleAttackR2 = false;//R2�{�^���p

    SoundGenerator soundGenerator;
    //private float AnimTime;

    // Start is called before the first frame update
    void Start()
    {
        //var meshList = GetComponentsInChildren<MeshRenderer>().ToList();
        //var materials = new List<Material>();
        //foreach (var mesh in meshList)
        //{
        //    materials.AddRange(mesh.materials);
        //}
        //foreach(var material in materials)
        //{
        //    material.color = new Color(1, 1, 1, 0.5f);
        //}

        Colli = GetComponent<Collider>();
        RigidBd = GetComponent<Rigidbody>();
        RigidBd.freezeRotation = true; //�v���C���[�̉�]�h�~
        animator = GetComponent<Animator>();
        defaultCameraDir = Camera.main.transform.rotation;
        defaultCameraOffset = Camera.main.transform.position - transform.position;
        //defaultPosition = transform.position;
        swordComponent = GetComponent<SwordComponent>();
        isAttacking = false;
        isDamaged = false;
        playerManager = GetComponent<PlayerManager>();
        soundGenerator = GameObject.Find("SoundManager").GetComponent<SoundGenerator>();
        deadTime = 0;
        playerDirection = new Vector2(transform.localEulerAngles.y, transform.localEulerAngles.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking /*&& !isDamaged*/)
        {
            PlayerInput();
            PlayerMovement();
            RotateCamera();
        }
        CheckFloor();
        PlayerSlopeMovement();
        SetAnimationSpeed();
        if (PlayerManager.PlayerisDead == true)
        {
            GameOver(deadTime);
            deadTime++;
        }
        //�J�����ʒu�����݂̃L�����N�^�[�ʒu��ɐݒ肷��
        Camera.main.transform.position = transform.position + Quaternion.Euler(0, playerDirection.x, 0) * defaultCameraOffset;
    }
    private void PlayerInput()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * moveSpeed;

        //if (Input.GetKey(KeyCode.Q))        //���Z�b�g�{�^��
        //{
        //    transform.position = defaultPosition;
        //}

        //���ʍU��
        if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Normal_Attack") || (Input.GetAxis("Normal_AttackRT") > 0.0f && isAbleAttackR2))
        {
            isAbleAttackR2 = false;//R2�̂�

            swordComponent.SetSwordActive(1);
            isAttacking = true;
            soundGenerator.GenerateSoundByID("901");
            TriggerAttack(1, "Attack", 1.417f);
            //playerManager.PlayerSPAdd(1);
        }

        //R2�𗣂��Ȃ��Ǝ��̒ʏ�U���͂ł��Ȃ�
        if (Input.GetAxis("Normal_AttackRT") <= 0.0f)
        {
            isAbleAttackR2 = true;
        }
        //��Z
        if (PlayerManager.nowSP == 100)
        {
            if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Special_Attack"))
            {
                swordComponent.SetSwordActive(2);
                isAttacking = true;
                soundGenerator.GenerateSoundByID("904");
                TriggerAttack(2, "Attack2", 2.133f);
                PlayerManager.PlayerSPReset();
            }
        }

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    animator.SetTrigger("Damage");
        //    playerManager.PlayerDamage(1);
        //}
        //}

        if (Input.GetAxis("Vertical_R") != 0.0f || Input.GetAxis("Horizontal_R") != 0.0f)
        {
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
            float normalizedDir = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, playerDirection.x + normalizedDir, 0.0f);
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
        Vector3 globalDirection = Quaternion.Euler(0, playerDirection.x, 0) * moveDirection;
        RigidBd.MovePosition(RigidBd.position + globalDirection * Time.deltaTime);

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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3.0f, floorLayer))
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
        if (Physics.Raycast(raycastOj.transform.position, Vector3.forward, out hit, Mathf.Infinity, stairLayer))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            //Debug.Log($"hitstair: {angle}, {angle > 0 && angle <= slopeLimit}");
            return angle > 0 && angle <= slopeLimit;
        }
        else
        {
            Debug.DrawRay(raycastOj.transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
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
    private void TriggerAttack(int attackingNum, string attackTrigger, float AnimTime)
    {
        isAttacking = true;
        animator.SetTrigger(attackTrigger);
        StartCoroutine(EndAttack(attackingNum, AnimTime));
    }
    private IEnumerator EndAttack(int attackingNum, float AnimTime)
    {
        yield return new WaitForSeconds(AnimTime);
        isAttacking = false;
        if (!isAttacking)
        {
            if (attackingNum == 1)
            {
                soundGenerator.DeleteSoundByID("901");
            }
            if (attackingNum == 2)
            {
                soundGenerator.DeleteSoundByID("904");
            }
        }
    }

    private void RotateCamera()
    {
        //�v���C���̎��_�ύX
        playerDirection.x += Input.GetAxis("Horizontal_R");

        //�J��������]
        Camera.main.transform.rotation = Quaternion.Euler(playerDirection.y, playerDirection.x, 0.0f) * defaultCameraDir;
        Camera.main.transform.position = transform.position + Quaternion.Euler(playerDirection.y, playerDirection.x, 0.0f) * defaultCameraOffset;
    }

    public void GameOver(int time)
    {
        if (time == 0)
        {
            soundGenerator.GenerateSoundByID("902");
            StartCoroutine(DeadSEEnd(1f));
            Debug.Log("DeadSEPlay");
        }

        //if (PlayerManager.PlayerisDead == true)
        //{
        //    soundGenerator.GenerateSoundByID("902");
        //    StartCoroutine(DeadSEEnd(1f));
        //    Debug.Log("DeadSEPlay");
        //}
    }

    private IEnumerator DeadSEEnd(float Time)
    {
        yield return new WaitForSeconds(Time);
        soundGenerator.DeleteSoundByID("902");
        Debug.Log("DeadSEEnd");
        manager.isDead = true;
    }
}
