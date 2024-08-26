using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IEventSource //IEventSourceを継承する
{
    CharacterController controller;
    Animator animator;
    public Vector3 moveDirection = Vector3.zero;
    public float gravity = 8;
    public float rotateForce = 200;   //回転量
    public float runForce = 3;        //前進量

    Quaternion defaultCameraDir;    //デフォルトのカメラ方向
    Vector3 defaultCameraOffset;    //デフォルトのカメラ補正位置
    float charaDir = 0;             //キャラクターの方向

    Vector3 defaultPosition;        //リセット時のキャラクター位置
    SwordComponent swordComponent;  //swordComponentにアクセスできるようにする

    // Start is called before the first frame update
    void Start()
    {
        // 必要なコンポーネントを自動取得
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
        if (controller == null) return;  //キャラコントローラーが入っていない場合は終了

        if (Input.GetKey(KeyCode.Q))        //リセットボタン
        {
            transform.position = defaultPosition;
        }

        //攻撃
        if (Input.GetKeyDown(KeyCode.Z))
        {
            swordComponent.SetSwordActive();  //攻撃時にswordComponentの関数を呼ぶ
            animator.SetTrigger("Hit");
        }

        //上方向の入力で進む
        bool isRun = false; //入力があるかどうか
        if (Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f)
        {
            isRun = true;
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (input.magnitude > 1.0f)
                input = input.normalized; //長さを1.0以内に収める
            moveDirection.z = input.z * runForce; //この後のMoveメソッドでTime.deltaTimeを行う
            moveDirection.x = input.x * runForce;

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
        //Vector3 globalDirection = transform.TransformDirection(moveDirection);
        //キャラクターの向きに前進する
        Vector3 globalDirection = Quaternion.Euler(0, charaDir, 0) * moveDirection;
        controller.Move(globalDirection * Time.deltaTime);

        //地面に着地していたらy方向移動をリセットする
        if (controller.isGrounded) moveDirection.y = 0;

        //カメラ位置を現在のキャラクター位置基準に設定する
        Camera.main.transform.position = transform.position + Quaternion.Euler(0, charaDir, 0) * defaultCameraOffset;

        //走っているかどうかのアニメーション設定
        animator.SetBool("Run", isRun);

        //アニメーション速度を調整（アニメーション名で判別）
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
