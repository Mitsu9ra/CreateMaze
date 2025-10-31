using UnityEngine;
using UnityEngine.SceneManagement; // �V�[���Ǘ��̂��߂ɕK�v
using UnityEngine.UI;

public class ClickSq : MonoBehaviour
{
    // �{�^���R���|�[�l���g���A�^�b�`���邽�߂�public�ϐ�
    public Button button1;
    public Button button2;
    public Button button3;

    void Start()
    {
        // �e�{�^���ɁA��p�̃N���b�N�C�x���g�n���h�������蓖�Ă�
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
        button3.onClick.AddListener(OnButton3Click);
    }

    // �{�^��1���N���b�N���ꂽ�Ƃ��̏���
    void OnButton1Click()
    {
        Debug.Log("�{�^��1���N���b�N����܂����I");
        // ���b�Z�[�W��\��������A�V�[�������[�h����
        SceneManager.LoadScene("TutoriialScene");
    }

    // �{�^��2���N���b�N���ꂽ�Ƃ��̏���
    void OnButton2Click()
    {
        Debug.Log("�{�^��2���N���b�N����܂����I");
    }

    // �{�^��3���N���b�N���ꂽ�Ƃ��̏���
    void OnButton3Click()
    {
        Debug.Log("�{�^��3���N���b�N����܂����I");
    }
}