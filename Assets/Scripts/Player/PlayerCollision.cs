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
        if (other.gameObject.CompareTag("Sword") || other.transform.Find("mixamorig:RightHand"))
        {
            // Ignore sword collider
            return;
        }

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyAttack"))
        {
            Debug.Log($"other: {other.gameObject.tag}");
            TriggerDamage("Damage", 1.033f + 0.5f);
                
        }

           
    }

    private void TriggerDamage(string DamageTrigger, float AnimTime)
    {
        if(!playerController.isDamaged)
        {
            animator.SetTrigger(DamageTrigger);
            PlayerManager.PlayerDamage(10);
            Debug.Log("Player's body collided with Enemy, 10 damage dealt to player.");
            playerController.isDamaged = true;
            StartCoroutine(EndDamage(AnimTime));
        }        
    }
    private IEnumerator EndDamage(float AnimTime)
    {
        yield return new WaitForSeconds(AnimTime);
        playerController.isDamaged = false;
    }
}
