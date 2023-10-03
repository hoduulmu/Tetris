using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;
    public Tilemap Tilemap { get; private set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int[] Cells { get; private set; }

    private void Awake()
    {
        Tilemap = GetComponentInChildren<Tilemap>();
        Cells = new Vector3Int[4];
    }

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
