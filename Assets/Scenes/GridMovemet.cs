using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // �ړ����x
    private Vector3 targetPos;        // �ړ���̍��W
    private MazeGenerator_WallExtend_Proper mazeGen; 
    private int[,] maze;

    private float inputDelay = 0.1f;  // �A�����͂̊Ԋu
    private float lastInputTime;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        // maze�����擾�Ȃ�x���擾
        if (maze == null)
        {
            mazeGen = FindObjectOfType<MazeGenerator_WallExtend_Proper>();
            if (mazeGen != null)
                maze = mazeGen.GetMaze();
            else
                return;
        }

        // ���Ɉړ����̓^�[�Q�b�g�֕��
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) > 0.01f) return;

        // ���͔���i�������ϑΉ��j
        float h = Input.GetAxisRaw("Horizontal"); // ���E
        float v = Input.GetAxisRaw("Vertical");   // �㉺

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
                maze[newX, newY] == 1) // �ʘH�̂�
            {
                targetPos = new Vector3(newX, newY, transform.position.z);
                lastInputTime = Time.time; // ���͎��ԍX�V
            }
        }
    }
}
