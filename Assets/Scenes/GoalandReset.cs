using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    private static int goalCount = 0;
    private bool isResetting = false;
    private bool isGameFinished = false;

    private TextMeshProUGUI goalText;
    private TextMeshProUGUI finishText;
    private ShowCountdown countdown;

    private static float totalScore = 0f;
    private float startDelay = 1f;
    private float elapsedTime = 0f;

    // ✅ ボタン参照を追加
    public Button titleButton;
    public Button rankingButton;

    void Start()
    {
        countdown = FindObjectOfType<ShowCountdown>();

        GameObject textObj = GameObject.Find("GoalText");
        if (textObj != null)
        {
            goalText = textObj.GetComponent<TextMeshProUGUI>();
            UpdateGoalText();
        }

        GameObject finishObj = GameObject.Find("FinishText");
        if (finishObj != null)
        {
            finishText = finishObj.GetComponent<TextMeshProUGUI>();
            finishText.gameObject.SetActive(false);
        }

        // ✅ ボタン初期化
        if (titleButton != null)
            titleButton.gameObject.SetActive(false);
        if (rankingButton != null)
            rankingButton.gameObject.SetActive(false);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < startDelay)
            return;

        if (countdown != null && countdown.RemainingTime <= 0 && !isGameFinished)
        {
            FinishGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int flagValue = Title.flag;
        if (isResetting) return;

        if (other.CompareTag("Player"))
        {
            goalCount++;

            // ここを先に呼び出す！
            MazeGenerator_WallExtend_Proper mazeGen = FindObjectOfType<MazeGenerator_WallExtend_Proper>();

            if (countdown != null && mazeGen != null)
            {
                float addScore = 0;
                // スコア = 残りタイム × 迷路の幅
                if (mazeGen.width > 28)
                {
                    addScore = countdown.RemainingTime * mazeGen.width * goalCount * 15;
                }
                else if (mazeGen.width >20 && mazeGen.width <28)
                {
                    addScore = countdown.RemainingTime * mazeGen.width * goalCount;
                }
                else
                {
                    addScore = countdown.RemainingTime * goalCount *5;
                }
                    totalScore += addScore;
                Debug.Log($"スコア加算: +{addScore:F2} (合計: {totalScore:F2})");

                //  残り時間 +
                if (Title.flag > 1)
                {
                    countdown.AddExtraTime(7f);
                    Debug.Log("残り時間 +7 秒！");
                }
                //  迷路拡大＋リセット
                isResetting = true;
                mazeGen.ExpandMaze(2);
                mazeGen.ResetMazeSafe();
                isResetting = false;
            }

            UpdateGoalText();
        }
    }


    private void UpdateGoalText()
    {
        if (goalText != null)
        {
            goalText.text = $"スコア: {totalScore:F2}";
        }
    }

    private void FinishGame()
    {
        if (finishText != null && !finishText.gameObject.activeSelf)
        {
            finishText.gameObject.SetActive(true);
            finishText.transform.SetAsLastSibling();

            // ✅ スコアを整数に丸めて表示
            int finalScore = Mathf.RoundToInt(totalScore);
            finishText.text = $"FINISH!!\n最終スコア: {finalScore}";

            // ✅ スコア保存（floatで保存してOK）
            PlayerPrefs.SetFloat("FinalScore", totalScore);
            PlayerPrefs.Save();

            Time.timeScale = 0f;
            isGameFinished = true;
            Debug.Log($"ゲーム終了: FINISH!!（最終スコア {totalScore:F2}）");

            // ✅ ボタンを表示
            if (titleButton != null)
            {
                titleButton.gameObject.SetActive(true);
                titleButton.onClick.RemoveAllListeners(); // ← 二重登録防止
                titleButton.onClick.AddListener(() => LoadScene("TitleScene"));
            }

            if (rankingButton != null)
            {
                rankingButton.gameObject.SetActive(true);
                rankingButton.onClick.RemoveAllListeners();
                rankingButton.onClick.AddListener(() => LoadScene("RankingEntryScene"));
            }
        }
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // 時間を戻す
        SceneManager.LoadScene(sceneName);
    }
}
