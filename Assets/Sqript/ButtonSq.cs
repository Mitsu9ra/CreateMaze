//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using UnityEngine.EventSystems; // EventTrigger���g�p���邽�߂ɕK�v

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

//    // --- �O���[�v�ݒ� ---
//    private readonly int[] GroupSizes = { 3, 2 }; // A(3), B(2)

//    private int NumGroups;
//    private int[] GroupStartIndices;

//    // --- ��� ---
//    private int currentGroupIndex = 0;
//    private int currentButtonIndexInGroup = 0;
//    private int[] lastSelectedIndices; // �e�O���[�v�̍Ō�ɑI�����Ă����{�^�����L�^
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
//            Debug.LogError("ButtonSq: GroupSizes�̍��v��mainMenuButtons�̐�����v���܂���B");
//            enabled = false;
//            return;
//        }

//        lastSelectedIndices = new int[NumGroups];
//        for (int i = 0; i < NumGroups; i++)
//            lastSelectedIndices[i] = 0;

//        // �}�E�X�C�x���g�̐ݒ�����������Ɏ��s
//        SetupMouseEvents();
//    }

//    void Start()
//    {
//        HideAllSubMenus();

//        if (mainMenuButtons.Length > 0 && eventSystem != null)
//        {
//            // B���ڂ̏�����Ԃ�Rank�ɂȂ�悤��Start���̏������𒲐��i�I�v�V�����j
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
//    // 4. Mouse Event Setup & Handler (�}�E�X�C�x���g����)
//    // -------------------------------------------------------------------------

//    // �w���p�[�F�O���[�o���C���f�b�N�X����O���[�v�C���f�b�N�X�ƃO���[�v���C���f�b�N�X���擾
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
//        // �͈͊O�̏ꍇ�̃t�H�[���o�b�N
//        return (0, 0);
//    }

//    // �S�{�^����EventTrigger��ݒ�
//    private void SetupMouseEvents()
//    {
//        for (int i = 0; i < mainMenuButtons.Length; i++)
//        {
//            Button button = mainMenuButtons[i];
//            int globalIndex = i;

//            // EventTrigger�R���|�[�l���g��ǉ��܂��͎擾
//            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
//            if (trigger == null)
//            {
//                trigger = button.gameObject.AddComponent<EventTrigger>();
//            }

//            // PointerEnter�C�x���g�̃��X�i�[��ݒ�
//            EventTrigger.Entry entry = new EventTrigger.Entry();
//            entry.eventID = EventTriggerType.PointerEnter;

//            // �}�E�X���{�^���ɏ������OnButtonPointerEnter���Ăяo���悤�ɐݒ�
//            entry.callback.AddListener((data) => { OnButtonPointerEnter(globalIndex); });

//            trigger.triggers.Add(entry);
//        }
//    }

//    // --- �}�E�X���{�^���ɏ�������̏��� ---
//    public void OnButtonPointerEnter(int globalIndex)
//    {
//        // 1. �O���[�o���C���f�b�N�X���O���[�v�C���f�b�N�X�ɕϊ�
//        (int gIndex, int bIndex) = GetGroupIndicesFromGlobal(globalIndex);

//        // 2. ��ԕϐ����X�V���A�L�[�{�[�h�I���Ɠ���
//        currentGroupIndex = gIndex;
//        currentButtonIndexInGroup = bIndex;

//        // 3. A����(�O���[�v0)�̏ꍇ�̂݁A���̏�Ԃ��L������
//        if (currentGroupIndex == 0)
//        {
//            lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;
//        }

//        // 4. Unity EventSystem�Ńt�H�[�J�X���Z�b�g�i�}�E�X�I�����L�[�{�[�h�I���Ɠ����j
//        // 5. �T�u���j���[��\��
//        UpdateSelectionInternal();
//        ShowSubMenu(globalIndex);
//    }

//    // -------------------------------------------------------------------------
//    // 5. Navigation Logic (�i�r�Q�[�V�������W�b�N)
//    // -------------------------------------------------------------------------

//    private int GetGlobalIndex()
//    {
//        return GroupStartIndices[currentGroupIndex] + currentButtonIndexInGroup;
//    }

//    // --- �O���[�v�؂�ւ��i�㉺�L�[�j ---
//    private void NavigateGroup(int direction)
//    {
//        // 1. ���݂̑I����Ԃ��L���i�S�O���[�v�Ŏ��s�j
//        lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;

//        int newGroupIndex = currentGroupIndex + direction;

//        // Wrap-around
//        if (newGroupIndex >= NumGroups)
//            newGroupIndex = 0;
//        else if (newGroupIndex < 0)
//            newGroupIndex = NumGroups - 1;

//        currentGroupIndex = newGroupIndex;

//        // 2. �L�����Ă����C���f�b�N�X�𕜌��i�O���[�v�ɂ���ď����𕪂���j
//        if (currentGroupIndex == 0) // A���� (�O���[�v 0) �̏ꍇ
//        {
//            // �L�����Ă����C���f�b�N�X�𕜌�
//            int groupSize = GroupSizes[currentGroupIndex];
//            currentButtonIndexInGroup = Mathf.Clamp(lastSelectedIndices[currentGroupIndex], 0, groupSize - 1);
//        }
//        else // B���� (�O���[�v 1) �₻�̑��̃O���[�v�̏ꍇ
//        {
//            // B���� (�O���[�v1) �͏�� Rank (�C���f�b�N�X 1) ��I������
//            currentButtonIndexInGroup = 1;
//        }

//        UpdateSelection();
//    }

//    // --- �O���[�v���̃{�^���؂�ւ��i���E�L�[�j ---
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

//        // �O���[�v 0 �̏ꍇ�ɂ̂݁A�Ō�ɑI��������Ԃ��L������
//        if (currentGroupIndex == 0)
//        {
//            lastSelectedIndices[currentGroupIndex] = currentButtonIndexInGroup;
//        }
//        // �O���[�v 1 �̏ꍇ�́A��Ԃ��L�^���Ȃ�

//        UpdateSelection();
//    }

//    // -------------------------------------------------------------------------
//    // 6. Selection & Action (�I���Ǝ��s)
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

//    // --- �I�𒆃{�^���̏��������s ---
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

//    // --- �e�{�^���̃A�N�V���� ---
//    public void OnButtonEasy() { Debug.Log("A����: Easy�����s����܂����I"); SceneManager.LoadScene("TitleScene"); }
//    public void OnButtonNormal() { Debug.Log("A����: Normal�����s����܂����I"); SceneManager.LoadScene("TitleScene"); }
//    public void OnButtonHard() { Debug.Log("A����: Hard�����s����܂����I"); SceneManager.LoadScene("TitleScene"); }
//    public void OnButtonExit() { Debug.Log("B����: Exit�����s����܂����I"); SceneManager.LoadScene("OutCenterScene"); }
//    public void OnButtonRank() { Debug.Log("B����: Rank�����s����܂����I"); SceneManager.LoadScene("SampleScene"); }

//    // -------------------------------------------------------------------------
//    // 7. Sub-Menu Control (�T�u���j���[����)
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