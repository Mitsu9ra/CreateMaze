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
        string userName = PlayerPrefs.GetString("userEmail", "Guest");
        int point = int.Parse(scoreInput.text);

        int userId = userName.GetHashCode(); // 仮の一意ID（DB側で管理でもOK）

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
