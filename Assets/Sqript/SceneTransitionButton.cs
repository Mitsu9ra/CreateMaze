using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionButton : MonoBehaviour
{
    // Inspectorから設定する移動先のシーン名 (例: GameScene, TitleSceneなど)
    [SerializeField]
    private string nextSceneName = "InCenterScene";

    // フェードにかける時間 (FadeManagerの実装に依存)
    [SerializeField]
    private float fadeDuration = 1.0f;

    /// <summary>
    /// ボタンの On Click イベントから呼び出すメソッド
    /// </summary>
    public void OnButtonClick()
    {
        // FadeManagerのインスタンスを取得し、シーン移動を呼び出す
        if (FadeManager.Instance != null)
        {
            // LoadScene(移動先のシーン名, フェードにかかる時間)
            FadeManager.Instance.LoadScene(nextSceneName, fadeDuration);
        }
        else
        {
            Debug.LogError("FadeManagerのインスタンスが見つかりません。最初のシーンに配置されているか確認してください。");
        }
    }
    void Update()
    {
        // Enterキー（メインのEnterキー または テンキーのEnterキー）が押された瞬間を検知
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // ボタンが押されたときと同じ処理を呼び出す
            OnButtonClick();
        }
    }
}