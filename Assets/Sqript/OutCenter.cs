using UnityEngine;
using UnityEngine.SceneManagement;

public class OutCenter : MonoBehaviour
{
    public void SwitchScene()
    {
        SceneManager.LoadScene("OutCenterScene");
    }
    void Update()
    {
        // Enter�L�[�i���C����Enter�L�[ �܂��� �e���L�[��Enter�L�[�j�������ꂽ�u�Ԃ����m
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // �{�^���������ꂽ�Ƃ��Ɠ����������Ăяo��
            SwitchScene();
        }
    }
}


