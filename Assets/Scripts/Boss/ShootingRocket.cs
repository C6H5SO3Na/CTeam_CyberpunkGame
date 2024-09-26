using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRocket : MonoBehaviour
{
    public GameObject rocketPrefab;
    public GameObject explosionPrefab; // 爆発のプレハブ
    public float heightAbovePlayer = 30.0f; // プレイヤーの上にロケットが開始する高さ
    public float spreadRadius = 7.0f; // プレイヤー周りにロケットが広がる半径
    public float delayBeforeSpawn = 3.0f; // ロケットがスポーンするまでの遅延時間
    public int rocketCount = 6; // スポーンするロケットの数
    public float rocketLifetime = 10.0f; // ロケットの寿命（破壊されるまでの時間）

    private Coroutine attackCoroutine;

    public GameObject bossHitBox;
    public List<GameObject> bossArm; // ボスの腕のリスト


    void OnEnable()
    {
        GameObject activeHitBox = GetComponentInParent<BossController>().GetActiveHitBox();

        // Ensure the correct hitbox's collider is enabled
        if (activeHitBox != null)
        {
            activeHitBox.GetComponent<MeshCollider>().enabled = true;
        }

        GetComponent<BossController>().SpawnHitableEffect();

        attackCoroutine = StartCoroutine(HandleShootingRocket());
    }

    void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator HandleShootingRocket()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        List<Vector3> spawnPositions = new List<Vector3>(); // スポーンした位置を追跡

        // ボスの腕のアニメーターに対してトリガーを設定
        foreach (GameObject arm in bossArm)
        {
            Animator armAnimator = arm.GetComponent<Animator>();
            if (armAnimator != null)
            {
                armAnimator.SetTrigger("Shoot");
            }
        }

        // スポーンするまでの遅延
        yield return new WaitForSeconds(delayBeforeSpawn);

        gameObject.GetComponentInParent<BossController>().DestroyHitableEffect();
        GameObject activeHitBox = GetComponentInParent<BossController>().GetActiveHitBox();
        if (activeHitBox != null)
        {
            activeHitBox.GetComponent<MeshCollider>().enabled = false;
        }

        for (int i = 0; i < rocketCount; i++)
        {
            Vector3 spawnPosition;
            bool validPosition = false;
            int maxAttempts = 10; // 最大試行回数
            int attempt = 0;

            // ロケットが十分離れた位置にスポーンするまで試行
            do
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spreadRadius, spreadRadius),
                    0,
                    Random.Range(-spreadRadius, spreadRadius)
                );

                spawnPosition = player.transform.position + randomOffset + Vector3.up * heightAbovePlayer;
                validPosition = true;

                // 既存のロケットとの距離をチェック
                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(pos, spawnPosition) < 3.0f) // 最小距離を設定（例: 3.0f）
                    {
                        validPosition = false;
                        break;
                    }
                }

                attempt++;
            } while (!validPosition && attempt < maxAttempts);

            // 有効なスポーン位置を追加
            spawnPositions.Add(spawnPosition);

            GameObject rocketInstance = Instantiate(rocketPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));

            // 衝突処理用にRocketCollisionHandlerスクリプトを追加
            RocketCollisionHandler collisionHandler = rocketInstance.AddComponent<RocketCollisionHandler>();
            collisionHandler.explosionPrefab = explosionPrefab; // 爆発プレハブを設定
            collisionHandler.SetDestroyOnCollision();

            // ロケットの寿命が尽きるまで待ってから破壊する
            Destroy(rocketInstance, rocketLifetime);
        }

        yield return new WaitForSeconds(10.0f);
        this.enabled = false;
    }
}



public class RocketCollisionHandler : MonoBehaviour
{
    private bool isDestroyed = false;
    public GameObject explosionPrefab; // Explosion prefab
    private float explosionRadius = 5f; 
    private int explosionDamage = 20; // Damage dealt by the explosion

    private LayerMask playerLayer; // Only affect the Player

    private void Awake()
    {
        // Assuming the player's layer is named "Player"
        playerLayer = LayerMask.GetMask("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDestroyed && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground")))
        {
            // Only proceed if the rocket hits the ground
            if (collision.gameObject.CompareTag("Ground"))
            {
                // Trigger explosion effect and apply AoE damage
                Explode(collision.contacts[0].point);
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                // Direct hit on the player
                PlayerManager.PlayerDamage(30); // Deal direct damage
                Debug.Log("Player HP now: " + PlayerManager.nowHP);
            }

            isDestroyed = true;
            Destroy(gameObject); // Destroy the rocket itself
        }
    }

    // Method to handle the explosion effect and AoE damage
    private void Explode(Vector3 explosionPoint)
    {
        // Lower the explosion position slightly for a more realistic effect
        explosionPoint.y -= 4f;

        // Spawn the explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, explosionPoint, Quaternion.identity);
        }

        // Apply AoE damage to the player only within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPoint, explosionRadius, playerLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            // Check if we hit the player
            if (hitCollider.CompareTag("Player"))
            {
                PlayerManager.PlayerDamage(explosionDamage);
            }
        }

    }

    public void SetDestroyOnCollision()
    {
        Collider collider = GetComponent<CapsuleCollider>();
    }
}



