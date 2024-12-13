using UnityEngine;

public class QueenSideRook : Rook
{
    public override void MakeFirstMove()
    {
        base.MakeFirstMove();

        if (m_pieceColor == PieceColor.White)
        {
            GameMode.Instance.whiteRookQueensideMoved = true;
        }
        else
        {
            GameMode.Instance.blackRookQueensideMoved = true;
        }
    }
}
