using UnityEngine;
using System.Collections.Generic;

public static class DataConst
{
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    // 회전 순서 배열
    public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

    // 테트리스 조각 모음
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new()
    {
        { Tetromino.I, new Vector2Int[] { new(-1, 1), new( 0, 1), new( 1, 1), new( 2, 1) } },
        { Tetromino.J, new Vector2Int[] { new(-1, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { Tetromino.L, new Vector2Int[] { new( 1, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { Tetromino.O, new Vector2Int[] { new( 0, 1), new( 1, 1), new( 0, 0), new( 1, 0) } },
        { Tetromino.S, new Vector2Int[] { new( 0, 1), new( 1, 1), new(-1, 0), new( 0, 0) } },
        { Tetromino.T, new Vector2Int[] { new( 0, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { Tetromino.Z, new Vector2Int[] { new(-1, 1), new( 0, 1), new( 0, 0), new( 1, 0) } },
    };

    // SRS 테트리스 월킥 조각 모음 -> 회전시 벽에 부딪혔을 때 시도할 테스트셋
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        { new(0, 0), new(-2, 0), new( 1, 0), new(-2,-1), new( 1, 2) },
        { new(0, 0), new( 2, 0), new(-1, 0), new( 2, 1), new(-1,-2) },
        { new(0, 0), new(-1, 0), new( 2, 0), new(-1, 2), new( 2,-1) },
        { new(0, 0), new( 1, 0), new(-2, 0), new( 1,-2), new(-2, 1) },
        { new(0, 0), new( 2, 0), new(-1, 0), new( 2, 1), new(-1,-2) },
        { new(0, 0), new(-2, 0), new( 1, 0), new(-2,-1), new( 1, 2) },
        { new(0, 0), new( 1, 0), new(-2, 0), new( 1,-2), new(-2, 1) },
        { new(0, 0), new(-1, 0), new( 2, 0), new(-1, 2), new( 2,-1) },
    };

    private static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] {
        { new(0, 0), new(-1, 0), new(-1, 1), new(0,-2), new(-1,-2) },
        { new(0, 0), new( 1, 0), new( 1,-1), new(0, 2), new( 1, 2) },
        { new(0, 0), new( 1, 0), new( 1,-1), new(0, 2), new( 1, 2) },
        { new(0, 0), new(-1, 0), new(-1, 1), new(0,-2), new(-1,-2) },
        { new(0, 0), new( 1, 0), new( 1, 1), new(0,-2), new( 1,-2) },
        { new(0, 0), new(-1, 0), new(-1,-1), new(0, 2), new(-1, 2) },
        { new(0, 0), new(-1, 0), new(-1,-1), new(0, 2), new(-1, 2) },
        { new(0, 0), new( 1, 0), new( 1, 1), new(0,-2), new( 1,-2) },
    };

    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks = new()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.J, WallKicksJLOSTZ },
        { Tetromino.L, WallKicksJLOSTZ },
        { Tetromino.O, WallKicksJLOSTZ },
        { Tetromino.S, WallKicksJLOSTZ },
        { Tetromino.T, WallKicksJLOSTZ },
        { Tetromino.Z, WallKicksJLOSTZ },
    };
}
