using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator_WallExtend_Proper : MonoBehaviour
{
    public int width = 21;   // 奇数推奨
    public int height = 21;  // 奇数推奨
    public GameObject wallPrefab;

    private int[,] maze;
    private List<Vector2Int> cellList = new List<Vector2Int>();
    private System.Random rand = new System.Random();

    void Start()
    {
        // 奇数に補正
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        GenerateMaze();
        DrawMaze();
        AdjustCamera();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // 全部壁(0)で初期化
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        // 候補セルリストに偶数座標を追加（奇数から始めると通路が奇数座標に広がる）
        for (int x = 1; x < width; x += 2)
            for (int y = 1; y < height; y += 2)
                cellList.Add(new Vector2Int(x, y));

        // ランダムにスタート
        Vector2Int start = cellList[rand.Next(cellList.Count)];
        maze[start.x, start.y] = 1;

        List<Vector2Int> activeCells = new List<Vector2Int>();
        activeCells.Add(start);

        // 壁伸ばしループ
        while (activeCells.Count > 0)
        {
            // ランダムにセル選択
            int index = rand.Next(activeCells.Count);
            Vector2Int cell = activeCells[index];

            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 2マス先のセルで通路でないものをチェック
            if (cell.y >= 2 && maze[cell.x, cell.y - 2] == 0) neighbors.Add(Vector2Int.down);
            if (cell.y < height - 2 && maze[cell.x, cell.y + 2] == 0) neighbors.Add(Vector2Int.up);
            if (cell.x >= 2 && maze[cell.x - 2, cell.y] == 0) neighbors.Add(Vector2Int.left);
            if (cell.x < width - 2 && maze[cell.x + 2, cell.y] == 0) neighbors.Add(Vector2Int.right);

            if (neighbors.Count > 0)
            {
                // ランダムに方向選択
                Vector2Int dir = neighbors[rand.Next(neighbors.Count)];

                // 壁を通路に変える
                int nx = cell.x + dir.x;
                int ny = cell.y + dir.y;
                int nnx = cell.x + dir.x * 2;
                int nny = cell.y + dir.y * 2;

                maze[nx, ny] = 1;
                maze[nnx, nny] = 1;

                // 新しいセルをアクティブリストに追加
                activeCells.Add(new Vector2Int(nnx, nny));
            }
            else
            {
                // 拡張できないセルは削除
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
