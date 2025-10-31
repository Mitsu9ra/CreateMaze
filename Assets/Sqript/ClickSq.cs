using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理のために必要
using UnityEngine.UI;

public class ClickSq : MonoBehaviour
{
    // ボタンコンポーネントをアタッチするためのpublic変数
    public Button button1;
    public Button button2;
    public Button button3;

    void Start()
    {
        // 各ボタンに、専用のクリックイベントハンドラを割り当てる
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
        button3.onClick.AddListener(OnButton3Click);
    }

    // ボタン1がクリックされたときの処理
    void OnButton1Click()
    {
        Debug.Log("ボタン1がクリックされました！");
        // メッセージを表示した後、シーンをロードする
        SceneManager.LoadScene("TutoriialScene");
    }

    // ボタン2がクリックされたときの処理
    void OnButton2Click()
    {
        Debug.Log("ボタン2がクリックされました！");
    }

    // ボタン3がクリックされたときの処理
    void OnButton3Click()
    {
        Debug.Log("ボタン3がクリックされました！");
    }
}