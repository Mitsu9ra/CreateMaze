using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RankingEntryUI : MonoBehaviour
{
    [Header("UI 参照")]
    public InputField nameInput;
    public InputField scoreInput;
    public Button sendButton;

    private UserPointManager api;

    void Start()
    {
        api = new UserPointManager();

        if (sendButton == null) Debug.LogError("❌ sendButton が設定されていません");
        if (nameInput == null) Debug.LogError("❌ nameInput が設定されていません");
        if (scoreInput == null) Debug.LogError("❌ scoreInput が設定されていません");

        // ✅ スコアを PlayerPrefs から読み込み
        float finalScore = PlayerPrefs.GetFloat("FinalScore", 0f);
        scoreInput.text = Mathf.RoundToInt(finalScore).ToString(); // 整数化
        scoreInput.readOnly = true; // 手入力禁止

        // ✅ 名前を自動入力（例: メール or 前回入力名）
        string lastName = PlayerPrefs.GetString("LastPlayerName", "");
        if (!string.IsNullOrEmpty(lastName))
            nameInput.text = lastName;

        sendButton.onClick.AddListener(OnSendClicked);
    }

    void OnSendClicked()
    {
        string userName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(userName))
        {
            Debug.LogWarning("⚠ 名前が入力されていません。");
            return;
        }

        if (!int.TryParse(scoreInput.text, out int point))
        {
            Debug.LogError("❌ スコアの数値変換に失敗しました。");
            return;
        }

        //  仮のユーザーID生成
        int userId = userName.GetHashCode();

        //  登録情報を保存しておく
        PlayerPrefs.SetString("LastPlayerName", userName);
        PlayerPrefs.Save();

        //  スコア送信処理
        StartCoroutine(api.UpdateUserPoint(
            userId, userName, point,
            (res) =>
            {
                Debug.Log($" 登録完了: {res.userName} = {res.point}");
                SceneManager.LoadScene("RankingScene");
            },
            (err) =>
            {
                Debug.LogError($"❌ 登録失敗: {err}");
            }
        ));
    }
}
