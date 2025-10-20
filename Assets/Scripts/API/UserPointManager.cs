using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// ←ここ注意！ Serializable は各クラス1回ずつだけ！
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

public class UserPointManager
{
    private string baseUrl = "http://localhost:5000/api";

    // ✅ ランキング順に全ユーザーのポイントを取得
    public IEnumerator GetAllUserPointsSorted(Action<List<UserPoint>> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/userPoints.php";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                string json = "{\"users\":" + request.downloadHandler.text + "}";
                UserPointList data = JsonUtility.FromJson<UserPointList>(json);

                // 🔽 ソート（ポイント降順）
                data.users.Sort((a, b) => b.point.CompareTo(a.point));

                onSuccess?.Invoke(data.users);
            }
            catch (Exception e)
            {
                onError?.Invoke("JSONパースエラー: " + e.Message);
            }
        }
        else
        {
            onError?.Invoke("通信エラー: " + request.error);
        }
    }


    // ✅ スコア登録（新規 or 更新）
    public IEnumerator UpdateUserPoint(int userId, string userName, int point, Action<UserPoint> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/updatePoint.php";
        string json = $"{{\"userId\":{userId},\"userName\":\"{userName}\",\"point\":{point}}}";

        byte[] body = Encoding.UTF8.GetBytes(json);
        UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");

        Debug.Log("送信内容: " + json); // デバッグ出力

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
