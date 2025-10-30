using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_Text messageText;

    private string baseUrl = "http://192.168.0.15/api/";

    public void OnRegisterClicked() => StartCoroutine(RegisterCoroutine());
    public void OnBackToLoginClicked() => UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");

    IEnumerator RegisterCoroutine()
    {
        messageText.text = "送信中...";

        WWWForm form = new WWWForm();
        form.AddField("email", emailField.text);
        form.AddField("password", passwordField.text);

        using (var req = UnityWebRequest.Post(baseUrl + "register.php", form))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                messageText.text = "通信エラー: " + req.error;
                yield break;
            }

            var res = JsonConvert.DeserializeObject<AuthResponse>(req.downloadHandler.text);
            messageText.text = res.message;
        }
    }
}
