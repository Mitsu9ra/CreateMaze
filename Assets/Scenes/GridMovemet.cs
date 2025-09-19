using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPos;
    private int[,] maze;
    private float inputDelay = 0.1f;
    private float lastInputTime;

    void Update()
    {
        if (maze == null) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) > 0.01f) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

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
}
