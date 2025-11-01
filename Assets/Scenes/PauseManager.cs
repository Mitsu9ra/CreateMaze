using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    // ポーズ状態をグローバルに参照するための静的プロパティ
    public static bool IsPaused { get; private set; } = false;

    // Inspectorでポーズ画面のUIパネルを設定
    public GameObject pausePanel;

    // 必須: Inspectorで設定する2つのボタン参照 (上下に並んでいると想定)
    public Button resumeButton; // 上: 再開するボタン
    public Button titleButton;  // 下: TitleSceneへ遷移するボタン

    // UIナビゲーション用の変数
    private Vector3 originalScaleResume; // 再開ボタンの初期スケール
    private Vector3 originalScaleTitle;  // タイトルボタンの初期スケール
    private readonly float scaleFactor = 1.1f;

    // 現在選択されているボタンを追跡する内部フラグ
    private Button currentSelectedButton;

    void Start()
    {
        // 初期状態としてポーズ画面は非表示
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // 初期スケールを保存
        if (resumeButton != null) originalScaleResume = resumeButton.transform.localScale;
        if (titleButton != null) originalScaleTitle = titleButton.transform.localScale;

        // ボタンに処理を登録
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (titleButton != null)
        {
            titleButton.onClick.RemoveAllListeners();
            titleButton.onClick.AddListener(() => LoadScene("TitleScene"));
        }

        // 🚨 修正点 1: 初期選択ボタンを titleButton に設定
        currentSelectedButton = titleButton;
        SetButtonScale(resumeButton, false, originalScaleResume);
        SetButtonScale(titleButton, true, originalScaleTitle);
    }

    void Update()
    {
        // 1. Escapeキーによるポーズ切り替え
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // 2. ポーズ中のボタン操作
        if (IsPaused)
        {
            HandleButtonNavigation();
        }
    }

    // ゲームをポーズする
    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Debug.Log("Game Paused");

        // 🚨 修正点 1: ポーズ画面表示時も titleButton を選択状態にする
        SelectButton(titleButton);
        // EventSystemにも設定
        if (titleButton != null)
        {
            EventSystem.current.SetSelectedGameObject(titleButton.gameObject);
        }
    }

    // ゲームを再開する
    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Debug.Log("Game Resumed");
    }

    // シーン遷移用のヘルパーメソッド
    private void LoadScene(string sceneName)
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        SceneManager.LoadScene(sceneName);
    }


    // ----------------------------------------------------
    // UIナビゲーションロジック (上下ループ対応)
    // ----------------------------------------------------

    private void HandleButtonNavigation()
    {
        // 上下キーまたはW/Sキーの入力をチェック
        bool isDownKey = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool isUpKey = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool isExecuteKey = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);

        // 🚨 修正点 2: 上下どちらのキーでも、常に反対側のボタンを選択する (ループ)
        if (isDownKey || isUpKey)
        {
            Button nextButton = null;

            // 2つのボタンしかないため、どちらかのキーが押されたら、常に反対側のボタンを選択する
            if (currentSelectedButton == resumeButton)
            {
                nextButton = titleButton; // 上から下へ、または上から下へループ
            }
            else if (currentSelectedButton == titleButton)
            {
                nextButton = resumeButton; // 下から上へ、または下から上へループ
            }

            if (nextButton != null)
            {
                // スケールをリセットし、新しいボタンを選択
                SetButtonScale(currentSelectedButton, false, (currentSelectedButton == resumeButton ? originalScaleResume : originalScaleTitle));
                SelectButton(nextButton);
                // 選択をEventSystemにも設定
                EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
            }
        }
        else if (isExecuteKey)
        {
            // 実行キーが押されたら、現在のボタンの処理を実行
            if (currentSelectedButton != null)
            {
                currentSelectedButton.onClick.Invoke();
            }
        }
    }

    private void SelectButton(Button buttonToSelect)
    {
        if (buttonToSelect != null)
        {
            currentSelectedButton = buttonToSelect;
            // 新しいボタンを拡大表示
            SetButtonScale(buttonToSelect, true, (buttonToSelect == resumeButton ? originalScaleResume : originalScaleTitle));
        }
    }

    private void SetButtonScale(Button button, bool isSelected, Vector3 originalScale)
    {
        if (button == null) return;

        if (isSelected)
        {
            button.transform.localScale = originalScale * scaleFactor;
        }
        else
        {
            button.transform.localScale = originalScale;
        }
    }
}