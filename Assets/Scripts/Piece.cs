using System;
using Unity.VisualScripting;
using UnityEngine;

// 테트리스 조각 자체를 다루는 클래스
public class Piece : MonoBehaviour
{
    public Board Board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public int RotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;


    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.Board = board;
        this.Position = position;
        this.Data = data;
        this.RotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        Cells ??= new Vector3Int[data.Cells.Length];

        for (int i = 0; i < data.Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)data.Cells[i];
        }
    }

    // 유저 입력에 따라 현 조각을 움직이거나 회전
    private void Update()
    {
        Board.Clear(this);
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Time.time >= stepTime)
        {
            Step();
        }

        Board.Set(this);
    }

    // 조각이 떨어지는 스텝을 정의 및 조각 고정
    private void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    // 조각 고정 및 보드판 라인 클리어 및 새 조각 생성
    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece();
    }

    // 조각 드롭하는 함수
    public void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    // 조각을 움직이고 유효한 위치에 있는지 리턴하는 함수
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = Board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.Position = newPosition;
            this.lockTime = 0f;
        }

        return valid;
    }

    // 조각을 회전하는 함수
    // 요청된 방향에 따라 조각을 회전하고 조각의 회전 모양 인덱스를 구해 월킥 테스트
    // 벽에 부딪혔을 경우를 위해 회전 가능을 테스트하며 위치를 조정하고 모두 실패할 경우 원복함
    private void Rotate(int direction)
    {
        int originalRotation = RotationIndex;
        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    // 넘어온 방향에 따라 조각 회전
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];
            int x, y;

            switch (Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * DataConst.RotationMatrix[0] * direction) + (cell.y * DataConst.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * DataConst.RotationMatrix[2] * direction) + (cell.y * DataConst.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * DataConst.RotationMatrix[0] * direction) + (cell.y * DataConst.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * DataConst.RotationMatrix[2] * direction) + (cell.y * DataConst.RotationMatrix[3] * direction));
                    break;
            }

            this.Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // SRS 월킥 테스트 케이스를 모두 돌며 성공하면 바로 리턴
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < Data.WallKicks.GetLength(1); i++)
        {
            Vector2Int translation = Data.WallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }

    // 테스트할 월킥 인덱스를 가져옴
    // 0 >> 1, 1 >> 0, 1 >> 2 처럼
    // 오른쪽 회전의 인덱스가 모두 짝수인 것을 활용해 2배를 하고 왼쪽 회전일 경우 하나를 뺌
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, Data.WallKicks.GetLength(0));
    }

    // 인자 input이 항상 최소값, 최대값 사이의 값을 갖도록 래핑해주는 함수
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        return min + (input - min) % (max - min);
    }
}
