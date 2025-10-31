using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionButton : MonoBehaviour
{
    // Inspector����ݒ肷��ړ���̃V�[���� (��: GameScene, TitleScene�Ȃ�)
    [SerializeField]
    private string nextSceneName = "InCenterScene";

    // �t�F�[�h�ɂ����鎞�� (FadeManager�̎����Ɉˑ�)
    [SerializeField]
    private float fadeDuration = 1.0f;

    /// <summary>
    /// �{�^���� On Click �C�x���g����Ăяo�����\�b�h
    /// </summary>
    public void OnButtonClick()
    {
        // FadeManager�̃C���X�^���X���擾���A�V�[���ړ����Ăяo��
        if (FadeManager.Instance != null)
        {
            // LoadScene(�ړ���̃V�[����, �t�F�[�h�ɂ����鎞��)
            FadeManager.Instance.LoadScene(nextSceneName, fadeDuration);
        }
        else
        {
            Debug.LogError("FadeManager�̃C���X�^���X��������܂���B�ŏ��̃V�[���ɔz�u����Ă��邩�m�F���Ă��������B");
        }
    }
    void Update()
    {
        // Enter�L�[�i���C����Enter�L�[ �܂��� �e���L�[��Enter�L�[�j�������ꂽ�u�Ԃ����m
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // �{�^���������ꂽ�Ƃ��Ɠ����������Ăяo��
            OnButtonClick();
        }
    }
}