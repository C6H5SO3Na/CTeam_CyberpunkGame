using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossLaser : MonoBehaviour
{
    public GameObject laserPrefab; // LineRendererコンポーネントを持つPrefab
    public Transform[] startpoints; // 複数のレーザーの発射点の配列
    public Transform player; // プレイヤーのTransformへの参照
    public GameObject playerobj;

    private List<GameObject> instantiatedLasers = new List<GameObject>(); // 生成されたPrefabを格納するリスト
    private List<Vector3> shotDirections = new List<Vector3>(); // 各レーザーの方向を格納するリスト
    private List<float> initialPlayerXPositions = new List<float>(); // プレイヤーの初期X位置を格納するリスト
    private List<float> initialPlayerZPositions = new List<float>(); // プレイヤーの初期Z位置を格納するリスト
    public float rotationSpeed = 0.0f; // レーザーの回転速度
    public float sweepAngle = 45f; // レーザーが左右にスイープする角度の半分
    private List<float> currentRotationAngles = new List<float>(); // 各レーザーの現在の回転角度
    private List<bool> rotatingLeft = new List<bool>(); // 各レーザーが左に回転しているかどうか

    private Coroutine attackCoroutine; // コルーチンを格納

    public GameObject bossHitBox;

    // ダメージに関連する変数
    public float damageRate = 0.5f; // レーザーがプレイヤーに当たった際にダメージが適用される間隔
    public int laserDamage = 10; // レーザーが与えるダメージ量
    private List<float> lastDamageTimes = new List<float>(); // 各レーザーが最後にダメージを適用した時間を追跡するリスト


    private void Start()
    {
        playerobj = GameObject.FindGameObjectWithTag("Player");
    }
    void OnEnable()
    {
        // スクリプトが有効化された際にリストを再初期化
        initialPlayerXPositions.Clear();
        initialPlayerZPositions.Clear();
        lastDamageTimes.Clear(); // 最後のダメージ適用時間の追跡をクリア

        // 各レーザーの回転速度を初期化し、Prefabを生成
        rotationSpeed = Random.Range(10.0f, 15.0f);

        for (int i = 0; i < startpoints.Length; i++)
        {
            Transform startpoint = startpoints[i];

            // 各スタートポイントでレーザーのPrefabを生成
            GameObject laserInstance = Instantiate(laserPrefab, startpoint.position, startpoint.rotation);
            instantiatedLasers.Add(laserInstance);

            // スクリプトが有効化された時点でランダムなX位置とプレイヤーの現在のZ位置を保存
            initialPlayerXPositions.Add(Random.Range(player.position.x - 5.0f, player.position.x + 5.0f));
            initialPlayerZPositions.Add(player.position.z);

            // 各レーザーのスイープパラメータを初期化
            shotDirections.Add((player.position - startpoint.position).normalized);
            currentRotationAngles.Add(0f);

            // 各レーザーの最後のダメージ適用時間を初期化（0でスタート）
            lastDamageTimes.Add(0f);

            // レーザーのインデックスが偶数か奇数かでスイープ方向を設定
            if (i % 2 == 0)
            {
                rotatingLeft.Add(true); // 偶数番号のレーザーは左にスイープ
            }
            else
            {
                rotatingLeft.Add(false); // 奇数番号のレーザーは右にスイープ
            }
        }

        attackCoroutine = StartCoroutine(LaserRoutine());
    }

    void OnDisable()
    {
        if (bossHitBox != null)
        {
            bossHitBox.GetComponent<MeshCollider>().enabled = false;
        }

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        // 生成されたすべてのレーザーを破壊し、リストをクリア
        foreach (GameObject laser in instantiatedLasers)
        {
            Destroy(laser);
        }
        instantiatedLasers.Clear();
    }

    IEnumerator LaserRoutine()
    {
        while (true)
        {
            for (int i = 0; i < startpoints.Length; i++)
            {
                SweepLaserOnGround(i); // 各発射点でレーザーをスイープ
                UpdateLaser(i); // 各発射点でレーザーを更新
            }
            yield return null;
        }
    }

    void SweepLaserOnGround(int index)
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime;

        // 各発射点でスイープ角度内で左右に回転
        if (rotatingLeft[index])
        {
            currentRotationAngles[index] -= rotationThisFrame;
            if (currentRotationAngles[index] <= -sweepAngle)
            {
                rotatingLeft[index] = false; // 方向を切り替え
            }
        }
        else
        {
            currentRotationAngles[index] += rotationThisFrame;
            if (currentRotationAngles[index] >= sweepAngle)
            {
                rotatingLeft[index] = true; // 方向を切り替え
            }
        }

        // 保存されたXおよびZ位置に基づいて各レーザーのターゲットポイントを計算
        Vector3 targetPoint = new Vector3(
            initialPlayerXPositions[index], // 有効化時に保存されたランダムなX位置を使用
            150, // 地面のレベル (y = 0)
            initialPlayerZPositions[index] // 有効化時に保存されたZ位置を使用
        );

        // ターゲットポイントにスイープ回転を適用
        Vector3 sweepDirection = Quaternion.Euler(0, currentRotationAngles[index], 0) * (targetPoint - startpoints[index].position).normalized;
        shotDirections[index] = sweepDirection;
    }

    void UpdateLaser(int index)
    {
        // 生成されたレーザーPrefabからLineRendererを取得
        LineRenderer lr = instantiatedLasers[index].GetComponent<LineRenderer>();

        // レーザーの始点を発射点に設定
        lr.SetPosition(0, startpoints[index].position);

        RaycastHit hit;
        if (Physics.Raycast(startpoints[index].position, shotDirections[index], out hit))
        {
            // オブジェクト（地面またはプレイヤー）に当たった位置にレーザーの終点を設定
            lr.SetPosition(1, hit.point);

            if (hit.transform.CompareTag("Player"))
            {
                ApplyLaserDamage(index); // プレイヤーにダメージを適用
            }
        }
        else
        {
            // 当たらなければ、レーザーをショット方向に遠くまで延長
            lr.SetPosition(1, startpoints[index].position + shotDirections[index] * 5000);
        }
    }

    void ApplyLaserDamage(int index)
    {
        // 最後のダメージ適用から指定時間（damageRate）が経過しているかを確認
        if (Time.time >= lastDamageTimes[index] + damageRate)
        {
            //player.GetComponent<PlayerHealth>().ApplyDamage(laserDamage);

            // 各レーザーの最後のダメージ適用時間を更新
            lastDamageTimes[index] = Time.time;
            PlayerManager.PlayerDamage(laserDamage);//プレイヤーのダメージ
            playerobj.GetComponent<PlayerCollision>().TriggerDamage("Damage", 1.033f + 0.5f);
            Debug.Log("レーザーがプレイヤーに当たり、" + laserDamage + " ダメージを適用");
            Debug.Log("Player HP now: " + PlayerManager.nowHP);
        }
    }
}
