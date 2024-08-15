using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField]
    int HP = 10;

    // References to the attack scripts
    private slidingPunch slidingPunchScript;
    private bossLaser bossLaserScript;
    private DaiPan daiPanScript;

    public float attackInterval = 5.0f; // Time between attacks

    private int lastAttack = -1; // Track the last attack (-1 means no previous attack)

    public GameObject bossHitBox;

    void Start()
    {
        // Get references to the attack scripts attached to this GameObject
        slidingPunchScript = GetComponent<slidingPunch>();
        bossLaserScript = GetComponent<bossLaser>();
        daiPanScript = GetComponent<DaiPan>();

        // Start the attack pattern cycle
        StartCoroutine(AttackPatternCycle());
    }

    IEnumerator AttackPatternCycle()
    {
        while (true)
        {
            int attackChoice;
            do
            {
                // Choose a random attack pattern, different from the last one
                attackChoice = Random.Range(0, 3);
            } while (attackChoice == lastAttack);

            // Execute the chosen attack
            switch (attackChoice)
            {
                case 0:
                    ExecuteSlidingPunch();
                    break;
                case 1:
                    ExecuteBossLaser();
                    break;
                case 2:
                    ExecuteDaiPan();
                    break;
            }

            // Update lastAttack with the current choice
            lastAttack = attackChoice;
            bossHitBox.GetComponent<MeshCollider>().enabled = true;

            // Wait for the interval before switching to the next attack
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void ExecuteSlidingPunch()
    {
        // Start the sliding punch attack
        slidingPunchScript.enabled = true;
        bossLaserScript.enabled = false;
        daiPanScript.enabled = false;
    }

    void ExecuteBossLaser()
    {
        // Start the laser attack
        slidingPunchScript.enabled = false;
        bossLaserScript.enabled = true;
        daiPanScript.enabled = false;
    }

    void ExecuteDaiPan()
    {
        // Start the DaiPan attack
        slidingPunchScript.enabled = false;
        bossLaserScript.enabled = false;
        daiPanScript.enabled = true;
    }
}
