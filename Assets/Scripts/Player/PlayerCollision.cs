using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerManager playerManager;
    private PlayerController_New playerController;
    private Collider Colli;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController_New>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyAttack"))
        {
            TriggerDamage("Damage", 1.033f);
            Debug.Log("Player's body collided with Enemy, 10 damage dealt to player.");
        }
    }

    private void TriggerDamage(string DamageTrigger, float AnimTime)
    {
        playerController.isDamaged = true;
        animator.SetTrigger(DamageTrigger);
        playerManager.PlayerDamage(10);
        StartCoroutine(EndDamage(AnimTime));
    }
    private IEnumerator EndDamage(float AnimTime)
    {
        yield return new WaitForSeconds(AnimTime);
        playerController.isDamaged = false;
    }
}
