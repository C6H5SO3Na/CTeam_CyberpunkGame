using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    // インスタンスへのアクセス
    public static SceneLoaderManager Instance;
    // ロードするシーンの名前
    public string loadSceneName;
    void Awake()
    {
        // インスタンスが存在しない場合、このオブジェクトをインスタンスに設定
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 既にインスタンスが存在する場合、このオブジェクトを破棄
            Destroy(this.gameObject);
        }
    }

    void OnEnable()
    {
        // シーンがロードされたときに呼び出されるイベントに登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // シーンがロードされたときに呼び出されるイベントから登録解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ここで各シーンがロードされた後に行いたい操作を実行
        //シーンの名前を確認して行うことができます
        if (scene.name == loadSceneName)
        {
            // SceneLoaderAnimation コンポーネントを検索
            SceneLoaderAnimation sceneLoaderAnimation = FindObjectOfType<SceneLoaderAnimation>();
            // SceneLoaderAnimation コンポーネントが存在する場合
            if (sceneLoaderAnimation != null)
            {
                //ロードが終わった後のアニメーションを再生
                sceneLoaderAnimation.PlayScaleAnimationOnEndAni();
                Debug.Log("load");
            }
            Debug.Log("right");
        }
    }
    public void OnAnimationComplete()
    {
        // アニメーションが完了した後にシーンを切り替え
        SceneManager.LoadScene(loadSceneName);

    }
}
