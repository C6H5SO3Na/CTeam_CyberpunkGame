using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BossController : MonoBehaviour, IDamageable
{
    public int HPMiddle = 200;
    //public int HPLeft = 50;
    //public int HPRight = 50;

   // public bool leftdead = false;
   // public bool rightdead = false;

    // References to the attack scripts
    private bossLaser bossLaserScript;
    private ShootingRocket shootingRocketScript;

    public float attackInterval = 5.0f; // Time between attacks
    private int lastAttack = -1; // Track the last attack (-1 means no previous attack)

    public GameObject bossHitBoxMiddle;
    public GameObject bossHitBoxRight;
    public GameObject bossHitBoxLeft;



    public GameObject Explosion;
    public GameObject hitableEffect;
    private GameObject activeHitBox; // Track the active hitbox
    private GameObject activeHitableEffect;


    private bool gameClear;
    private Animator animator;
    private float fadeDuration = 5.0f;

    public GameObject gameManager;

    // Audio clips for death, laser, and missile
    public AudioClip deathSound;
    public AudioClip laserSound;
    public AudioClip missileSound;

    // AudioSource to play the sounds
    private AudioSource audioSource;

    void Start()
    {
        // Get references to the attack scripts attached to this GameObject
        bossLaserScript = GetComponent<bossLaser>();
        shootingRocketScript = GetComponent<ShootingRocket>();
        animator = GetComponent<Animator>();

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Start the attack pattern cycle
        StartCoroutine(AttackPatternCycle());
        gameClear = false;
    }

    public void TakeDamage(int damage)
    {
        HPMiddle -= damage;
        Debug.Log($"Boss took {damage} damage. Remaining HP: {HPMiddle}");

        // Check if the boss's health reaches zero
        if (HPMiddle <= 0 && !gameClear)
        {
            // Play the death sound
            if (deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }

            StartCoroutine(FadeOutAndDestroy());

            Vector3 spawnpos = new Vector3(bossHitBoxMiddle.transform.position.x, bossHitBoxMiddle.transform.position.y + 5.0f, bossHitBoxMiddle.transform.position.z - 10.0f);
            GameObject explosion = Instantiate(Explosion, spawnpos, bossHitBoxMiddle.transform.rotation);
            explosion.transform.SetParent(bossHitBoxMiddle.transform);

            gameClear = true;
            animator.SetBool("gameClear", gameClear);

            bossLaserScript.enabled = false;
            shootingRocketScript.enabled = false;
        }
    }

    private void Update()
    {

        if (HPMiddle <= 0 && !gameClear)
        {
            StartCoroutine(PlayDeathSoundRepeatedly());
            StartCoroutine(FadeOutAndDestroy());

            Vector3 spawnpos = new Vector3(bossHitBoxMiddle.transform.position.x, bossHitBoxMiddle.transform.position.y + 5.0f, bossHitBoxMiddle.transform.position.z - 10.0f);
            GameObject explosion = Instantiate(Explosion, spawnpos, bossHitBoxMiddle.transform.rotation);
            explosion.transform.SetParent(bossHitBoxMiddle.transform);

            gameClear = true;
            animator.SetBool("gameClear", gameClear);

            bossLaserScript.enabled = false;
            shootingRocketScript.enabled = false;
        }
    }


    public GameObject GetActiveHitBox()
    {
        return activeHitBox;
    }
    IEnumerator FadeOutAndDestroy()
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

        gameManager.GetComponent<GameManager>().isClear = true;
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
                    SpawnHitableEffect();
                    ExecuteShootingRocket();
                    break;
                case 1:
                    SpawnHitableEffect();
                    ExecuteBossLaser();
                    break;
                default:
                    break;
            }
            lastAttack = attackChoice;
            

            yield return new WaitForSeconds(attackInterval);
        }
    }
    public void SpawnHitableEffect()
    {
        if (hitableEffect != null && activeHitableEffect == null)
        {
            // Randomly choose one of the three hitboxes (0: Middle, 1: Left, 2: Right)
            int randomHitboxIndex = Random.Range(0, 3);

            switch (randomHitboxIndex)
            {
                case 0:
                    activeHitBox = bossHitBoxMiddle;
                    break;
                case 1:
                    activeHitBox = bossHitBoxLeft;
                    break;
                case 2:
                    activeHitBox = bossHitBoxRight;
                    break;
            }

            // Enable the chosen hitbox collider
            if (activeHitBox != null)
            {
                activeHitBox.GetComponent<CapsuleCollider>().enabled = true;
            }

            // Instantiate the hitable effect at the chosen hitbox's position
            activeHitableEffect = Instantiate(hitableEffect, activeHitBox.transform.position, activeHitBox.transform.rotation);

            // Set the parent to the chosen hitbox
            activeHitableEffect.transform.SetParent(activeHitBox.transform);

            // Adjust the local position relative to the active hitbox
            activeHitableEffect.transform.localPosition += new Vector3(0f, 0.17f, 1.23f);  // Adjust as needed
        }
    }

    public void DestroyHitableEffect()
    {
        if (activeHitableEffect != null)
        {
            Destroy(activeHitableEffect);
            activeHitableEffect = null;
        }

        // Disable the active hitbox collider
        if (activeHitBox != null)
        {
            activeHitBox.GetComponent<MeshCollider>().enabled = false;
            activeHitBox = null;
        }
    }

    void ExecuteShootingRocket()
    {
        bossLaserScript.enabled = false;
        shootingRocketScript.enabled = true;

        StartCoroutine(PlayMissileSoundRepeatedly());
    }

    IEnumerator PlayMissileSoundRepeatedly()
    {
        float firstdelay = 0.8f;
        yield return new WaitForSeconds(firstdelay);
        int times = 6;  
        float delay = 0.23f;  

        for (int i = 0; i < times; i++)
        {
            if (missileSound != null)
            {
                audioSource.PlayOneShot(missileSound);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    void ExecuteBossLaser()
    {
        bossLaserScript.enabled = true;
        shootingRocketScript.enabled = false;

        // Play laser sound when firing a laser
        if (laserSound != null)
        {
            audioSource.PlayOneShot(laserSound);
        }
    }

    IEnumerator PlayDeathSoundRepeatedly()
    {
        int times = 3; // Number of times to play the sound
        float delay = deathSound.length; // Delay between sound plays

        for (int i = 0; i < times; i++)
        {
            if (deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
            yield return new WaitForSeconds(delay); // Wait before playing the next sound
        }
    }
}
