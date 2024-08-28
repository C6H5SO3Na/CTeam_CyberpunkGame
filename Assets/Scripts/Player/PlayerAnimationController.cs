using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        animator.SetBool("Run", Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f);
    }
}
