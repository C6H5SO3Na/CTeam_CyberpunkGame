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
        //if (PlayerManager.Instance == null) // Check if the Singleton instance is available
        //{
        //    Debug.LogError("PlayerManager instance not found.");
        //}

        //else
        //{
        //    Debug.LogError("GameObject with 'Player' tag not found.");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {

        //if (other.gameObject.CompareTag("Sword") || other.transform.IsChildOf(transform.Find("Sword")))
        //{
        //    // Ignore sword collider
        //    return;
        //}

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
        PlayerManager.PlayerDamage(10);
        StartCoroutine(EndDamage(AnimTime));
    }
    private IEnumerator EndDamage(float AnimTime)
    {
        yield return new WaitForSeconds(AnimTime);
        playerController.isDamaged = false;
    }
}
