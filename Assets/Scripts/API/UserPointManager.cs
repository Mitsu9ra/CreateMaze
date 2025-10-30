using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[Serializable]
public class UserPoint
{
    public int userId;
    public string userName;
    public int point;
}

[Serializable]
public class UserPointList
{
    public List<UserPoint> users;
}

public class UnsafeCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // ⚠️ 開発中だけ使用。本番では無効にする！
        return true;
    }
}

public class UserPointManager
{
    // LAN 内サーバー
    private string baseUrl = "https://192.168.0.10/api";

    // ✅ ランキング順に全ユーザーのポイントを取得
    public IEnumerator GetAllUserPointsSorted(Action<List<UserPoint>> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/userPoints.php";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 🔥 証明書検証を無視（開発用）
            request.certificateHandler = new UnsafeCertificateHandler();
            request.timeout = 10; // タイムアウト対策

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = "{\"users\":" + request.downloadHandler.text + "}";
                    Debug.Log("レスポンス全文: " + json);

                    UserPointList data = JsonUtility.FromJson<UserPointList>(json);

                    if (data != null && data.users != null)
                    {
                        data.users.Sort((a, b) => b.point.CompareTo(a.point));
                        onSuccess?.Invoke(data.users);
                    }
                    else
                    {
                        onError?.Invoke("データが空です");
                    }
                }
                catch (Exception e)
                {
                    onError?.Invoke("JSONパースエラー: " + e.Message + "\nレスポンス: " + request.downloadHandler.text);
                }
            }
            else
            {
                onError?.Invoke("通信エラー: " + request.error + "\nURL: " + url);
            }
        }
    }

    // ✅ ユーザーポイント登録・更新
    public IEnumerator UpdateUserPoint(int userId, string userName, int point, Action<UserPoint> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/updatePoint.php";
        string json = $"{{\"userId\":{userId},\"userName\":\"{userName}\",\"point\":{point}}}";

        Debug.Log("送信内容: " + json);

        using (UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(body);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
            req.SetRequestHeader("Accept", "application/json");

            // 🔥 証明書検証を無視（開発用）
            req.certificateHandler = new UnsafeCertificateHandler();
            req.timeout = 10;

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    Debug.Log("レスポンス: " + req.downloadHandler.text);
                    UserPoint data = JsonUtility.FromJson<UserPoint>(req.downloadHandler.text);
                    onSuccess?.Invoke(data);
                }
                catch (Exception e)
                {
                    onError?.Invoke("JSONパースエラー: " + e.Message + "\nレスポンス全文: " + req.downloadHandler.text);
                }
            }
            else
            {
                onError?.Invoke("通信エラー: " + req.error + "\nURL: " + url);
            }
        }
    }
}
