using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIScript : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Animator enemyAnim;
    public Rigidbody rb;

    [Header("Layers")]
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Patroling")]
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public float strafeSpeed;

    [Header("Hurt")]
    public float knockbackForce;
    public Vector3 lastHitPosition;

    [Header("Health")]
    public int health = 3;

    [Header("Ranges")]
    public float sightRange, attackRange, meleeRange, projectileSpeed;
    public bool playerInSightRange, playerInAttackRange, playerInMeleeRange;
    public GameObject Firepoint;

    [Header("Cooldowns")]
    public bool canRangeattack = true;

    [Header("State Flags")]
    public bool meDead;
    public bool isRunning;
    public bool isWalking;
    public bool isAttackingmelee;
    public bool isAttackingranged;
    public bool isHurt;

    [Header("Attack Configurations")]
    public bool canMeleeAttack;
    public bool canRangedAttack;

    public List<GameObject> patrolPoints;

    private IAIState currentState;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ChangeState(new IdleState());
    }

    private void Update()
    {
        if (health <= 0f && !(currentState is DeathState))
        {
            ChangeState(new DeathState());
        }
        else
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
            playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, whatIsPlayer);
            currentState.Execute(this);
        }


        enemyAnim.SetBool("isRunning", isRunning);
        enemyAnim.SetBool("isWalking", isWalking);
        enemyAnim.SetBool("meDead", meDead);
        enemyAnim.SetBool("isAttackingmelee", isAttackingmelee);
        enemyAnim.SetBool("isAttackingranged", isAttackingranged);
    }

    public void ChangeState(IAIState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }

    public void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    public void ResetRangeCD()
    {
        canRangeattack = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) ChangeState(new DeathState());
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public void GetHurt(Vector3 hitPosition)
    {
        if (!meDead && !isHurt)
        {
            lastHitPosition = hitPosition;
            health--;
            ChangeState(new HurtState());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 hitPosition = collision.contacts[0].point;
            Debug.Log("Collision detected with PlayerWeapon at position: " + hitPosition);
            GetHurt(hitPosition);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}