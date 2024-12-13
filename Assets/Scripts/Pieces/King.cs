using UnityEngine;

public class King : Piece
{
    protected bool m_firstMove = true;

    public bool FirstMove { get => m_firstMove; }

    public void MakeFirstMove()
    {
        m_firstMove = false;

        if (m_pieceColor == PieceColor.White)
        {
            GameMode.Instance.whiteKingMoved = true;
        }
        else
        {
            GameMode.Instance.blackKingMoved = true;
        }
    }
}
