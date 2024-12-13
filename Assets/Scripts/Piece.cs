using System.Collections.Generic;
using UnityEngine;

public enum PieceColor
{
    None,
    Black,
    White
}

public class Piece : MonoBehaviour
{
    [SerializeField] protected PieceInfo pieceInfo;
    [SerializeField] private GameTile m_currentTile;
    [SerializeField] protected PieceColor m_pieceColor;
    [SerializeField] private Transform m_ghostObject;
    [SerializeField] private Collider m_mainCollider;
    [SerializeField] private Collider m_ghostColldier;

    public List<Vector2Int> ListOfMoves = new List<Vector2Int>();
    protected int m_colorMultiplier = 1;
    private MeshFilter m_Filter;

    public PieceInfo PieceInfo { get => pieceInfo; }
    public PieceColor PieceColor { get => m_pieceColor; }
    public GameTile CurrentTile { get => m_currentTile; }

    private void Awake()
    {
        m_Filter = GetComponent<MeshFilter>();
    }

    protected virtual void Start()
    {
        if (m_pieceColor == PieceColor.Black)
        {
            m_colorMultiplier *= -1;
        }

        InitPieceMovementLimits();
    }

    public void EnableGhost(Vector3 newPos)
    {
        m_ghostObject.position = newPos;
        m_mainCollider.enabled = false;
        m_ghostColldier.enabled = true;
    }

    public void DisableGhost()
    {
        m_ghostColldier.enabled = false;
        m_mainCollider.enabled = true;
        m_ghostObject.position = transform.position;
    }

    public PieceName GetPieceName()
    {
        return pieceInfo.pieceName;
    }

    public void InitPiece(GameTile newTile)
    {
        m_currentTile = newTile;
        m_currentTile.OccupyTile(this);
        transform.position = new Vector3(m_currentTile.GetTilePos().x, transform.position.y, m_currentTile.GetTilePos().y);

        m_Filter.mesh = pieceInfo.pieceMesh;
    }

    public void SetTile(GameTile newTile)
    {
        m_currentTile = newTile;
    }

    private void InitPieceMovementLimits()
    {
        switch (pieceInfo.pieceName)
        {
            case PieceName.Pawn:
                ListOfMoves.Add(new Vector2Int(0, 1 * m_colorMultiplier));
                break;

            case PieceName.Queen:
                AddDirectionalMoves(1, 0);  // Up
                AddDirectionalMoves(-1, 0); // Down
                AddDirectionalMoves(0, 1);  // Right
                AddDirectionalMoves(0, -1); // Left
                AddDirectionalMoves(1, 1);  // Up-right diagonal
                AddDirectionalMoves(-1, -1); // Down-left diagonal
                AddDirectionalMoves(1, -1);  // Up-left diagonal
                AddDirectionalMoves(-1, 1);  // Down-right diagonal

                break;

            case PieceName.Rook:
                AddDirectionalMoves(1, 0);  // Up
                AddDirectionalMoves(-1, 0); // Down
                AddDirectionalMoves(0, 1);  // Right
                AddDirectionalMoves(0, -1); // Left

                break;

            case PieceName.Bishop:
                AddDirectionalMoves(1, 1);  // Up-right diagonal
                AddDirectionalMoves(-1, -1); // Down-left diagonal
                AddDirectionalMoves(1, -1);  // Up-left diagonal
                AddDirectionalMoves(-1, 1);  // Down-right diagonal

                break;

            case PieceName.King:
                ListOfMoves.Add(new Vector2Int(1, 1));
                ListOfMoves.Add(new Vector2Int(1, 0));
                ListOfMoves.Add(new Vector2Int(0, 1));
                ListOfMoves.Add(new Vector2Int(-1, 1));
                ListOfMoves.Add(new Vector2Int(-1, -1));
                ListOfMoves.Add(new Vector2Int(-1, 0));
                ListOfMoves.Add(new Vector2Int(0, -1));
                ListOfMoves.Add(new Vector2Int(1, -1));

                break;

            case PieceName.Knight:
                ListOfMoves.Add(new Vector2Int(2, 1));
                ListOfMoves.Add(new Vector2Int(2, -1));
                ListOfMoves.Add(new Vector2Int(-2, 1));
                ListOfMoves.Add(new Vector2Int(-2, -1));

                ListOfMoves.Add(new Vector2Int(1, 2));
                ListOfMoves.Add(new Vector2Int(1, -2));
                ListOfMoves.Add(new Vector2Int(-1, 2));
                ListOfMoves.Add(new Vector2Int(-1, -2));

                break;
        }

    }

    private void AddDirectionalMoves(int deltaX, int deltaY)
    {
        for (int i = 1; i <= 8; i++)
        {
            ListOfMoves.Add(new Vector2Int(deltaX * i, deltaY * i));
        }
    }
}
