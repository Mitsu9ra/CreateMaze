using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Bs : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // 1. Inspector Fields (Public/Serialized)
    // -------------------------------------------------------------------------

    [Header("1. Main Menu Buttons (A(3) + B(2))")]
    public Button[] mainMenuButtons;

    [Header("2. Sub-Menu Panels (Submenu)")]
    public GameObject[] subMenus;

    [Header("3. Event System")]
    public EventSystem eventSystem;

    [Header("4. Executor Reference")]
    [Tooltip("ボタン実行ロジックと設定を保持するExecutorへの参照")]
    public ButtonExecutor buttonExecutor;


    // -------------------------------------------------------------------------
    // 2. Configuration Fields (Private/ReadOnly)
    // -------------------------------------------------------------------------

    // --- グループ設定 ---
    private readonly int[] GroupSizes = { 3, 2 }; // A(3), B(2)

    private int NumGroups;
    private int[] GroupStartIndices;

    // --- 状態 ---
    private int currentGroupIndex = 0;
    private int currentButtonIndexInGroup = 0;
    private int[] lastSelectedIndices; // 各グループの最後に選択していたボタンを記録
    private int lastSubMenuIndex = -1;

    private bool isMenuFocused = false;

    // 入力切り替え用フィールド
    private Vector3 lastMousePosition;
    private bool isMouseActive = true;

    // -------------------------------------------------------------------------
    // 3. Unity Lifecycle Methods
    // -------------------------------------------------------------------------

    void Awake()
    {
        NumGroups = GroupSizes.Length;
        GroupStartIndices = new int[NumGroups];
        int currentStart = 0;

        for (int i = 0; i < NumGroups; i++)
        {
            GroupStartIndices[i] = currentStart;
            currentStart += GroupSizes[i];
        }

        if (currentStart != mainMenuButtons.Length)
        {
            Debug.LogError("Bs: GroupSizesの合計とmainMenuButtonsの数が一致しません。");
            enabled = false;
            return;
        }

        lastSelectedIndices = new int[NumGroups];
        for (int i = 0; i < NumGroups; i++)
            lastSelectedIndices[i] = 0;

        SetupMouseEvents();
    }

    void Start()
    {
        // 初期状態では、無選択状態からスタートするため、すべて非表示にする
        HideAllSubMenus();

        // EventSystemの選択状態をクリア
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // カメラを初期位置に戻す（ButtonExecutorを通して実行）
        if (buttonExecutor != null)
        {
            // -1を渡すことでButtonExecutorにリセットを指示
            buttonExecutor.HandleSelectionZoom(-1);
        }

        lastMousePosition = Input.mousePosition;
        isMenuFocused = false; // 無選択状態でスタート
    }

    void Update()
    {
        bool navigationKeyPressed = false;

        // 1. Escapeキーによる無選択状態への遷移
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuFocused)
            {
                DeselectMenu();
            }
            return;
        }

        // 2. キーボードによるナビゲーション (WASDと矢印キーに対応)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            NavigateGroup(-1);
            navigationKeyPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            NavigateGroup(1);
            navigationKeyPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            NavigateButtonInGroup(-1);
            navigationKeyPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NavigateButtonInGroup(1);
            navigationKeyPressed = true;
        }

        if (navigationKeyPressed)
        {
            isMouseActive = false;

            // 無選択状態からのキー入力の場合、最初のボタンを選択する
            if (!isMenuFocused)
            {
                isMenuFocused = true;

                // 初回選択時は、最初のグループの最後に選択していたボタンに戻る
                currentGroupIndex = 0;
                currentButtonIndexInGroup = lastSelectedIndices[currentGroupIndex];

                // 最初の選択処理を実行
                UpdateSelection();
            }
        }

        // 3. マウス移動のチェック
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            if (!isMouseActive)
            {
                isMouseActive = true;
            }
        }
    }

    // -------------------------------------------------------------------------
    // 4. Mouse Event Setup & Handler (マウスイベント処理)
    // -------------------------------------------------------------------------

    private (int groupIndex, int buttonIndexInGroup) GetGroupIndicesFromGlobal(int globalIndex)
    {
        for (int g = 0; g < NumGroups; g++)
        {
            int groupStart = GroupStartIndices[g];
            int groupEnd = groupStart + GroupSizes[g] - 1;

            if (globalIndex >= groupStart && globalIndex <= groupEnd)
            {
                int buttonIndexInGroup = globalIndex - groupStart;
                return (g, buttonIndexInGroup);
            }
        }
        return (0, 0);
    }

    private void SetupMouseEvents()
    {
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            Button button = mainMenuButtons[i];
            int globalIndex = i;

            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;

            entry.callback.AddListener((data) => { OnButtonPointerEnter(globalIndex); });

            trigger.triggers.Add(entry);
        }
    }

    public void OnButtonPointerEnter(int globalIndex)
    {
        // キーボード操作中でマウスが無効になっている場合は、マウスオーバーを無視
        if (!isMouseActive)
        {
            return;
        }

        (int gIndex, int bIndex) = GetGroupIndicesFromGlobal(globalIndex);

        currentGroupIndex = gIndex;
        currentButtonIndexInGroup = bIndex;

        if (currentGroupIndex == 0)
        {
            lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;
        }

        // マウスオーバーで無選択状態から選択状態へ遷移
        if (!isMenuFocused)
        {
            isMenuFocused = true;
        }

        // 選択状態の更新、サブメニュー表示、ズーム実行を行う
        UpdateSelectionInternal();
        ShowSubMenu(globalIndex);

        ExecuteSelectionZoom(globalIndex);
    }

    // -------------------------------------------------------------------------
    // 5. Navigation Logic (ナビゲーションロジック)
    // -------------------------------------------------------------------------

    private int GetGlobalIndex()
    {
        return GroupStartIndices[currentGroupIndex] + currentButtonIndexInGroup;
    }

    private void NavigateGroup(int direction)
    {
        // 無選択状態からのナビゲーションはUpdate()で処理される
        if (!isMenuFocused) return;

        lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;

        int newGroupIndex = currentGroupIndex + direction;

        if (newGroupIndex >= NumGroups)
            newGroupIndex = 0;
        else if (newGroupIndex < 0)
            newGroupIndex = NumGroups - 1;

        currentGroupIndex = newGroupIndex;

        if (currentGroupIndex == 0)
        {
            int groupSize = GroupSizes[currentGroupIndex];
            currentButtonIndexInGroup = Mathf.Clamp(lastSelectedIndices[currentGroupIndex], 0, groupSize - 1);
        }
        else
        {
            currentButtonIndexInGroup = 1;
        }

        UpdateSelection();
    }

    private void NavigateButtonInGroup(int direction)
    {
        // 無選択状態からのナビゲーションはUpdate()で処理される
        if (!isMenuFocused) return;

        int groupSize = GroupSizes[currentGroupIndex];
        int newButtonIndex = currentButtonIndexInGroup + direction;

        if (newButtonIndex >= groupSize)
            newButtonIndex = 0;
        else if (newButtonIndex < 0)
            newButtonIndex = groupSize - 1;

        currentButtonIndexInGroup = newButtonIndex;

        if (currentGroupIndex == 0)
        {
            lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;
        }

        UpdateSelection();
    }

    // -------------------------------------------------------------------------
    // 6. Selection & Action (選択と実行) - 外部公開用
    // -------------------------------------------------------------------------

    private void UpdateSelection()
    {
        if (!isMenuFocused) return; // 無選択状態の場合は処理しない

        UpdateSelectionInternal();
        // キーボード操作時は、isMouseActiveの状態に関わらずサブメニューとズームを実行
        ShowSubMenu(GetGlobalIndex());

        ExecuteSelectionZoom(GetGlobalIndex());
    }

    // 無選択状態に戻すロジック
    private void DeselectMenu()
    {
        isMenuFocused = false;
        HideAllSubMenus();

        // EventSystemの選択状態をクリア
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }

        // カメラをデフォルトターゲットに戻す (-1を渡すとButtonExecutorでリセットされる)
        ExecuteSelectionZoom(-1);
    }

    private void ExecuteSelectionZoom(int globalIndex)
    {
        if (buttonExecutor != null)
        {
            buttonExecutor.HandleSelectionZoom(globalIndex);
        }
    }

    private void UpdateSelectionInternal()
    {
        int globalIndex = GetGlobalIndex();
        if (globalIndex >= 0 && globalIndex < mainMenuButtons.Length)
        {
            // EventSystemの選択を更新
            eventSystem.SetSelectedGameObject(mainMenuButtons[globalIndex].gameObject);
        }
    }

    public int GetSelectedButtonGlobalIndex()
    {
        // 無選択状態の場合は-1を返す
        return isMenuFocused ? GetGlobalIndex() : -1;
    }

    public GameObject GetSelectedButtonGameObject()
    {
        int globalIndex = GetGlobalIndex();
        if (isMenuFocused && globalIndex >= 0 && globalIndex < mainMenuButtons.Length)
        {
            return mainMenuButtons[globalIndex].gameObject;
        }
        return null;
    }

    // -------------------------------------------------------------------------
    // 7. Sub-Menu Control (サブメニュー制御)
    // -------------------------------------------------------------------------

    private void ShowSubMenu(int index)
    {
        if (lastSubMenuIndex != -1 && lastSubMenuIndex < subMenus.Length && subMenus[lastSubMenuIndex] != null)
        {
            subMenus[lastSubMenuIndex].SetActive(false);
        }

        if (index >= 0 && index < subMenus.Length && subMenus[index] != null)
        {
            subMenus[index].SetActive(true);
            lastSubMenuIndex = index;
        }
        else
        {
            lastSubMenuIndex = -1;
        }
    }

    private void HideAllSubMenus()
    {
        foreach (GameObject subMenu in subMenus)
        {
            if (subMenu != null)
            {
                subMenu.SetActive(false);
            }
        }
        lastSubMenuIndex = -1;
    }
}