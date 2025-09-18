using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 15;
    public int height = 15;
    public GameObject wallPrefab;

    private int[,] maze;

    void Start()
    {
        GenerateMaze();
        DrawMaze();
        AdjustCamera();

    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // 全部を壁(0)で埋める
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 0;
            }
        }

        // スタート位置
        Carve(1, 1);
    }

    void Carve(int x, int y)
    {
        // 掘った場所を通路(1)に
        maze[x, y] = 1;

        // ランダムな方向に進む
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };
        int[] dir = { 0, 1, 2, 3 };
        Shuffle(dir);

        foreach (int i in dir)
        {
            int nx = x + dx[i] * 2;
            int ny = y + dy[i] * 2;

            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1)
            {
                if (maze[nx, ny] == 0)
                {
                    maze[x + dx[i], y + dy[i]] = 1; // 間の壁を壊す
                    Carve(nx, ny);
                }
            }
        }
    }

    void Shuffle(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int r = Random.Range(i, array.Length);
            int tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 0)
                {
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                }
            }
        }
    }
    void AdjustCamera()
{
    Camera cam = Camera.main;

    // 迷路の中心にカメラを置く
    cam.transform.position = new Vector3(width / 2f, height / 2f, -10f);

    // 横幅と高さに応じてカメラサイズを調整
    float aspect = (float)Screen.width / (float)Screen.height;
    float cameraSizeX = width / 2f / aspect;
    float cameraSizeY = height / 2f;
    cam.orthographicSize = Mathf.Max(cameraSizeX, cameraSizeY);
}

}
