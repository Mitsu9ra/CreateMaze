using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ClickChange : MonoBehaviour
{
    [SerializeField] private string otherScene = "TitleScene";
    [SerializeField] private float autoChangeTime = 15f; // 自動遷移までの秒数

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // 時間経過で自動遷移
        if (timer >= autoChangeTime)
        {
            FadeManager.Instance.LoadScene(otherScene);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // UI（ボタン）の上なら return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 画面の他クリック
            FadeManager.Instance.LoadScene(otherScene);
        }
    }

    // ボタン1用
    public void OnButton1Click()
    {
        FadeManager.Instance.LoadScene("RankingEntryScene");
    }

    // ボタン2用
    public void OnButton2Click()
    {
        FadeManager.Instance.LoadScene("RankingScene");
    }
}


