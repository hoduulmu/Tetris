using UnityEngine;
using UnityEngine.Tilemaps;

// 테트리스 조각들이 올라가는 메인 보드판 로직
public class Board : MonoBehaviour
{
    public Tilemap Tilemap { get; private set; }

    // 현 보드판에 활성화된 테트리스 조각
    public Piece ActivePiece { get; private set; }
    public Vector2Int boardSize = new(10, 20);
    public TetrominoData[] tetrominos;
    public Vector3Int tileSpawnPosition;
    public GameManager gameManager;

    // 보드판 바운더리
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    // 초기화
    public void Awake()
    {
        this.Tilemap = GetComponentInChildren<Tilemap>();
        this.ActivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }

    public void Start()
    {
        SpawnPiece();
    }

    // 테트리스 조각 랜덤 생성 및 생성시 유효한 포지션이 아니면 게임 오버
    // 생성시 유효한 포지션이 아님 -> 이미 둘 곳이 없는 상태 -> 게임 오버
    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];
        ActivePiece.Initialize(this, tileSpawnPosition, data);

        if (IsValidPosition(ActivePiece, tileSpawnPosition))
        {
            Set(ActivePiece);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        gameManager.GameOver();
    }

    // 현 보드판 위에 테트리스 조각을 세팅
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    // 현 보드판 위에 테트리스 조각을 지움
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    // 보드판 바운더리 및 현재 테트리스 조각들이 깔린 위치를 활용해 인자로 들어온 테트리스 조각을 둘 수 있는지 체크
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (Tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    // 보드판 라인이 꽉찼으면 라인 클리어 점수를 올리면서 라인을 지움
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                gameManager.addLineClearCount();
            }
            else
            {
                row++;
            }
        }
    }

    // 해당 열이 모두 테트리스 조각으로 찼는지 체크하는 함수
    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new(col, row, 0);
            if (!Tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    // 인자로 들어온 열을 돌면서 테트리스 조각들을 지우고 위에 있는 테트리스 조각을 아래로 옮기는 함수
    private void LineClear(int row)
    {
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new(col, row, 0);
            Tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new(col, row + 1, 0);
                TileBase above = Tilemap.GetTile(position);
                position = new(col, row, 0);
                Tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}
