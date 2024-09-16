using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerController_New : MonoBehaviour, IEventSource //IEventSourceを継承する
{
    Animator animator;
    public LayerMask floorLayer;
    public Vector3 moveDirection = Vector3.zero;

    public float gravity = 8f;
    public float rotateForce = 200f;   //回転量
    public float runForce = 3f;        //前進量
    public float moveSpeed = 5f;
    //public float jumpForce = 7f;
    public float slopeLimit = 45f;
    public float staircaseHeight = 0.5f;

    Quaternion defaultCameraDir;    //デフォルトのカメラ方向
    Vector3 defaultCameraOffset;    //デフォルトのカメラ補正位置
    float charaDir = 0;             //キャラクターの方向

    Vector3 defaultPosition;        //リセット時のキャラクター位置
    SwordComponent swordComponent;  //swordComponentにアクセスできるようにする

    private Rigidbody RigidBd;
    private Collider Colli;
    private bool isFloor;
    private bool isRun;
    private bool isAttacking;
    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        Colli = GetComponent<Collider>();
        RigidBd = GetComponent<Rigidbody>();
        RigidBd.freezeRotation = true; //プレイヤーの回転防止
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

        if (Input.GetKey(KeyCode.Q))        //リセットボタン
        {
            transform.position = defaultPosition;
        }

        //仮モーションテスト
        if (Input.GetKeyDown(KeyCode.X))
        {
            isAttacking = true;
            TriggerAttack1("Attack");
            playerManager.PlayerSPAdd(1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isAttacking = true;
            TriggerAttack2("Attack2");
            playerManager.PlayerSPReset();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Damage");
            playerManager.PlayerDamage(1);
        }

        //攻撃
        if (Input.GetKeyDown(KeyCode.Z))
        {
            swordComponent.SetSwordActive();  //攻撃時にswordComponentの関数を呼ぶ
            animator.SetTrigger("Hit");
        }
    }

    private void PlayerMovement()
    {
        //上方向の入力で進む
        isRun = false; //入力があるかどうか
        if (Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)
        {
            isRun = true;
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (input.magnitude > 1.0f)
                input = input.normalized; //長さを1.0以内に収める
            //moveDirection.z = input.z * runForce; //この後のMoveメソッドでTime.deltaTimeを行う
            //moveDirection.x = input.x * runForce;
            moveDirection = input * moveSpeed;

            //カメラの向きを基準にキャラの向きを変える
            float Dir = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, charaDir + Dir, 0);
        }
        else
        {
            moveDirection.z = 0;
            moveDirection.x = 0;
        }

        //重力を計算する
        moveDirection.y -= gravity * Time.deltaTime;

        //移動を行う
        //キャラクターの向きに前進する
        Vector3 globalDirection = Quaternion.Euler(0, charaDir, 0) * moveDirection;
        RigidBd.MovePosition(RigidBd.position + globalDirection * Time.deltaTime);

        //カメラ位置を現在のキャラクター位置基準に設定する
        Camera.main.transform.position = transform.position + Quaternion.Euler(0, charaDir, 0) * defaultCameraOffset;

        //走っているかどうかのアニメーション設定
        animator.SetBool("Run", isRun);
    }
    private void PlayerSlopeMovement()
    {
        //階段や斜面を登る
        if (isFloor)
        {
            Vector3 moveVelocity = moveDirection;

            if (OnSlope()) // Handle slope movement
            {
                moveVelocity = Vector3.ProjectOnPlane(moveDirection, GetSlopeNormal());
            }

            RigidBd.velocity = new Vector3(moveVelocity.x, RigidBd.velocity.y, moveVelocity.z);
        }

        // 重力計算
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
        //アニメーション速度を調整（アニメーション名で判別）
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
        //地面チェック
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
    //斜面チェック
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

    //slope normalチェック
    private Vector3 GetSlopeNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            return hit.normal;
        }
        return Vector3.up;
    }
    private void TriggerAttack1(string attackTrigger)
    {
        isAttacking = true;
        animator.SetTrigger(attackTrigger);
        StartCoroutine(EndAttack1());
    }
    private IEnumerator EndAttack1()
    {
        yield return new WaitForSeconds(1.417f);
        isAttacking = false;
    }

    private void TriggerAttack2(string attackTrigger)
    {
        isAttacking = true;
        animator.SetTrigger(attackTrigger);
        StartCoroutine(EndAttack2());
    }
    private IEnumerator EndAttack2()
    {
        yield return new WaitForSeconds(2.133f);
        isAttacking = false;
    }
}
