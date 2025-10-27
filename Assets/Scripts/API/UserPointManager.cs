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

public class UserPointManager
{
    // ★ ここをLAN内のPC IPに変更
    private string baseUrl = "http://10.18.20.27/api";

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
                // PHP側が配列だけ返すので、JSONUtilityで読める形にする
                string json = "{\"users\":" + request.downloadHandler.text + "}";
                Debug.Log("レスポンス全文: " + json);

                UserPointList data = JsonUtility.FromJson<UserPointList>(json);

                if (data != null && data.users != null)
                {
                    // 🔽 ポイント降順にソート
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
