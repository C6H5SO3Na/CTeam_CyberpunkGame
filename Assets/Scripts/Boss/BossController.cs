using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BossController : MonoBehaviour
{
    [SerializeField]
    public int HP = 10;

    // References to the attack scripts
    //private slidingPunch slidingPunchScript;
    private bossLaser bossLaserScript;
    //private DaiPan daiPanScript;
    private ShootingRocket shootingRocketScript; 

    public float attackInterval = 5.0f; // Time between attacks

    private int lastAttack = -1; // Track the last attack (-1 means no previous attack)

    public GameObject bossHitBox;

    public GameObject Explosion;

    public Text HpText;

    private bool gameClear;

    void Start()
    {
        // Get references to the attack scripts attached to this GameObject
        //slidingPunchScript = GetComponent<slidingPunch>();
        //daiPanScript = GetComponent<DaiPan>();
        bossLaserScript = GetComponent<bossLaser>();
        shootingRocketScript = GetComponent<ShootingRocket>(); 

        // Start the attack pattern cycle
        StartCoroutine(AttackPatternCycle());
        gameClear = false;
    }

    private void Update()
    {
        HpText.text = "BossHP : " + HP;
        if(HP <= 0 && gameClear == false) //&&GameManager.isClear == false)
        {
            Vector3 spawnpos = new Vector3(bossHitBox.transform.position.x, bossHitBox.transform.position.y + 5.0f, bossHitBox.transform.position.z - 10.0f);
            GameObject explosion = Instantiate(Explosion, spawnpos, bossHitBox.transform.rotation);
            explosion.transform.SetParent(gameObject.transform);
            Invoke("DestroyObject", 5.0f);
            gameClear = true;
            //GameManager.isClear = true;
        }
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    IEnumerator AttackPatternCycle()
    {
        yield return new WaitForSeconds(attackInterval);
        while (!gameClear)
        {
            int attackChoice;
            do
            {
            attackChoice = Random.Range(0, 2); 
            } while (attackChoice == lastAttack);

            // Execute the chosen attack
            switch (attackChoice)
            {
                case 0:
                    ExecuteShootingRocket();
                    break;
                case 1:
                    ExecuteBossLaser();
                    break;
                case 2:
                    ExecuteDaiPan();
                    break;
                case 3:
                    ExecuteSlidingPunch();
                    break;
                default:
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
        //slidingPunchScript.enabled = true;
        bossLaserScript.enabled = false;
        //daiPanScript.enabled = false;
        shootingRocketScript.enabled = false; 
    }

    void ExecuteBossLaser()
    {
        // Start the laser attack
       //slidingPunchScript.enabled = false;
        bossLaserScript.enabled = true;
        //daiPanScript.enabled = false;
        shootingRocketScript.enabled = false; 
    }

    void ExecuteDaiPan()
    {
        // Start the DaiPan attack
        //slidingPunchScript.enabled = false;
        bossLaserScript.enabled = false;
        //daiPanScript.enabled = true;
        shootingRocketScript.enabled = false; 
    }

    void ExecuteShootingRocket() // New
    {
        // Start the ShootingRocket attack
        //slidingPunchScript.enabled = false;
        bossLaserScript.enabled = false;
        //daiPanScript.enabled = false;
        shootingRocketScript.enabled = true;
    }
}
