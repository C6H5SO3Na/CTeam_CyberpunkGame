using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isRun;
        animator.SetBool("Run", Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f);
        isRun = animator.GetBool("Run");

        //�����[�V�����e�X�g
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("Attack2");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Damage");
        }

        //�A�j���[�V�������x�𒲐��i�A�j���[�V�������Ŕ��ʁj
        string anim_name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (anim_name.Contains("Run") && isRun)
        {

        }
        else
        {
            animator.speed = 1.0f;
        }
    }
}
