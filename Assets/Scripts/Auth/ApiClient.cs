using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public static class ApiClient
{
    private static string baseUrl = "http://192.168.0.15/api/";

    public static IEnumerator PostRequest(string endpoint, WWWForm form, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl + endpoint, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke(www.downloadHandler.text);
            else
                onError?.Invoke(www.error);
        }
    }
}
