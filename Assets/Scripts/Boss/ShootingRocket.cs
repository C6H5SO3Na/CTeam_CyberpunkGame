using System.Collections;
using UnityEngine;

public class ShootingRocket : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform[] launchPoints; // ロケットを発射するポイントの配列
    public float heightAbovePlayer = 30.0f; // プレイヤーの上にロケットが開始する高さ
    public float spreadRadius = 5.0f; // プレイヤー周りにロケットが広がる半径
    public float ascentSpeed = 20.0f; // ロケットが上昇する速度
    public float descentDelay = 1.0f; // ロケットが頂点に到達した後、下降を開始するまでの遅延時間
    public float rocketLifetime = 10.0f; // ロケットの寿命（破壊されるまでの時間）
    public float rotationSpeed = 360.0f; // ロケットが下向きに回転する速度

    private Coroutine attackCoroutine;

    public GameObject bossHitBox;

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

        for (int i = 0; i < launchPoints.Length; i++)
        {
            // プレイヤーの周りでランダムなオフセット位置を計算し、ロケットを拡散させる
            Vector3 randomOffset = new Vector3(
                Random.Range(-spreadRadius, spreadRadius),
                0,
                Random.Range(-spreadRadius, spreadRadius)
            );

            // 最初の発射ポイントの場合は、オフセットをゼロに設定する
            if (i == 0)
            {
                randomOffset = Vector3.zero;
            }

            Vector3 targetPosition = player.transform.position + randomOffset;

            // 発射ポイントでロケットを生成し、X軸を-90度に設定して生成する
            GameObject rocketInstance = Instantiate(rocketPrefab, launchPoints[i].position, Quaternion.Euler(-90, 0, 0));

            // 衝突処理用にRocketCollisionHandlerスクリプトを追加
            RocketCollisionHandler collisionHandler = rocketInstance.AddComponent<RocketCollisionHandler>();
            collisionHandler.SetDestroyOnCollision();

            // ロケットを上方に移動させ、目標位置の上に配置する
            StartCoroutine(MoveRocketUpwardsAndRotate(rocketInstance, targetPosition));

            // ロケットの破壊はすぐには行わず、後で処理する
        }

        // 攻撃が終了したら、スクリプトを無効にする
        yield return new WaitForSeconds(10.0f);
        this.enabled = false;
    }

    IEnumerator MoveRocketUpwardsAndRotate(GameObject rocket, Vector3 targetPosition)
    {
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        rb.useGravity = false;

        MeshRenderer rocketRenderer = rocket.GetComponentInChildren<MeshRenderer>();
        if (rocketRenderer != null)
        {
            // ロケットが発射され、上昇中はシャドウを無効にする
            rocketRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        Vector3 peakPosition = targetPosition + Vector3.up * heightAbovePlayer;

        // ロケットを頂点位置に向かって上昇させる
        while (rocket != null && Vector3.Distance(rocket.transform.position, peakPosition) > 0.1f)
        {
            Vector3 moveDirection = (peakPosition - rocket.transform.position).normalized;
            rocket.transform.position += moveDirection * ascentSpeed * Time.deltaTime;

            RotateRocketTowardsDirection(rocket, moveDirection);

            yield return null;
        }

        // 下降を開始する前に少し待つ
        yield return new WaitForSeconds(descentDelay);

        // ロケットを下向きに回転させる
        if (rocket != null)
        {
            yield return StartCoroutine(RotateRocketToFaceDown(rocket));

            // ロケットが回転し、下降を開始した後にシャドウを有効にする
            if (rocketRenderer != null)
            {
                rocketRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }

            // 重力を有効にしてロケットを落下させる
            if (rocket != null)
            {
                rb = rocket.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                }

                // ロケットの寿命が尽きるまで待ってから破壊する
                yield return new WaitForSeconds(rocketLifetime - descentDelay);
                if (rocket != null)
                {
                    Destroy(rocket);
                }
            }
        }
    }


    IEnumerator RotateRocketToFaceDown(GameObject rocket)
    {
        Vector3 downwardDirection = Vector3.down;

        // ロケットが下向きになるまで回転を続ける
        while (rocket != null && Vector3.Angle(rocket.transform.forward, downwardDirection) > 1f)
        {
            // ロケットを滑らかに下向きに回転させる
            Quaternion targetRotation = Quaternion.LookRotation(downwardDirection);
            rocket.transform.rotation = Quaternion.RotateTowards(rocket.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            yield return null; // 次のフレームまで待つ
        }
    }

    void RotateRocketTowardsDirection(GameObject rocket, Vector3 direction)
    {
        // ロケットを移動方向に合わせるための回転を計算する
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rocket.transform.rotation = Quaternion.RotateTowards(rocket.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

// このスクリプトはロケットの衝突を処理します
public class RocketCollisionHandler : MonoBehaviour
{
    private bool isDestroyed = false;

    private PlayerManager playerManager; //プレイヤーのHP情報

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); //プレイヤーを探す
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();

            if (playerManager == null)
            {
                Debug.LogError("Player component not found on GameObject with 'Player' tag.");
            }
        }
        else
        {
            Debug.LogError("GameObject with 'Player' tag not found.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ロケットが地面またはプレイヤーに衝突した場合
        if (!isDestroyed && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground")))
        {
           if(collision.gameObject.CompareTag("Player"))
           {
                playerManager.PlayerDamage(10);//プレイヤーのダメージ
                Debug.Log("プレイヤーをヒット10ダメージ");
                Debug.Log("Player HP now: " + playerManager.nowHP);
            }
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public void SetDestroyOnCollision()
    {
        // ロケットにコライダーがあり、OnTriggerEnterを使用する場合はトリガーとして設定されていることを確認する
        Collider collider = GetComponent<CapsuleCollider>();
    }
}
