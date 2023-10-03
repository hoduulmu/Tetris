using UnityEngine.Tilemaps;
using UnityEngine;

public enum Tetromino
{
    I, 
    O,
    T,
    J,
    L,
    S,
    Z
}

// 테트리스 조각 데이터로 모양과 타일, 위치와 벽에 부딪혔을 시 테스트할 월킥 목록을 가지고 있음
[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] Cells { get; private set; }
    public Vector2Int[,] WallKicks { get; private set; }

    public void Initialize()
    {
        Cells = DataConst.Cells[tetromino];
        WallKicks = DataConst.WallKicks[tetromino];
    }
}
