using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    // �C���X�^���X�ւ̃A�N�Z�X
    public static SceneLoaderManager Instance;
    // ���[�h����V�[���̖��O
    public string loadSceneName;
    void Awake()
    {
        // �C���X�^���X�����݂��Ȃ��ꍇ�A���̃I�u�W�F�N�g���C���X�^���X�ɐݒ�
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // ���ɃC���X�^���X�����݂���ꍇ�A���̃I�u�W�F�N�g��j��
            Destroy(this.gameObject);
        }
    }

    void OnEnable()
    {
        // �V�[�������[�h���ꂽ�Ƃ��ɌĂяo�����C�x���g�ɓo�^
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // �V�[�������[�h���ꂽ�Ƃ��ɌĂяo�����C�x���g����o�^����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �����Ŋe�V�[�������[�h���ꂽ��ɍs��������������s
        //�V�[���̖��O���m�F���čs�����Ƃ��ł��܂�
        if (scene.name == loadSceneName)
        {
            // SceneLoaderAnimation �R���|�[�l���g������
            SceneLoaderAnimation sceneLoaderAnimation = FindObjectOfType<SceneLoaderAnimation>();
            // SceneLoaderAnimation �R���|�[�l���g�����݂���ꍇ
            if (sceneLoaderAnimation != null)
            {
                //���[�h���I�������̃A�j���[�V�������Đ�
                sceneLoaderAnimation.PlayScaleAnimationOnEndAni();
                Debug.Log("load");
            }
            Debug.Log("right");
        }
    }
    public void OnAnimationComplete()
    {
        // �A�j���[�V����������������ɃV�[����؂�ւ�
        SceneManager.LoadScene(loadSceneName);

    }
}
