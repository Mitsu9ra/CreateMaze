using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class ButtonZoomSetting
{
    [Header("1. Selection Zoom")]
    [Tooltip("ボタン選択時（Enterキーやマウスオーバー時）にズームを有効にするか")]
    public bool shouldSelectionZoom = false;

    [Tooltip("ボタン選択時のズームターゲットオブジェクト (nullの場合はズームしない)")]
    public Transform selectionTargetObject;

    [Tooltip("ボタン選択時のorthographicSize (selectionZoomEnabledがtrueの場合に適用)")]
    public float selectionZoomSize = 5.0f;

    [Header("2. Execution Zoom/Target")]
    [Tooltip("カメラを移動・ズームさせるかどうか (falseの場合、ズーム関連の処理はスキップされます)")]
    public bool shouldZoom = true;

    [Tooltip("このボタンが押されたときにズームするターゲットオブジェクト")]
    public Transform targetObject; // 実行時ズームターゲット

    [Tooltip("ズーム後のカメラのorthographicSize (小さいほど拡大)")]
    public float zoomSize = 2.0f;

    [Header("3. Final Action")]
    [Tooltip("ズーム後にシーン遷移などの最終アクションを行うかどうか")]
    public bool shouldExecuteAction = false;

    [Tooltip("shouldExecuteActionがtrueの場合のシーン名")]
    public string sceneName = "";

    // ⭐ 4. Cross-Scene Flag (int値のみ)
    [Header("4. Cross-Scene Flag (int値のみ)")]
    [Tooltip("他のシーンで使用するフラグに設定する値（int型）。キーはスクリプト内で固定されています。")]
    public int crossSceneFlagInt = 0;
}

public class ButtonExecutor : MonoBehaviour
{
    [Header("Controller References")]
    public Bs buttonController;
    public CameraMover cameraMover;

    [Header("Action Settings")]
    public ButtonZoomSetting[] buttonSettings;

    // ⭐ クロスシーンフラグの固定キーを定義
    private const string CROSS_SCENE_FLAG_KEY = "MENU_ACTION_FLAG";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteAction();
        }
    }

    public void HandleSelectionZoom(int globalIndex)
    {
        if (cameraMover == null) return;

        if (globalIndex < 0 || globalIndex >= buttonSettings.Length)
        {
            cameraMover.ResetToDefaultTarget();
            return;
        }

        ButtonZoomSetting setting = buttonSettings[globalIndex];

        if (setting.shouldSelectionZoom)
        {
            if (setting.selectionTargetObject != null)
            {
                Transform targetTransform = setting.selectionTargetObject;
                float zoomSize = setting.selectionZoomSize;

                cameraMover.MoveToSelectionTarget(targetTransform, zoomSize);
            }
            else
            {
                Debug.LogWarning($"ButtonExecutor: Button index {globalIndex} has selection zoom enabled but no selectionTargetObject assigned. Resetting camera.");
                cameraMover.ResetToDefaultTarget();
            }
        }
        else
        {
            cameraMover.ResetToDefaultTarget();
        }
    }

    public void OnButtonClickExecute()
    {
        ExecuteAction();
    }

    private void ExecuteAction()
    {
        if (buttonController == null || cameraMover == null)
        {
            Debug.LogError("ButtonController (Bs) または CameraMover が設定されていません。Inspectorを確認してください。");
            return;
        }

        int globalIndex = buttonController.GetSelectedButtonGlobalIndex();

        if (globalIndex < 0 || globalIndex >= buttonSettings.Length)
        {
            Debug.LogWarning($"Index {globalIndex} の設定が buttonSettings に見つかりません。配列サイズを確認してください。");
            return;
        }

        ButtonZoomSetting setting = buttonSettings[globalIndex];
        float waitTime = 0f;

        // 2. ズームの実行 (実行時設定を使用)
        if (setting.shouldZoom)
        {
            if (setting.targetObject != null)
            {
                cameraMover.MoveToExecutionTarget(setting.targetObject, setting.zoomSize);
                waitTime = cameraMover.duration;
                Debug.Log($"Index {globalIndex} が実行され、ターゲット {setting.targetObject.name} にカメラが移動・ズームしました。");
            }
            else
            {
                Debug.LogWarning($"Index {globalIndex} は実行時ズーム有効ですが、ターゲットが設定されていません。");
            }
        }

        // ⭐ 3. クロスシーンフラグの設定 (固定キーを使用)
        // 値が0であっても、設定された値を常にPlayerPrefsに保存します。
        PlayerPrefs.SetInt(CROSS_SCENE_FLAG_KEY, setting.crossSceneFlagInt);
        PlayerPrefs.Save();
        Debug.Log($"Cross-Scene Flag Set: Key='{CROSS_SCENE_FLAG_KEY}', Value={setting.crossSceneFlagInt}");

        // 4. アクション（シーン遷移など）の実行
        if (setting.shouldExecuteAction && !string.IsNullOrEmpty(setting.sceneName))
        {
            StartCoroutine(WaitAndExecuteFinalAction(waitTime, setting.sceneName));
        }
        else if (setting.shouldExecuteAction && string.IsNullOrEmpty(setting.sceneName))
        {
            Debug.LogWarning($"Index {globalIndex} はアクション実行が有効ですが、シーン名が設定されていません。");
        }
    }

    private IEnumerator WaitAndExecuteFinalAction(float waitTime, string sceneName)
    {
        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        SceneManager.LoadScene(sceneName);
    }
}