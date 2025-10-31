using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("カメラのX/Y座標をターゲットに合わせた際の、Y軸方向のオフセット")]
    public float yOffset = 0f;

    [Tooltip("ズームイン・移動にかける時間 (秒)")]
    public float duration = 1.0f;

    [Tooltip("補間カーブの緩急を制御 (0:一定速度, 1:スムーズ)")]
    [Range(0f, 1f)]
    public float smoothStepFactor = 1.0f;

    [Header("Zoom Constraints")]
    [Tooltip("アクション実行時のズームで許可されるorthographicSizeの最小値（ズームイン最大値）")]
    public float minExecutionZoomSize = 1.0f;

    [Tooltip("ボタン選択時のズームで許可されるorthographicSizeの最小値（ズームイン最大値）")]
    public float minSelectionZoomSize = 1.5f;

    // プライベート変数
    private Camera cam;
    private bool isMoving = false;
    private float cameraZ;
    private Vector3 initialPosition; // ⭐ 初期位置
    private float initialSize;       // ⭐ 初期サイズ

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic)
        {
            Debug.LogError("CameraMoverはOrthographicカメラにアタッチしてください。");
            enabled = false;
            return;
        }

        cameraZ = transform.position.z;

        // ⭐ 初期状態を保存
        initialPosition = transform.position;
        initialSize = cam.orthographicSize;
    }

    /// <summary>
    /// カメラを初期のズームと位置に戻す
    /// </summary>
    public void ResetToDefaultTarget() // ⭐ 新規追加
    {
        InternalMoveToTarget(initialPosition, initialSize);
    }

    /// <summary>
    /// アクション実行時のズーム移動（ButtonExecutorから呼び出される）
    /// </summary>
    public void MoveToExecutionTarget(Transform newTarget, float newZoomSize)
    {
        float clampedZoomSize = Mathf.Max(newZoomSize, minExecutionZoomSize);
        InternalMoveToTarget(newTarget, clampedZoomSize);
    }

    /// <summary>
    /// ボタン選択時のズーム移動（Bsから呼び出される）
    /// </summary>
    public void MoveToSelectionTarget(Transform newTarget, float newZoomSize)
    {
        float clampedZoomSize = Mathf.Max(newZoomSize, minSelectionZoomSize);
        InternalMoveToTarget(newTarget, clampedZoomSize);
    }

    // ヘルパーメソッド: Transformを受け取る
    private void InternalMoveToTarget(Transform newTarget, float clampedZoomSize)
    {
        if (newTarget == null)
        {
            Debug.LogError("新しいターゲットが設定されていません。");
            return;
        }

        // ターゲット位置を2D専用に計算 (YOffsetとZ座標を適用)
        Vector3 targetPosition = new Vector3(
            newTarget.position.x,
            newTarget.position.y + yOffset,
            cameraZ
        );

        InternalMoveToTarget(targetPosition, clampedZoomSize);
    }

    // ヘルパーメソッド: Vector3を受け取る（Reset用）
    private void InternalMoveToTarget(Vector3 position, float clampedZoomSize)
    {
        if (isMoving)
        {
            StopAllCoroutines();
        }

        StartCoroutine(MoveAndZoom(position, clampedZoomSize));
    }


    private IEnumerator MoveAndZoom(Vector3 endPosition, float endZoomSize)
    {
        isMoving = true;
        float timeElapsed = 0f;
        Vector3 startPosition = transform.position;
        float startSize = cam.orthographicSize;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;

            float smoothT = Mathf.Lerp(t, Mathf.SmoothStep(0f, 1f, t), smoothStepFactor);

            transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);

            cam.orthographicSize = Mathf.Lerp(startSize, endZoomSize, smoothT);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        cam.orthographicSize = endZoomSize;

        isMoving = false;
    }
}