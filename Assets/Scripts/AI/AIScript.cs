using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AIScript : MonoBehaviour, IDamageable
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Animator enemyAnim;
    public Rigidbody rb;
    public LineRenderer laser;
    public GameObject beamsource;
    public GameObject explosionEffect;
    public GameObject playerobj;

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
    public float fadeDuration = 1.7f;

    [Header("Health")]
    public int health = 30;

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

    [Header("SoundEffect")]
    public AudioClip deathSound;
    public AudioClip laserSound;
    public AudioClip meleeSound;

    // AudioSource to play the sounds
    public AudioSource audioSource;

    public List<GameObject> patrolPoints;

    public bool hasExploded = false;


    private IAIState currentState;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerobj = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        if (GetComponent<Animator>() != null)
        {
            enemyAnim = GetComponent<Animator>();
        }
        else
        {
            enemyAnim = GetComponentInChildren<Animator>();
        }
        rb = GetComponent<Rigidbody>();
        laser = GetComponent<LineRenderer>();
        ChangeState(new IdleState());
        audioSource = GetComponent<AudioSource>();
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



    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void TriggerFireLaser()
    {
        if (currentState is RangedAttackState rangedAttackState)
        {
            rangedAttackState.FireLaser();
        }
    }

    public IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();


        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }


        foreach (Material mat in materials)
        {
            mat.SetFloat("_Surface", 1);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color baseColor = mat.GetColor("_BaseColor");
                    mat.SetColor("_BaseColor", new Color(baseColor.r, baseColor.g, baseColor.b, alpha));
                }
            }

            yield return null;
        }
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {

        health -= damage;


        if (health <= 0)
        {
            ChangeState(new DeathState());
        }
    }

    public void GetHurt(Vector3 hitPosition)
    {
        if (!meDead && !isHurt)
        {
            lastHitPosition = hitPosition;
            ChangeState(new HurtState());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that hit the AI is tagged as "Sword"
        if (other.CompareTag("Sword"))
        {
            // Try to get the SwordComponent from the other object or its parent
            SwordComponent sword = other.GetComponentInParent<SwordComponent>();

            // If we found the sword component, apply the damage
            if (sword != null)
            {
                Vector3 hitPosition = other.transform.position;  // Get the hit position
                GetHurt(hitPosition);  // Trigger the hurt animation or state

                // Use SwordPower from the SwordComponent to deal damage
                TakeDamage(sword.SwordPower);
                Debug.Log($"Received {sword.SwordPower} damage from sword.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object that collided with the AI is tagged as "Sword"
        if (collision.gameObject.CompareTag("Sword"))
        {
            // Try to get the SwordComponent from the object that collided or its parent
            SwordComponent sword = collision.gameObject.GetComponentInParent<SwordComponent>();

            // If we found the sword component, apply the damage
            if (sword != null)
            {
                Vector3 hitPosition = collision.contacts[0].point;  // Get the hit point on collision
                Debug.Log("Collision detected with PlayerWeapon at position: " + hitPosition);
                GetHurt(hitPosition);  // Trigger the hurt state

                // Use SwordPower from the SwordComponent to deal damage
                TakeDamage(sword.SwordPower);
                Debug.Log($"Received {sword.SwordPower} damage from sword.");
            }
        }
    }

    public void OnMeleeCollider()
    {
        GetComponentInChildren<BoxCollider>().enabled = true;
    }
    public void OffMeleeCollider()
    {
        GetComponentInChildren<BoxCollider>().enabled = false;
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