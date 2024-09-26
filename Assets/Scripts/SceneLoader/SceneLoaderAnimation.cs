using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneLoaderAnimation : MonoBehaviour
{
    private Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);// �����X�P�[��
    private Vector3 endScale = new Vector3(4000f, 4000f, 4000f); // �ŏI�X�P�[��
    private float duration = 1.0f;// �A�j���[�V�����̎���
    [SerializeField] GameObject sceneLoadingImg;// �V�[���̓ǂݍ��݉摜
    void Start()
    {
        // �I�u�W�F�N�g���V�[���؂�ւ������ێ������悤��
        DontDestroyOnLoad(gameObject);
        PlayScaleAnimation();
    }

    void PlayScaleAnimation()
    {
        // �����X�P�[����ݒ�
        sceneLoadingImg.transform.localScale = startScale;

        // DOTween���g�p���ăX�P�[���̃A�j���[�V���������s
        sceneLoadingImg.transform.DOScale(endScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => OnAnimationComplete());
    }

    void OnAnimationComplete()
    {
        // �A�j���[�V����������������ɕʂ̕����ɃV�[���؂�ւ���ʒm
        SceneLoaderManager.Instance.OnAnimationComplete();
    }

    public void PlayScaleAnimationOnEndAni()
    {
        // �X�P�[���̃A�j���[�V�������t�Đ����A������ɃI�u�W�F�N�g��j��
        sceneLoadingImg.transform.DOScale(startScale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}
