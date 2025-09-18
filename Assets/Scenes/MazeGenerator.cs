using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator_WallExtend_Proper : MonoBehaviour
{
    public int width = 21;   // �����
    public int height = 21;  // �����
    public GameObject wallPrefab;

    private int[,] maze;
    private List<Vector2Int> cellList = new List<Vector2Int>();
    private System.Random rand = new System.Random();

    void Start()
    {
        // ��ɕ␳
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        GenerateMaze();
        DrawMaze();
        AdjustCamera();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // �S����(0)�ŏ�����
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        // ���Z�����X�g�ɋ������W��ǉ��i�����n�߂�ƒʘH������W�ɍL����j
        for (int x = 1; x < width; x += 2)
            for (int y = 1; y < height; y += 2)
                cellList.Add(new Vector2Int(x, y));

        // �����_���ɃX�^�[�g
        Vector2Int start = cellList[rand.Next(cellList.Count)];
        maze[start.x, start.y] = 1;

        List<Vector2Int> activeCells = new List<Vector2Int>();
        activeCells.Add(start);

        // �ǐL�΂����[�v
        while (activeCells.Count > 0)
        {
            // �����_���ɃZ���I��
            int index = rand.Next(activeCells.Count);
            Vector2Int cell = activeCells[index];

            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 2�}�X��̃Z���ŒʘH�łȂ����̂��`�F�b�N
            if (cell.y >= 2 && maze[cell.x, cell.y - 2] == 0) neighbors.Add(Vector2Int.down);
            if (cell.y < height - 2 && maze[cell.x, cell.y + 2] == 0) neighbors.Add(Vector2Int.up);
            if (cell.x >= 2 && maze[cell.x - 2, cell.y] == 0) neighbors.Add(Vector2Int.left);
            if (cell.x < width - 2 && maze[cell.x + 2, cell.y] == 0) neighbors.Add(Vector2Int.right);

            if (neighbors.Count > 0)
            {
                // �����_���ɕ����I��
                Vector2Int dir = neighbors[rand.Next(neighbors.Count)];

                // �ǂ�ʘH�ɕς���
                int nx = cell.x + dir.x;
                int ny = cell.y + dir.y;
                int nnx = cell.x + dir.x * 2;
                int nny = cell.y + dir.y * 2;

                maze[nx, ny] = 1;
                maze[nnx, nny] = 1;

                // �V�����Z�����A�N�e�B�u���X�g�ɒǉ�
                activeCells.Add(new Vector2Int(nnx, nny));
            }
            else
            {
                // �g���ł��Ȃ��Z���͍폜
                activeCells.RemoveAt(index);
            }
        }
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 0 && wallPrefab != null)
                {
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                }
            }
        }
    }

    void AdjustCamera()
    {
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        float aspect = (float)Screen.width / (float)Screen.height;
        float cameraSizeX = width / 2f / aspect;
        float cameraSizeY = height / 2f;
        cam.orthographicSize = Mathf.Max(cameraSizeX, cameraSizeY);
    }
}
