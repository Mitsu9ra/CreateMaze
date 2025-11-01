using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator_WallExtend_Proper : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject goalPrefab; // Inspectorで設定してなくてもOK

    [HideInInspector] public Vector2Int startPos;
    [HideInInspector] public Vector2Int goalPos;
    [HideInInspector] public GameObject playerInstance;
    [HideInInspector] public GameObject goalInstance;

    private int[,] maze;
    private List<Vector2Int> cellList = new List<Vector2Int>();
    private System.Random rand = new System.Random();
    
    void Start()
    {
        int flagValue = Title.flag;
        Debug.Log($"[MazeGenerator] Title.flagから取得した難易度値: {flagValue}");
        switch (flagValue)
        {
            case 1:
                width = 11;
                height = 11;
                break;
            case 2:
                width = 19;
                height = 19;
                break;
            case 3:
                width = 29;
                height = 29;
                break;
            default:
                width = 21;
                height = 21;
                break;
        }
                if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        GenerateAndDrawMaze();
    }

    public void GenerateAndDrawMaze()
    {
        // 古い壁・プレイヤー・ゴール削除
        foreach (Transform child in transform) Destroy(child.gameObject);
        if (playerInstance != null) { Destroy(playerInstance); playerInstance = null; }
        if (goalInstance != null) { Destroy(goalInstance); goalInstance = null; }

        // 迷路生成
        GenerateMaze();
        DrawMaze();
        SetStartAndGoal();
        AdjustCamera();
        if (width >= 21)
        {
            HighlightRandomCell();
        }
    }

    void GenerateMaze()
    {
        maze = new int[width, height];
        cellList.Clear();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 0;

        for (int x = 1; x < width; x += 2)
            for (int y = 1; y < height; y += 2)
                cellList.Add(new Vector2Int(x, y));

        Vector2Int start = cellList[rand.Next(cellList.Count)];
        maze[start.x, start.y] = 1;

        List<Vector2Int> activeCells = new List<Vector2Int> { start };

        while (activeCells.Count > 0)
        {
            int index = rand.Next(activeCells.Count);
            Vector2Int cell = activeCells[index];
            List<Vector2Int> neighbors = new List<Vector2Int>();

            if (cell.y >= 2 && maze[cell.x, cell.y - 2] == 0) neighbors.Add(Vector2Int.down);
            if (cell.y < height - 2 && maze[cell.x, cell.y + 2] == 0) neighbors.Add(Vector2Int.up);
            if (cell.x >= 2 && maze[cell.x - 2, cell.y] == 0) neighbors.Add(Vector2Int.left);
            if (cell.x < width - 2 && maze[cell.x + 2, cell.y] == 0) neighbors.Add(Vector2Int.right);

            if (neighbors.Count > 0)
            {
                Vector2Int dir = neighbors[rand.Next(neighbors.Count)];
                int nx = cell.x + dir.x;
                int ny = cell.y + dir.y;
                int nnx = cell.x + dir.x * 2;
                int nny = cell.y + dir.y * 2;

                maze[nx, ny] = 1;
                maze[nnx, nny] = 1;

                activeCells.Add(new Vector2Int(nnx, nny));
            }
            else
            {
                activeCells.RemoveAt(index);
            }
        }
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (maze[x, y] == 0 && wallPrefab != null)
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
    }

    void SetStartAndGoal()
    {
        // スタートとゴール位置決定
        Vector2Int[] corners = new Vector2Int[]
        {
            new Vector2Int(1,1),
            new Vector2Int(width-2,1),
            new Vector2Int(1,height-2),
            new Vector2Int(width-2,height-2)
        };
        int startIndex = Random.Range(0, corners.Length);
        int goalIndex = (startIndex + Random.Range(1, corners.Length)) % corners.Length;

        startPos = corners[startIndex];
        goalPos = corners[goalIndex];

        if (maze[startPos.x, startPos.y] == 0) maze[startPos.x, startPos.y] = 1;
        if (maze[goalPos.x, goalPos.y] == 0) maze[goalPos.x, goalPos.y] = 1;

        // プレイヤー生成
        playerInstance = Instantiate(playerPrefab, new Vector3(startPos.x, startPos.y, -0.1f), Quaternion.identity);
        playerInstance.tag = "Player";

        GridMovement gm = playerInstance.GetComponent<GridMovement>();
        if (gm != null)
            gm.SetMaze(maze, new Vector3(startPos.x, startPos.y, -0.1f));

        // Goal Prefab が null の場合は Resources からロード
        if (goalPrefab == null)
            goalPrefab = Resources.Load<GameObject>("Goal");

        if (goalPrefab != null)
        {
            goalInstance = Instantiate(goalPrefab, new Vector3(goalPos.x, goalPos.y, -0.1f), Quaternion.identity);
            goalInstance.tag = "Goal";

            // 必ず BoxCollider2D と Goal スクリプトを有効化
            BoxCollider2D col = goalInstance.GetComponent<BoxCollider2D>();
            if (col != null) col.enabled = true;

            Goal goalScript = goalInstance.GetComponent<Goal>();
            if (goalScript != null) goalScript.enabled = true;
        }
        else
        {
            Debug.LogError("goalPrefab が設定されていません！");
        }
    }

    public void ResetMazeSafe()
{
    // prefab が設定されているか確認
    if (goalPrefab == null || playerPrefab == null || wallPrefab == null)
    {
        Debug.LogError("Prefab が設定されていません!");
        return;
    }



    // 迷路をリセット
    ResetMaze();
}

    void AdjustCamera()
    {
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        float aspect = (float)Screen.width / Screen.height;
        float sizeX = width / 2f / aspect;
        float sizeY = height / 2f;
        cam.orthographicSize = Mathf.Max(sizeX, sizeY);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        
    }

    public int[,] GetMaze() => maze;
    public void ResetMaze() => GenerateAndDrawMaze();

    public void ExpandMaze(int amount)
    {
        width += amount;
        height += amount;
        Debug.Log($"迷路拡張: {width} × {height}");
    }

    void HighlightRandomCell()
    {
        List<Vector2Int> openCells = new List<Vector2Int>();

        // 通路セルを収集（外周1マスは除外）
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y] == 1)
                {
                    // 周囲4方向が壁で囲まれていない場所だけ選ぶ
                    if (maze[x + 1, y] == 1 && maze[x - 1, y] == 1 &&
                        maze[x, y + 1] == 1 && maze[x, y - 1] == 1)
                    {
                        openCells.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        if (openCells.Count == 0)
        {
            Debug.LogWarning("⚠️ 有効な通路セルが見つかりませんでした。");
            return;
        }

        // ✅ 難易度やサイズに応じて生成数変更
        int count = width >= 25 ? 5 : 3;

        for (int i = 0; i < count; i++)
        {
            // ランダムな通路を選択（重複防止）
            int index = Random.Range(0, openCells.Count);
            Vector2Int randomCell = openCells[index];
            openCells.RemoveAt(index);

            // ✅ 赤いマーカー生成
            GameObject marker = new GameObject($"ReverseTile_{i}");
            marker.transform.position = new Vector3(randomCell.x, randomCell.y, 0f);
            marker.transform.localScale = new Vector3(90f, 90f, 90f);

            SpriteRenderer sr = marker.AddComponent<SpriteRenderer>();

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.red);
            tex.Apply();

            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            sr.sortingOrder = 10;

            // ✅ Collider（トリガー）と Rigidbody2D 追加
            BoxCollider2D col = marker.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            Rigidbody2D rb = marker.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.isKinematic = true;

            marker.AddComponent<ReverseTrigger>();

            Debug.Log($"✅ ReverseTile[{i}] 生成完了: {randomCell}");
        }
    }




}
