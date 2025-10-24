using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;


[System.Serializable]
public class AuthResponse
{
    public string status;
    public string message;
}

public class AuthManager : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] TMP_Text messageText;

    public void OnRegisterClicked()
    {
        StartCoroutine(RegisterCoroutine());
    }

    public void OnLoginClicked()
    {
        StartCoroutine(LoginCoroutine());
    }

    IEnumerator RegisterCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailField.text);
        form.AddField("password", passwordField.text);

        yield return ApiClient.PostRequest("register.php", form,
            (res) =>
            {
                var data = JsonConvert.DeserializeObject<AuthResponse>(res);
                messageText.text = data.message;
            },
            (err) => messageText.text = "通信エラー: " + err
        );
    }

    IEnumerator LoginCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailField.text);
        form.AddField("password", passwordField.text);

        yield return ApiClient.PostRequest("login.php", form,
            (res) =>
            {
                var data = JsonConvert.DeserializeObject<AuthResponse>(res);
                if (data.status == "ok")
                {
                    messageText.text = "ログイン成功！";
                    PlayerPrefs.SetString("userEmail", emailField.text);
                    PlayerPrefs.SetInt("isLoggedIn", 1);
                    PlayerPrefs.Save();

                    // 次のシーンへ（例：ランキング入力）
                    UnityEngine.SceneManagement.SceneManager.LoadScene("RankingEntryScene");
                }
                else
                {
                    messageText.text = data.message;
                }
            },
            (err) => messageText.text = "通信エラー: " + err
        );
    }
}
