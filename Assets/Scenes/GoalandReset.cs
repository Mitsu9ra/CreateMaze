using UnityEngine;
using TMPro;

public class Goal : MonoBehaviour
{
    private static int goalCount = 0;
    private bool isResetting = false; // ��d���Z�b�g�h�~

    private TextMeshProUGUI goalText; // ��ʂɕ\������Text(TMP)

    void Start()
    {
        // �V�[����́uGoalText�v��T���Ď擾
        GameObject textObj = GameObject.Find("GoalText");
        if (textObj != null)
        {
            goalText = textObj.GetComponent<TextMeshProUGUI>();
            UpdateGoalText();
        }
        else
        {
            Debug.LogWarning("GoalText �I�u�W�F�N�g���V�[�����Ɍ�����܂���BCanvas�� Text (TMP) ��z�u���Ă��������B");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isResetting) return; // ��d���Z�b�g�h�~

        if (other.CompareTag("Player"))
        {
            goalCount++; // �S�[���񐔂����Z
            Debug.Log($"���݂̃S�[����: {goalCount}");
            UpdateGoalText();

            MazeGenerator_WallExtend_Proper mazeGen = FindObjectOfType<MazeGenerator_WallExtend_Proper>();
            if (mazeGen != null)
            {
                isResetting = true;
                mazeGen.ResetMazeSafe(); // ���H�Đ���
                isResetting = false;
            }
        }
    }

    private void UpdateGoalText()
    {
        if (goalText != null)
        {
            goalText.text = $"�S�[����: {goalCount}";
        }
    }
}
