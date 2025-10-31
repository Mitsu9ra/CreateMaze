using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class ButtonZoomSetting
{
    [Header("1. Selection Zoom")]
    [Tooltip("ボタン選択時（Enterキーやマウスオーバー時）にズームを有効にするか")]
    public bool shouldSelectionZoom = false;

    [Tooltip("ボタン選択時のズームターゲットオブジェクト (nullの場合はズームしない)")] // ⭐ 新規追加
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
}

public class ButtonExecutor : MonoBehaviour
{
    [Header("Controller References")]
    public Bs buttonController;
    public CameraMover cameraMover;

    [Header("Action Settings")]
    public ButtonZoomSetting[] buttonSettings;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteAction();
        }
    }

    public void HandleSelectionZoom(int globalIndex)
    {
        // cameraMoverが設定されているか確認
        if (cameraMover == null) return;

        // 設定配列の範囲外チェック
        if (globalIndex < 0 || globalIndex >= buttonSettings.Length)
        {
            cameraMover.ResetToDefaultTarget();
            return;
        }

        ButtonZoomSetting setting = buttonSettings[globalIndex];

        // 選択時ズームが有効な場合
        if (setting.shouldSelectionZoom)
        {
            // ⭐ ズームターゲットを selectionTargetObject に設定する
            if (setting.selectionTargetObject != null)
            {
                Transform targetTransform = setting.selectionTargetObject; // 選択時ズーム専用ターゲット
                float zoomSize = setting.selectionZoomSize;

                // カメラをターゲットと選択時ズームサイズで移動させる
                cameraMover.MoveToSelectionTarget(targetTransform, zoomSize);
            }
            else
            {
                // selectionTargetObjectが設定されていない場合はデフォルトに戻す
                Debug.LogWarning($"ButtonExecutor: Button index {globalIndex} has selection zoom enabled but no selectionTargetObject assigned. Resetting camera.");
                cameraMover.ResetToDefaultTarget();
            }
        }
        else
        {
            // 選択時ズームが無効な場合は、デフォルトターゲットに戻す
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

        // 3. アクション（シーン遷移など）の実行
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