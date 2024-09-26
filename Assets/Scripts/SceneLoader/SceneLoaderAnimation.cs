using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneLoaderAnimation : MonoBehaviour
{
    private Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);// 初期スケール
    private Vector3 endScale = new Vector3(4000f, 4000f, 4000f); // 最終スケール
    private float duration = 1.0f;// アニメーションの時間
    [SerializeField] GameObject sceneLoadingImg;// シーンの読み込み画像
    void Start()
    {
        // オブジェクトがシーン切り替え中も維持されるように
        DontDestroyOnLoad(gameObject);
        PlayScaleAnimation();
    }

    void PlayScaleAnimation()
    {
        // 初期スケールを設定
        sceneLoadingImg.transform.localScale = startScale;

        // DOTweenを使用してスケールのアニメーションを実行
        sceneLoadingImg.transform.DOScale(endScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnAnimationComplete());
    }

    void OnAnimationComplete()
    {
        // アニメーションが完了した後に別の部分にシーン切り替えを通知
        SceneLoaderManager.Instance.OnAnimationComplete();
    }

    public void PlayScaleAnimationOnEndAni()
    {
        // スケールのアニメーションを逆再生し、完了後にオブジェクトを破棄
        sceneLoadingImg.transform.DOScale(startScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}
