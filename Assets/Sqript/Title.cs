using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public static int flag = 0;
    // Inspector����ݒ�ł���悤�ɁA2�̃{�^�������J
    public Button buttonA;
    public Button buttonB;

    // �X�P�[���ύX�p�ϐ�
    private Vector3 originalScaleA;
    private Vector3 originalScaleB;
    private readonly float scaleFactor = 1.1f; // 10%�g�� (1.1�{)

    // Canvas�R���|�[�l���g��ێ�����t�B�[���h
    private Canvas canvas;

    // ���O�̃t���[���Ńz�o�[����Ă����{�^�����L�� (�}�E�XEnter����p)
    private Button previousHoveredButton = null;

    void Start()
    {
        // �N�����Ƀ{�^���̌��݂̃X�P�[����ۑ�
        if (buttonA != null) originalScaleA = buttonA.transform.localScale;
        if (buttonB != null) originalScaleB = buttonB.transform.localScale;

        // �e�܂��͎��g����Canvas�R���|�[�l���g���擾
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Title�X�N���v�g��Canvas�̎q�I�u�W�F�N�g�ɃA�^�b�`����Ă���K�v������܂��B");
            return;
        }

        // ������Ԃ�ButtonA��I����Ԃɂ���
        SelectButton(buttonA);
    }

    // ���t���[���Ăяo�����֐�
    void Update()
    {
        GameObject currentSelection = EventSystem.current.currentSelectedGameObject;

        // ���㏟���������W�b�N�i�}�E�X�D��j��
        Button currentHoveredButton = GetCurrentHoveredButton();

        // ��ԕω����o: �}�E�X���V�����{�^���̏�ɓ������u��
        if (currentHoveredButton != null && currentHoveredButton != previousHoveredButton)
        {
            // �}�E�X���͂��L�[�{�[�h�I���ɏ���: �I����Ԃ��}�E�X�ɓ���
            SelectButton(currentHoveredButton);
        }
        // �z�o�[��Ԃ̋L�����X�V
        previousHoveredButton = currentHoveredButton;

        // ���L�[�{�[�h���샍�W�b�N�i�㏟���j��
        bool isKeyDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) ||
                         Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);

        if (isKeyDown)
        {
            // �L�[���͂��}�E�X�I���ɏ���: �I����Ԃ��L�[�{�[�h�ő��삵���{�^���ɐݒ�
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

        // �{�^���̃X�P�[���ƑI����Ԃ̐���
        // ���C����: �I����ԁiEventSystem.current.currentSelectedGameObject�j�݂̂Ɋ�Â��Ċg�������
        SetButtonScale(buttonA, currentSelection == buttonA.gameObject, originalScaleA);
        SetButtonScale(buttonB, currentSelection == buttonB.gameObject, originalScaleB);

        // �L�[���͂ɂ��{�^�����s
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

    // �{�^���̃X�P�[���ƑI����Ԃ�ݒ肷��w���p�[�֐�
    private void SetButtonScale(Button button, bool isSelected, Vector3 originalScale)
    {
        if (button == null || canvas == null) return;

        // ���C���_�F�g�働�W�b�N��EventSystem�̑I����Ԃ݂̂Ɉˑ������遚
        // �}�E�X�I�[�o�[�̔���́A�����ł͎��o�I�ȃn�C���C�g�ɂ͎g�p���Ȃ��B
        // isSelected (EventSystem�őI������Ă�����) ��true�̏ꍇ�̂݊g�傷��B
        if (isSelected)
        {
            button.transform.localScale = originalScale * scaleFactor;
        }
        else
        {
            // �I������Ă��Ȃ��ꍇ�͌��̃T�C�Y�ɖ߂�
            button.transform.localScale = originalScale;
        }
    }

    // �{�^����I����Ԃɂ���w���p�[�֐�
    private void SelectButton(Button buttonToSelect)
    {
        if (buttonToSelect != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }
    }

    /// <summary>
    /// A�{�^���������ꂽ�Ƃ��Ɏ��s����鏈��
    /// </summary>
   
        
public void HandleButtonAAction()
    {
        Debug.Log(buttonA.gameObject.name + "������܂���");
        const string FLAG_KEY = "MENU_ACTION_FLAG";
        int flagValue = PlayerPrefs.GetInt(FLAG_KEY, 0);

        Debug.Log($"[Title] PlayerPrefs����ǂݍ��񂾓�Փx�t���O�l: {flagValue}");

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
        Debug.Log($"[Title] Title.flag �ɐݒ肵���ŏI�l: {Title.flag}");
        SceneManager.LoadScene("MazeScene");
    }

    /// <summary>
    /// B�{�^���������ꂽ�Ƃ��Ɏ��s����鏈��
    /// </summary>
    public void HandleButtonBAction()
    {
        Debug.Log(buttonB.gameObject.name + "������܂���");
        SceneManager.LoadScene("InCenterScene");
    }
}