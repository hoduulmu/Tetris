using UnityEngine;
using UnityEngine.Tilemaps;

// 보드판 맨밑에서 현 테트리스 조각이 놓일 위치 미리보기 해주는 객체
public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    // 현재 활성화되어 트래킹되는 테트리스 조각
    public Piece trackingPiece;
    public Tilemap Tilemap { get; private set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int[] Cells { get; private set; }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

    // 현 보드판의 모든 세팅이 끝나고 고스트 조각을 세팅해야 하기 때문에 늦은 업데이트 사용
    // 고스트 조각판 타일맵을 클리어하고 (아니면 드롭 함수에서 체킹이 제대로 안됨) 
    // 현재 트래킹하는 테트리스 조각의 위ㄴ치를 복사하여 가능한 제일 아래로 보드판에 세팅
    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i] = trackingPiece.Cells[i];
        }
    }

    // 트래킹하는 테트리스 조각을 둘 수 있는 가장 아래를 찾아 보드판에 세팅
    private void Drop()
    {
        Vector3Int currentPosition = trackingPiece.Position;

        int current = currentPosition.y;
        int bottom = -board.boardSize.y / 2 - 1;

        board.Clear(trackingPiece);
        for (int row = current; row >= bottom; row--)
        {
            currentPosition.y = row;
            if (board.IsValidPosition(trackingPiece, currentPosition))
            {
                Position = currentPosition; 
            }
            else
            {
                break;
            }
        }
        board.Set(trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3Int tilePosition = Cells[i] + Position;
            Tilemap.SetTile(tilePosition, tile);
        }
    }
}
