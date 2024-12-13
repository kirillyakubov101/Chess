using UnityEngine;

public class Rook : Piece
{
    protected bool m_firstMove = true;

    public bool FirstMove { get => m_firstMove; }

    public virtual void MakeFirstMove()
    {
        m_firstMove = false;
    }
}
