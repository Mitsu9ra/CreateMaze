using UnityEngine;
using UnityEngine.SceneManagement;

public class InCenter : MonoBehaviour
{
    public void SwitchScene()
    {
        SceneManager.LoadScene("InCenterScene");
    }
    void Update()
    {
        // Enterキー（メインのEnterキー または テンキーのEnterキー）が押された瞬間を検知
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // ボタンが押されたときと同じ処理を呼び出す
            SwitchScene();
        }
    }
}


