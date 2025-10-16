using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// ←ここ注意！ Serializable は各クラス1               回ずつだけ！
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
        string url = $"{baseUrl}/userPoints";
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
        string url = $"{baseUrl}/updatePoint";
        string json = $"{{\"userId\":{userId},\"userName\":\"{userName}\",\"point\":{point}}}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            try
            {
                UserPoint data = JsonUtility.FromJson<UserPoint>(req.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            catch (Exception e)
            {
                onError?.Invoke("JSONパースエラー: " + e.Message);
            }
        }
        else
        {
            onError?.Invoke("通信エラー: " + req.error);
        }
    }
}
