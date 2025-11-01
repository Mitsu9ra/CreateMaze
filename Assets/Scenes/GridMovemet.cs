using UnityEngine;
using System.Collections;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPos;
    private int[,] maze;
    private float inputDelay = 0.1f;
    private float lastInputTime;
    private bool inputLocked = false; // ← ← ← ここの "/" を消して正しく！

    // ✅ 反転フラグ
    public bool isReversed = false;

    void Update()
    {
        if (maze == null) return;

        // 移動中なら続けて動かす
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // 目的地にまだ到達してなければ新しい入力は受け付けない
        if (Vector3.Distance(transform.position, targetPos) > 0.01f) return;

        // ✅ 入力ロックがかかっている場合、ボタンが完全に離されるまで待機
        if (inputLocked)
        {
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                inputLocked = false; // ← 離されたら解除
                Debug.Log("🔓 入力ロック解除");
            }
            else
            {
                return; // ← 押しっぱなし中は入力無効
            }
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // ✅ 反転処理
        if (isReversed)
        {
            h = -h;
            v = -v;
        }

        Vector2Int dir = Vector2Int.zero;

        if (Time.time - lastInputTime >= inputDelay)
        {
            if (v > 0.1f) dir = Vector2Int.up;
            else if (v < -0.1f) dir = Vector2Int.down;
            else if (h > 0.1f) dir = Vector2Int.right;
            else if (h < -0.1f) dir = Vector2Int.left;
        }

        if (dir != Vector2Int.zero)
        {
            int newX = Mathf.RoundToInt(targetPos.x) + dir.x;
            int newY = Mathf.RoundToInt(targetPos.y) + dir.y;

            if (newX >= 0 && newX < maze.GetLength(0) &&
                newY >= 0 && newY < maze.GetLength(1) &&
                maze[newX, newY] == 1)
            {
                targetPos = new Vector3(newX, newY, transform.position.z);
                lastInputTime = Time.time;
            }
        }
    }

    public void SetMaze(int[,] newMaze, Vector3 startPos)
    {
        maze = newMaze;
        transform.position = startPos;
        targetPos = startPos;
    }

    // ✅ 操作反転を切り替える関数
    public void ReverseControls()
    {
        isReversed = !isReversed;
        Debug.Log($"操作反転状態: {isReversed}");
    }

    // ✅ 一定時間だけ反転させる
    public void ReverseTemporarily(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ReverseCoroutine(duration));
    }

    private IEnumerator ReverseCoroutine(float duration)
    {
        isReversed = true;
        LockInput(); // ← ここ追加：反転直後に入力ロック！

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;

        Debug.Log("操作反転開始");
        yield return new WaitForSeconds(duration);

        isReversed = false;
        if (sr != null) sr.color = Color.white;
        Debug.Log("操作反転終了");
    }

    // ✅ 入力ロックを外部からも使えるようにする
    public void LockInput()
    {
        inputLocked = true;
        Debug.Log("🔒 入力ロック中（キーを離すまで操作できません）");
    }
}
