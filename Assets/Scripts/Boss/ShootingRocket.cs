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
        attackCoroutine = StartCoroutine(HandleShootingRocket());
    }

    void OnDisable()
    {
        bossHitBox.GetComponent<MeshCollider>().enabled = false;
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
    public GameObject explosionPrefab; // 爆発のプレハブ

    private PlayerManager playerManager; //プレイヤーのHP情報

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //プレイヤーを探す
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();

            if (playerManager == null)
            {
                Debug.LogWarning("Player component not found on GameObject with 'Player' tag.");
            }
            else
            {
                
            }
        }
        else
        {
            Debug.LogError("GameObject with 'Player' tag not found.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDestroyed && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground")))
        {

            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerManager.PlayerDamage(10);//プレイヤーのダメージ
                Debug.Log("プレイヤーをヒット10ダメージ");
                Debug.Log("Player HP now: " + PlayerManager.nowHP);
            }

            // 衝突位置を取得
            Vector3 collisionPoint = collision.contacts[0].point;
            collisionPoint.y -= 2f;

            // 爆発エフェクトを衝突位置にスポーン
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, collisionPoint, Quaternion.identity);
            }
            
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public void SetDestroyOnCollision()
    {
        Collider collider = GetComponent<CapsuleCollider>();
    }
}



