using UnityEngine;

public class KingSideRook : Rook
{
    public override void MakeFirstMove()
    {
        base.MakeFirstMove();

        if (m_pieceColor == PieceColor.White)
        {
            GameMode.Instance.whiteRookKingsideMoved = true;
        }
        else
        {
            GameMode.Instance.blackRookKingsideMoved = true;
        }
    }
}
