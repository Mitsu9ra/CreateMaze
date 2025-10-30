using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowCountdown : MonoBehaviour
{
    public float m_fStartTime;
    public string m_strFormat;
    public GameTimer m_gameTimer;

    private Text m_txt;

    public float RemainingTime { get; private set; }
    public float RemainingTimePlus15 => RemainingTime + 15f;

    private void Start()
    {
        m_txt = GetComponent<Text>();

        if (m_fStartTime <= 0f)
        {
            m_fStartTime = 60f;
            Debug.LogWarning("ShowCountdown: m_fStartTime が設定されていないため、60秒に初期化しました。");
        }

        if (m_gameTimer == null)
        {
            m_gameTimer = FindObjectOfType<GameTimer>();
            if (m_gameTimer == null)
                Debug.LogError("GameTimer がシーン内に見つかりません！");
        }
    }

    private void Update()
    {
        RemainingTime = Mathf.Clamp(m_fStartTime - m_gameTimer.CurrentTime, 0f, m_fStartTime);
        m_txt.text = string.Format(m_strFormat, RemainingTime);
    }

    // ★ 追加: 残り時間を増やす関数
    public void AddExtraTime(float seconds)
    {
        m_fStartTime += seconds;
        Debug.Log($"タイマー延長: +{seconds}秒 (合計 {m_fStartTime:F1}秒)");
    }
}
