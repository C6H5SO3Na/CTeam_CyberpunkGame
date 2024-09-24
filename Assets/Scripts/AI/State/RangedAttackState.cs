using UnityEngine;
using System.Collections;

public class RangedAttackState : IAIState
{
    private AIScript ai;
    private float projectileCooldown;
    private bool isFiringLaser = false;
    private float laserDamageTimer = 0f;

    // レーザー関連の変数
    private float laserDuration = 2f; // レーザーの継続時間
    private float laserDamageRate = 0.1f; // レーザーのダメージを与える頻度
    public RangedAttackState()
    {
        projectileCooldown = 4.0f; // 射撃クールダウン時間
    }

    public void Enter(AIScript ai)
    {
        this.ai = ai;
        ai.canRangeattack = false;
        ai.Invoke(nameof(ai.ResetRangeCD), projectileCooldown);
        ai.isAttackingmelee = false;
        ai.isRunning = false;
        ai.isWalking = false;
        ai.isAttackingranged = true;

        if (ai.laser != null)
        {
            ai.laser.enabled = false; // レーザーを無効にする
        }

        ai.enemyAnim.SetTrigger("isAttack"); // 攻撃アニメーションを開始する
    }

    public void Execute(AIScript ai)
    {
        if (ai.isHurt)
        {
            ai.ChangeState(new HurtState()); // ダメージを受けたらHurtStateに移行
        }
        else if (!ai.playerInSightRange)
        {
            ai.ChangeState(new PatrolState()); // プレイヤーが視界外ならPatrolStateに移行
        }
        else if (ai.canRangeattack && !isFiringLaser)
        {
            if (IsFacingPlayer())
            {
                FireLaser(); // プレイヤーに向いている場合のみレーザー攻撃を開始
            }
            else
            {
                RotateTowardsPlayer(); // プレイヤーの方向を向くように回転
            }
        }
        
    }

    public void Exit(AIScript ai)
    {
        ai.isAttackingranged = false; // 遠距離攻撃の状態を終了
    }

    private void RotateTowardsPlayer()
    {
        // プレイヤーの位置への方向を取得
        Vector3 directionToPlayer = (ai.player.position - ai.transform.position).normalized;
        directionToPlayer.y = 0; // 水平方向のみの回転にするためY軸を無視

        // 現在の向きからプレイヤーへの向きへゆっくりと回転
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, targetRotation, Time.deltaTime * 1f); // 回転速度を調整（2fは速度）
    }


    public void FireLaser()
    {
        //レーザーを発射
        if (!isFiringLaser)
        {
            ai.StartCoroutine(FireLaserCoroutine()); // レーザー発射コルーチンを開始
            ai.audioSource.PlayOneShot(ai.laserSound);
        }
    }
    

    private bool IsFacingPlayer()
    {
        // AIからプレイヤーへの方向ベクトルを取得
        Vector3 directionToPlayer = (ai.player.position - ai.transform.position).normalized;

        // AIの前方ベクトルとプレイヤーへの方向ベクトルとのドット積を計算
        float dotProduct = Vector3.Dot(ai.transform.forward, directionToPlayer);

        // ドット積が0.5より大きければ、AIがプレイヤーの方向を向いていると判断
        return dotProduct > 0.5f;
    }

    private IEnumerator FireLaserCoroutine()
    {
        isFiringLaser = true;

        if (ai.laser != null)
        {
            ai.laser.enabled = true;  // レーザーを有効にする（発射時のみ）
        }

        GameObject spawnedBeamSource = null;
        if (ai.beamsource != null)
        {
            // 発射ポイントにビームソースを生成する
            spawnedBeamSource = GameObject.Instantiate(ai.beamsource, ai.Firepoint.transform.position, ai.Firepoint.transform.rotation, ai.Firepoint.transform);

            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Play();  // レーザーのパーティクルエフェクトを再生
            }
        }

        // プレイヤーの位置を一度だけ計算する
        Vector3 playerPosition = SmoothPredictPlayerPosition();
        Vector3 laserDirection = (playerPosition - ai.Firepoint.transform.position).normalized;

        // 計算された方向に基づいてレーザーの方向と回転を設定
        Quaternion targetRotation = Quaternion.LookRotation(laserDirection);
        ai.Firepoint.transform.rotation = targetRotation;

        float timer = 0f;
        while (timer < laserDuration)
        {
            // レーザーを同じ方向に発射し続ける
            Vector3 laserEndPoint = CalculateLaserEndPoint(laserDirection);

            if (ai.laser != null)
            {
                ai.laser.SetPosition(0, ai.Firepoint.transform.position);  // レーザーの開始位置を設定
                ai.laser.SetPosition(1, laserEndPoint);  // レーザーの終了位置を設定
            }

            // レーザーがプレイヤーにヒットしている場合、ダメージを与える
            if (HitPlayer(laserDirection))
            {
                laserDamageTimer += Time.deltaTime;
                if (laserDamageTimer >= laserDamageRate)
                {
                    ApplyLaserDamage(); // ダメージを適用
                    laserDamageTimer = 0f;  // ダメージタイマーをリセット
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // レーザーを無効にし、攻撃後にクリーンアップ
        if (ai.laser != null)
        {
            ai.laser.enabled = false;  // 攻撃が終了したらレーザーを無効にする
        }

        // レーザー攻撃が終了した後、ビームソースを破壊する
        if (spawnedBeamSource != null)
        {
            ParticleSystem beamParticles = spawnedBeamSource.GetComponentInChildren<ParticleSystem>();
            if (beamParticles != null)
            {
                beamParticles.Stop();  // ビームのパーティクルエフェクトを停止
            }
            GameObject.Destroy(spawnedBeamSource);  // ビームソースを破壊
        }

        // 発射状態をリセットし、アイドル状態に戻る
        isFiringLaser = false;
        ai.ChangeState(new IdleState());  // レーザー攻撃終了後、IdleStateに移行
    }

    // レーザーがプレイヤーにヒットしているか確認
    private bool HitPlayer(Vector3 laserDirection)
    {
        float laserMaxDistance = 150f;
        if (Physics.Raycast(ai.Firepoint.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    // プレイヤーの位置を予測してスムーズに補正
    private Vector3 SmoothPredictPlayerPosition()
    {
        Vector3 currentPosition = ai.player.position;

        float playerHeight = ai.player.GetComponent<Collider>().bounds.extents.y;
        currentPosition.y = ai.player.position.y + playerHeight;

        return currentPosition;
    }

    // レーザーの終点を計算
    private Vector3 CalculateLaserEndPoint(Vector3 laserDirection)
    {
        float laserMaxDistance = 150f;
        if (Physics.Raycast(ai.Firepoint.transform.position, laserDirection, out RaycastHit hit, laserMaxDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return ai.player.position + Vector3.up * hit.collider.bounds.extents.y; // プレイヤーの中心点を狙う
            }
            else
            {
                return hit.point; // プレイヤー以外のオブジェクトにヒットした場合
            }
        }
        return ai.Firepoint.transform.position + laserDirection * laserMaxDistance; // ヒットしない場合、最大距離までレーザーを飛ばす
    }

    // レーザーのダメージを適用
    private void ApplyLaserDamage()
    {
        PlayerManager.PlayerDamage(10);//プレイヤーのダメージ
        Debug.Log("プレイヤーにヒット! 10ダメージを適用中...");
        Debug.Log("Player HP now: " + PlayerManager.nowHP);
    }
}
