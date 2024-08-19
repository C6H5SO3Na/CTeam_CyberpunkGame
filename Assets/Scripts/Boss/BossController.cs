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
    private ShootingRocket shootingRocketScript; 

    public float attackInterval = 5.0f; // Time between attacks

    private int lastAttack = -1; // Track the last attack (-1 means no previous attack)

    public GameObject bossHitBox;

    void Start()
    {
        // Get references to the attack scripts attached to this GameObject
        slidingPunchScript = GetComponent<slidingPunch>();
        bossLaserScript = GetComponent<bossLaser>();
        daiPanScript = GetComponent<DaiPan>();
        shootingRocketScript = GetComponent<ShootingRocket>(); 

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
            attackChoice = Random.Range(0, 4); 
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
                case 3: 
                    ExecuteShootingRocket();
                    break;
            }

            
            lastAttack = attackChoice;
            bossHitBox.GetComponent<MeshCollider>().enabled = true;

            
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void ExecuteSlidingPunch()
    {
        // Start the sliding punch attack
        slidingPunchScript.enabled = true;
        bossLaserScript.enabled = false;
        daiPanScript.enabled = false;
        shootingRocketScript.enabled = false; 
    }

    void ExecuteBossLaser()
    {
        // Start the laser attack
        slidingPunchScript.enabled = false;
        bossLaserScript.enabled = true;
        daiPanScript.enabled = false;
        shootingRocketScript.enabled = false; 
    }

    void ExecuteDaiPan()
    {
        // Start the DaiPan attack
        slidingPunchScript.enabled = false;
        bossLaserScript.enabled = false;
        daiPanScript.enabled = true;
        shootingRocketScript.enabled = false; 
    }

    void ExecuteShootingRocket() // New
    {
        // Start the ShootingRocket attack
        slidingPunchScript.enabled = false;
        bossLaserScript.enabled = false;
        daiPanScript.enabled = false;
        shootingRocketScript.enabled = true;
    }
}
