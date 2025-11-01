using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public static int flag = 0;
    // Inspectorから設定できるように、2つのボタンを公開
    public Button buttonA;
    public Button buttonB;

    // スケール変更用変数
    private Vector3 originalScaleA;
    private Vector3 originalScaleB;
    private readonly float scaleFactor = 1.1f; // 10%拡大 (1.1倍)

    // Canvasコンポーネントを保持するフィールド
    private Canvas canvas;

    // 直前のフレームでホバーされていたボタンを記憶 (マウスEnter判定用)
    private Button previousHoveredButton = null;

    void Start()
    {
        // 起動時にボタンの現在のスケールを保存
        if (buttonA != null) originalScaleA = buttonA.transform.localScale;
        if (buttonB != null) originalScaleB = buttonB.transform.localScale;

        // 親または自身からCanvasコンポーネントを取得
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("TitleスクリプトはCanvasの子オブジェクトにアタッチされている必要があります。");
            return;
        }

        // 初期状態でButtonAを選択状態にする
        SelectButton(buttonA);
    }

    // 毎フレーム呼び出される関数
    void Update()
    {
        GameObject currentSelection = EventSystem.current.currentSelectedGameObject;

        // ★後勝ち同期ロジック（マウス優先）★
        Button currentHoveredButton = GetCurrentHoveredButton();

        // 状態変化検出: マウスが新しいボタンの上に入った瞬間
        if (currentHoveredButton != null && currentHoveredButton != previousHoveredButton)
        {
            // マウス入力がキーボード選択に勝つ: 選択状態をマウスに同期
            SelectButton(currentHoveredButton);
        }
        // ホバー状態の記憶を更新
        previousHoveredButton = currentHoveredButton;

        // ★キーボード操作ロジック（後勝ち）★
        bool isKeyDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) ||
                         Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);

        if (isKeyDown)
        {
            // キー入力がマウス選択に勝つ: 選択状態をキーボードで操作したボタンに設定
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (currentSelection == buttonA.gameObject) SelectButton(buttonB);
                else if (currentSelection == buttonB.gameObject) SelectButton(buttonA);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (currentSelection == buttonB.gameObject) SelectButton(buttonA);
                else if (currentSelection == buttonA.gameObject) SelectButton(buttonB);
            }
        }

        // ボタンのスケールと選択状態の制御
        // ★修正後: 選択状態（EventSystem.current.currentSelectedGameObject）のみに基づいて拡大を決定
        SetButtonScale(buttonA, currentSelection == buttonA.gameObject, originalScaleA);
        SetButtonScale(buttonB, currentSelection == buttonB.gameObject, originalScaleB);

        // キー入力によるボタン実行
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSelection == buttonA.gameObject)
            {
                HandleButtonAAction();
            }
            else if (currentSelection == buttonB.gameObject)
            {
                HandleButtonBAction();
            }
        }
    }

    private Button GetCurrentHoveredButton()
    {
        Camera cam = (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                     ? canvas.worldCamera
                     : null;

        if (buttonA != null && RectTransformUtility.RectangleContainsScreenPoint(buttonA.GetComponent<RectTransform>(), Input.mousePosition, cam))
        {
            return buttonA;
        }
        if (buttonB != null && RectTransformUtility.RectangleContainsScreenPoint(buttonB.GetComponent<RectTransform>(), Input.mousePosition, cam))
        {
            return buttonB;
        }
        return null;
    }

    // ボタンのスケールと選択状態を設定するヘルパー関数
    private void SetButtonScale(Button button, bool isSelected, Vector3 originalScale)
    {
        if (button == null || canvas == null) return;

        // ★修正点：拡大ロジックをEventSystemの選択状態のみに依存させる★
        // マウスオーバーの判定は、ここでは視覚的なハイライトには使用しない。
        // isSelected (EventSystemで選択されている状態) がtrueの場合のみ拡大する。
        if (isSelected)
        {
            button.transform.localScale = originalScale * scaleFactor;
        }
        else
        {
            // 選択されていない場合は元のサイズに戻す
            button.transform.localScale = originalScale;
        }
    }

    // ボタンを選択状態にするヘルパー関数
    private void SelectButton(Button buttonToSelect)
    {
        if (buttonToSelect != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }
    }

    /// <summary>
    /// Aボタンが押されたときに実行される処理
    /// </summary>
   
        
public void HandleButtonAAction()
    {
        Debug.Log(buttonA.gameObject.name + "おされました");
        const string FLAG_KEY = "MENU_ACTION_FLAG";
        int flagValue = PlayerPrefs.GetInt(FLAG_KEY, 0);

        Debug.Log($"[Title] PlayerPrefsから読み込んだ難易度フラグ値: {flagValue}");

        switch (flagValue)
        {
            case 1:
                Title.flag = 1;
                break;
            case 2:
                Title.flag = 2;
                break;
            case 3:
                Title.flag = 3;
                break;
            default:
                break;
        }
        Debug.Log($"[Title] Title.flag に設定した最終値: {Title.flag}");
        SceneManager.LoadScene("MazeScene");
    }

    /// <summary>
    /// Bボタンが押されたときに実行される処理
    /// </summary>
    public void HandleButtonBAction()
    {
        Debug.Log(buttonB.gameObject.name + "おされました");
        SceneManager.LoadScene("InCenterScene");
    }
}