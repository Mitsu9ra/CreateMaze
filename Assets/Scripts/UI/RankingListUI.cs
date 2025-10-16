using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingListUI : MonoBehaviour
{
    public Transform contentParent;   // ScrollViewのContent部分
    public GameObject rankingItemPrefab; // 名前とスコアを表示するプレハブ
    private UserPointManager api;

    void Start()
    {
        api = new UserPointManager();
        StartCoroutine(api.GetAllUserPointsSorted(
            (users) => DisplayRanking(users),
            (error) => Debug.LogError(error)
        ));
    }

    void DisplayRanking(List<UserPoint> users)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        int rank = 1;
        foreach (var user in users)
        {
            GameObject item = Instantiate(rankingItemPrefab, contentParent);
            item.GetComponentInChildren<Text>().text = $"{rank}. {user.userName} - {user.point}";
            rank++;
        }
    }
}
