using UnityEngine;
using UnityEngine.UI;

public class RankingEntryUI : MonoBehaviour
{
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
        sendButton.onClick.AddListener(OnSendClicked);
    }

    void OnSendClicked()
    {
        string userName = nameInput.text;
        int point = int.Parse(scoreInput.text);

        // 仮のID（実際はログインIDなどで管理）
        int userId = Random.Range(1, 9999);

        StartCoroutine(api.UpdateUserPoint(userId, userName, point,
            (res) =>
            {
                Debug.Log($"登録完了: {res.userName} = {res.point}");
                UnityEngine.SceneManagement.SceneManager.LoadScene("RankingScene");
            },
            (err) => Debug.LogError(err)
        ));
    }
}
