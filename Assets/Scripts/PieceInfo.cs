using System.Collections.Generic;
using UnityEngine;

public enum PieceName
{
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn
}

[CreateAssetMenu(fileName = "NewPieceInfo", menuName = "Chess/PieceInfo")]
public class PieceInfo : ScriptableObject
{
    [Header("Basic Information")]
    public PieceName pieceName; // e.g., "Pawn", "Rook"
    public Mesh pieceMesh; // The 3D model for this piece
  


}
