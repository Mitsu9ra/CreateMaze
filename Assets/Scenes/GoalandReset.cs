using UnityEngine;
using TMPro;

public class Goal : MonoBehaviour
{
    private static int goalCount = 0;
    private bool isResetting = false; // 二重リセット防止

    private TextMeshProUGUI goalText; // 画面に表示するText(TMP)

    void Start()
    {
        // シーン上の「GoalText」を探して取得
        GameObject textObj = GameObject.Find("GoalText");
        if (textObj != null)
        {
            goalText = textObj.GetComponent<TextMeshProUGUI>();
            UpdateGoalText();
        }
        else
        {
            Debug.LogWarning("GoalText オブジェクトがシーン内に見つかりません。Canvasに Text (TMP) を配置してください。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isResetting) return; // 二重リセット防止

        if (other.CompareTag("Player"))
        {
            goalCount++; // ゴール回数を加算
            Debug.Log($"現在のゴール回数: {goalCount}");
            UpdateGoalText();

            MazeGenerator_WallExtend_Proper mazeGen = FindObjectOfType<MazeGenerator_WallExtend_Proper>();
            if (mazeGen != null)
            {
                isResetting = true;
                mazeGen.ResetMazeSafe(); // 迷路再生成
                isResetting = false;
            }
        }
    }

    private void UpdateGoalText()
    {
        if (goalText != null)
        {
            goalText.text = $"ゴール回数: {goalCount}";
        }
    }
}
