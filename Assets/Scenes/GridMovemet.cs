using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // 移動速度
    private Vector3 targetPos;        // 移動先の座標
    private MazeGenerator_WallExtend_Proper mazeGen; 
    private int[,] maze;

    private float inputDelay = 0.1f;  // 連続入力の間隔
    private float lastInputTime;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        // mazeが未取得なら遅延取得
        if (maze == null)
        {
            mazeGen = FindObjectOfType<MazeGenerator_WallExtend_Proper>();
            if (mazeGen != null)
                maze = mazeGen.GetMaze();
            else
                return;
        }

        // 既に移動中はターゲットへ補間
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) > 0.01f) return;

        // 入力判定（押しっぱ対応）
        float h = Input.GetAxisRaw("Horizontal"); // 左右
        float v = Input.GetAxisRaw("Vertical");   // 上下

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
                maze[newX, newY] == 1) // 通路のみ
            {
                targetPos = new Vector3(newX, newY, transform.position.z);
                lastInputTime = Time.time; // 入力時間更新
            }
        }
    }
}
