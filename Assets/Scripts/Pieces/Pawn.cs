using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public List<Vector2Int> SpecialCaptureMoves = new List<Vector2Int>();
    public bool DidFirstMove = false;
    Vector2Int FirstSpecialMove;



    protected override void Start()
    {
        base.Start();

        SpecialCaptureMoves.Add(new Vector2Int(1 * m_colorMultiplier, 1 * m_colorMultiplier));
        SpecialCaptureMoves.Add(new Vector2Int(-1 * m_colorMultiplier, 1 * m_colorMultiplier));

        FirstSpecialMove = new Vector2Int(0, 2 * m_colorMultiplier);

        ListOfMoves.Add(FirstSpecialMove);
    }

    public void PawnMoveFirstTime()
    {
        DidFirstMove = true;
        ListOfMoves.Remove(FirstSpecialMove);
    }
}
