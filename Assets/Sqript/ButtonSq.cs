//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using UnityEngine.EventSystems; // EventTriggerを使用するために必要

//public class ButtonSq : MonoBehaviour
//{
//    // -------------------------------------------------------------------------
//    // 1. Inspector Fields (Public/Serialized)
//    // -------------------------------------------------------------------------

//    [Header("1. Main Menu Buttons (A(3) + B(2))")]
//    public Button[] mainMenuButtons;

//    [Header("2. Sub-Menu Panels (Submenu)")]
//    public GameObject[] subMenus;

//    [Header("3. Event System")]
//    public EventSystem eventSystem;

//    // -------------------------------------------------------------------------
//    // 2. Configuration Fields (Private/ReadOnly)
//    // -------------------------------------------------------------------------

//    // --- グループ設定 ---
//    private readonly int[] GroupSizes = { 3, 2 }; // A(3), B(2)

//    private int NumGroups;
//    private int[] GroupStartIndices;

//    // --- 状態 ---
//    private int currentGroupIndex = 0;
//    private int currentButtonIndexInGroup = 0;
//    private int[] lastSelectedIndices; // 各グループの最後に選択していたボタンを記録
//    private int lastSubMenuIndex = -1;

//    // -------------------------------------------------------------------------
//    // 3. Unity Lifecycle Methods
//    // -------------------------------------------------------------------------

//    void Awake()
//    {
//        NumGroups = GroupSizes.Length;
//        GroupStartIndices = new int[NumGroups];
//        int currentStart = 0;

//        for (int i = 0; i < NumGroups; i++)
//        {
//            GroupStartIndices[i] = currentStart;
//            currentStart += GroupSizes[i];
//        }

//        if (currentStart != mainMenuButtons.Length)
//        {
//            Debug.LogError("ButtonSq: GroupSizesの合計とmainMenuButtonsの数が一致しません。");
//            enabled = false;
//            return;
//        }

//        lastSelectedIndices = new int[NumGroups];
//        for (int i = 0; i < NumGroups; i++)
//            lastSelectedIndices[i] = 0;

//        // マウスイベントの設定を初期化時に実行
//        SetupMouseEvents();
//    }

//    void Start()
//    {
//        HideAllSubMenus();

//        if (mainMenuButtons.Length > 0 && eventSystem != null)
//        {
//            // B項目の初期状態がRankになるようにStart時の初期化を調整（オプション）
//            // currentGroupIndex = 0;
//            // currentButtonIndexInGroup = 0;

//            UpdateSelectionInternal();
//            ShowSubMenu(GetGlobalIndex());
//        }
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            NavigateGroup(-1);
//        }
//        else if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            NavigateGroup(1);
//        }
//        else if (Input.GetKeyDown(KeyCode.LeftArrow))
//        {
//            NavigateButtonInGroup(-1);
//        }
//        else if (Input.GetKeyDown(KeyCode.RightArrow))
//        {
//            NavigateButtonInGroup(1);
//        }

//        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
//        {
//            ExecuteSelectedButtonAction();
//        }
//    }

//    // -------------------------------------------------------------------------
//    // 4. Mouse Event Setup & Handler (マウスイベント処理)
//    // -------------------------------------------------------------------------

//    // ヘルパー：グローバルインデックスからグループインデックスとグループ内インデックスを取得
//    private (int groupIndex, int buttonIndexInGroup) GetGroupIndicesFromGlobal(int globalIndex)
//    {
//        for (int g = 0; g < NumGroups; g++)
//        {
//            int groupStart = GroupStartIndices[g];
//            int groupEnd = groupStart + GroupSizes[g] - 1;

//            if (globalIndex >= groupStart && globalIndex <= groupEnd)
//            {
//                int buttonIndexInGroup = globalIndex - groupStart;
//                return (g, buttonIndexInGroup);
//            }
//        }
//        // 範囲外の場合のフォールバック
//        return (0, 0);
//    }

//    // 全ボタンにEventTriggerを設定
//    private void SetupMouseEvents()
//    {
//        for (int i = 0; i < mainMenuButtons.Length; i++)
//        {
//            Button button = mainMenuButtons[i];
//            int globalIndex = i;

//            // EventTriggerコンポーネントを追加または取得
//            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
//            if (trigger == null)
//            {
//                trigger = button.gameObject.AddComponent<EventTrigger>();
//            }

//            // PointerEnterイベントのリスナーを設定
//            EventTrigger.Entry entry = new EventTrigger.Entry();
//            entry.eventID = EventTriggerType.PointerEnter;

//            // マウスがボタンに乗ったらOnButtonPointerEnterを呼び出すように設定
//            entry.callback.AddListener((data) => { OnButtonPointerEnter(globalIndex); });

//            trigger.triggers.Add(entry);
//        }
//    }

//    // --- マウスがボタンに乗った時の処理 ---
//    public void OnButtonPointerEnter(int globalIndex)
//    {
//        // 1. グローバルインデックスをグループインデックスに変換
//        (int gIndex, int bIndex) = GetGroupIndicesFromGlobal(globalIndex);

//        // 2. 状態変数を更新し、キーボード選択と同期
//        currentGroupIndex = gIndex;
//        currentButtonIndexInGroup = bIndex;

//        // 3. A項目(グループ0)の場合のみ、この状態を記憶する
//        if (currentGroupIndex == 0)
//        {
//            lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;
//        }

//        // 4. Unity EventSystemでフォーカスをセット（マウス選択をキーボード選択と同期）
//        // 5. サブメニューを表示
//        UpdateSelectionInternal();
//        ShowSubMenu(globalIndex);
//    }

//    // -------------------------------------------------------------------------
//    // 5. Navigation Logic (ナビゲーションロジック)
//    // -------------------------------------------------------------------------

//    private int GetGlobalIndex()
//    {
//        return GroupStartIndices[currentGroupIndex] + currentButtonIndexInGroup;
//    }

//    // --- グループ切り替え（上下キー） ---
//    private void NavigateGroup(int direction)
//    {
//        // 1. 現在の選択状態を記憶（全グループで実行）
//        lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;

//        int newGroupIndex = currentGroupIndex + direction;

//        // Wrap-around
//        if (newGroupIndex >= NumGroups)
//            newGroupIndex = 0;
//        else if (newGroupIndex < 0)
//            newGroupIndex = NumGroups - 1;

//        currentGroupIndex = newGroupIndex;

//        // 2. 記憶していたインデックスを復元（グループによって処理を分ける）
//        if (currentGroupIndex == 0) // A項目 (グループ 0) の場合
//        {
//            // 記憶していたインデックスを復元
//            int groupSize = GroupSizes[currentGroupIndex];
//            currentButtonIndexInGroup = Mathf.Clamp(lastSelectedIndices[currentGroupIndex], 0, groupSize - 1);
//        }
//        else // B項目 (グループ 1) やその他のグループの場合
//        {
//            // B項目 (グループ1) は常に Rank (インデックス 1) を選択する
//            currentButtonIndexInGroup = 1;
//        }

//        UpdateSelection();
//    }

//    // --- グループ内のボタン切り替え（左右キー） ---
//    private void NavigateButtonInGroup(int direction)
//    {
//        int groupSize = GroupSizes[currentGroupIndex];
//        int newButtonIndex = currentButtonIndexInGroup + direction;

//        // Wrap-around
//        if (newButtonIndex >= groupSize)
//            newButtonIndex = 0;
//        else if (newButtonIndex < 0)
//            newButtonIndex = groupSize - 1;

//        currentButtonIndexInGroup = newButtonIndex;

//        // グループ 0 の場合にのみ、最後に選択した状態を記憶する
//        if (currentGroupIndex == 0)
//        {
//            lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;
//        }
//        // グループ 1 の場合は、状態を記録しない

//        UpdateSelection();
//    }

//    // -------------------------------------------------------------------------
//    // 6. Selection & Action (選択と実行)
//    // -------------------------------------------------------------------------

//    private void UpdateSelection()
//    {
//        UpdateSelectionInternal();
//        ShowSubMenu(GetGlobalIndex());
//    }

//    private void UpdateSelectionInternal()
//    {
//        int globalIndex = GetGlobalIndex();
//        if (globalIndex >= 0 && globalIndex < mainMenuButtons.Length)
//        {
//            eventSystem.SetSelectedGameObject(mainMenuButtons[globalIndex].gameObject);
//        }
//    }

//    // --- 選択中ボタンの処理を実行 ---
//    private void ExecuteSelectedButtonAction()
//    {
//        int globalIndex = GetGlobalIndex();

//        switch (globalIndex)
//        {
//            case 0: OnButtonEasy(); break;
//            case 1: OnButtonNormal(); break;
//            case 2: OnButtonHard(); break;

//            case 3: OnButtonExit(); break;
//            case 4: OnButtonRank(); break;
//        }
//    }

//    // --- 各ボタンのアクション ---
//    public void OnButtonEasy() { Debug.Log("A項目: Easyが実行されました！"); SceneManager.LoadScene("TitleScene"); }
//    public void OnButtonNormal() { Debug.Log("A項目: Normalが実行されました！"); SceneManager.LoadScene("TitleScene"); }
//    public void OnButtonHard() { Debug.Log("A項目: Hardが実行されました！"); SceneManager.LoadScene("TitleScene"); }
//    public void OnButtonExit() { Debug.Log("B項目: Exitが実行されました！"); SceneManager.LoadScene("OutCenterScene"); }
//    public void OnButtonRank() { Debug.Log("B項目: Rankが実行されました！"); SceneManager.LoadScene("SampleScene"); }

//    // -------------------------------------------------------------------------
//    // 7. Sub-Menu Control (サブメニュー制御)
//    // -------------------------------------------------------------------------

//    private void ShowSubMenu(int index)
//    {
//        if (lastSubMenuIndex != -1 && lastSubMenuIndex < subMenus.Length && subMenus[lastSubMenuIndex] != null)
//        {
//            subMenus[lastSubMenuIndex].SetActive(false);
//        }

//        if (index >= 0 && index < subMenus.Length && subMenus[index] != null)
//        {
//            subMenus[index].SetActive(true);
//            lastSubMenuIndex = index;
//        }
//        else
//        {
//            lastSubMenuIndex = -1;
//        }
//    }

//    private void HideAllSubMenus()
//    {
//        foreach (GameObject subMenu in subMenus)
//        {
//            if (subMenu != null)
//            {
//                subMenu.SetActive(false);
//            }
//        }
//        lastSubMenuIndex = -1;
//    }
//}