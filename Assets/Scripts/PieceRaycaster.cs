using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceRaycaster : MonoBehaviour
{
    [SerializeField] private float m_offsetRaycastOriginY;
    [SerializeField] private LayerMask LayerMask;
    [SerializeField] private float m_raycastDistance = 80f;
    [SerializeField] private Piece PossibleToCaptureEnemy = null;

    private int m_currentRightLimitIndex = 0;
    private RaycastHit[] raycastHits = new RaycastHit[8];
    public GameObject[] objects = new GameObject[8];

    public void RayCastFromKing(out bool wasHit, GameTile newTile, Piece CurrentPiece)
    {
        Array.Clear(raycastHits, 0, 8);
        Array.Clear(objects, 0, 8);
        bool isChecked = false;
        //position the ghost of the piece
        Vector3 ghostPos = new Vector3(newTile.transform.position.x, CurrentPiece.transform.position.y, newTile.transform.position.z);

        CurrentPiece.EnableGhost(ghostPos);
        PossibleToCaptureEnemy = null;
        PossibleToCaptureEnemy = newTile.OccupiedPiece;

        //cache the king pos
        Vector3 origin = GameMode.Instance.GetCurrentTurnKing().transform.position;
        origin.y += m_offsetRaycastOriginY;

        //raycast Forward
        DirectionRayCast(ref isChecked, CurrentPiece, origin, Vector3.forward, DangerDirections.Forward);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        //raycast Backward
        DirectionRayCast(ref isChecked, CurrentPiece, origin, Vector3.back, DangerDirections.Backward);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, CurrentPiece, origin, Vector3.left, DangerDirections.Left);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, CurrentPiece, origin, Vector3.right, DangerDirections.Right);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, CurrentPiece, origin, new Vector3(-1,0,1), DangerDirections.TopLeft);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, CurrentPiece, origin, new Vector3(1, 0, 1), DangerDirections.TopRight);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, CurrentPiece, origin, new Vector3(-1, 0, -1), DangerDirections.BottomLeft);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, CurrentPiece, origin, new Vector3(1, 0, -1), DangerDirections.BottomRight);
        if (isChecked)
        {
            wasHit = true;
            CurrentPiece.DisableGhost();
            return;
        }



        CurrentPiece.DisableGhost();
        wasHit = false;
    }

    private void DirectionRayCast(ref bool isChecked, Piece CurrentPiece, Vector3 origin, Vector3 Direction, DangerDirections dangerDirection)
    {
        int hits = Physics.RaycastNonAlloc(origin, Direction, raycastHits, m_raycastDistance, LayerMask);
        if (hits == 0)
        {
            isChecked = false;
            m_currentRightLimitIndex = 0;
            return;
        }
        Array.Sort(raycastHits, 0, hits, DistanceComparer.Instance);

        //you need to find the danger piece and check if your ghost could take her out / make sure to account for the pawn weird movements
        //if (raycastHits[0].transform.CompareTag("Ghost"))
        //{
        //    CurrentPiece.DisableGhost();
        //    isChecked = false;
        //    m_currentRightLimitIndex = 0;
        //    print("the danger peice is about to be taken 22");
        //    return;
        //}


        //test
        for (int i = 0; i < 8; i++)
        {
            if (raycastHits[i].transform)
            {
                objects[i] = raycastHits[i].transform.gameObject;
            }
            else
            {
                objects[i] = null;
            }
        }
        if (hits > 0)
        {
            m_currentRightLimitIndex = hits - 1;
            isChecked = CheckMateHandler.CheckForCheck(dangerDirection, m_currentRightLimitIndex, raycastHits, GameMode.Instance.GetCurrentTurnKing().PieceColor, PossibleToCaptureEnemy);
            m_currentRightLimitIndex = 0;
            if (isChecked)
            {
                CurrentPiece.DisableGhost();
                isChecked = true;
            }
        }
    }

    private class DistanceComparer : IComparer<RaycastHit>
    {
        public static readonly DistanceComparer Instance = new DistanceComparer();

        private DistanceComparer() { }
        public int Compare(RaycastHit x, RaycastHit y)
        {
            // Compare distances in descending order
            return y.distance.CompareTo(x.distance);
        }
    }

    public void RayCastForKingNextTile(out bool wasHit, GameTile newTile)
    {
        if (CheckForKingAndPawns(newTile))
        {
            wasHit = true;
            return;
        }

        Array.Clear(raycastHits, 0, 8);
        Array.Clear(objects, 0, 8);
        bool isChecked = false;
        wasHit = true;

        Piece currentKing = GameMode.Instance.GetCurrentTurnKing();   

        //position the ghost of the piece
        Vector3 ghostPos = new Vector3(newTile.transform.position.x, currentKing.transform.position.y, newTile.transform.position.z);
        currentKing.EnableGhost(ghostPos);
        PossibleToCaptureEnemy = null;
        PossibleToCaptureEnemy = newTile.OccupiedPiece;

        Vector3 origin = ghostPos;
        origin.y += m_offsetRaycastOriginY;

        //raycast Forward
        DirectionRayCast(ref isChecked, currentKing, origin, Vector3.forward, DangerDirections.Forward);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        //raycast Backward
        DirectionRayCast(ref isChecked, currentKing, origin, Vector3.back, DangerDirections.Backward);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, currentKing, origin, Vector3.left, DangerDirections.Left);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, currentKing, origin, Vector3.right, DangerDirections.Right);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, currentKing, origin, new Vector3(-1, 0, 1), DangerDirections.TopLeft);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, currentKing, origin, new Vector3(1, 0, 1), DangerDirections.TopRight);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, currentKing, origin, new Vector3(-1, 0, -1), DangerDirections.BottomLeft);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }

        DirectionRayCast(ref isChecked, currentKing, origin, new Vector3(1, 0, -1), DangerDirections.BottomRight);
        if (isChecked)
        {
            wasHit = true;
            currentKing.DisableGhost();
            return;
        }



        currentKing.DisableGhost();
        wasHit = false;
    }

    private bool CheckForKingAndPawns(GameTile newTile)
    {
        Piece currentKing = GameMode.Instance.GetCurrentTurnKing();
        PieceColor currentKingColor = currentKing.PieceColor;
        Vector3 simulatedPos = new Vector3(newTile.transform.position.x, GameMode.Instance.GetCurrentTurnKing().transform.position.y, newTile.transform.position.z);

        //check vertical and horizontal collisions with enemy king
        // Directions: [row offset, col offset]
        int[,] kingDirections = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
        int[,] pawnDirections = currentKingColor == PieceColor.Black ? new int[,] { { -1, -1 }, { -1, 1 } } : new int[,] { { 1, -1 }, { 1, 1 } };

        // Convert kingPos to indices
        int kingRow = newTile.GetTileName()[1] - '1'; // Convert row to 0-7
        int kingCol = newTile.GetTileName()[0] - 'a'; // Convert col to 0-7
                                                      // Check surrounding tiles for enemy king
        for (int i = 0; i < kingDirections.GetLength(0); i++)
        {
            int newRow = kingRow + kingDirections[i, 0];
            int newCol = kingCol + kingDirections[i, 1];

            if (IsValidTile(newRow, newCol))
            {
                string tile = GetTileName(newRow, newCol);
                
                if (Board.Instance.GetTile(tile) && Board.Instance.GetTile(tile).IsTileOccupied && Board.Instance.GetTile(tile).OccupiedPiece.GetPieceName() == PieceName.King && Board.Instance.GetTile(tile).OccupiedPiece != currentKing)
                {
                    return true; // Under attack by enemy king
                }
            }
        }

        // Check pawn attack directions
        for (int i = 0; i < pawnDirections.GetLength(0); i++)
        {
            int newRow = kingRow + pawnDirections[i, 0];
            int newCol = kingCol + pawnDirections[i, 1];

            if (IsValidTile(newRow, newCol))
            {
                string tile = GetTileName(newRow, newCol);
                if (Board.Instance.GetTile(tile) && Board.Instance.GetTile(tile).IsTileOccupied && Board.Instance.GetTile(tile).OccupiedPiece.GetPieceName() == PieceName.Pawn && Board.Instance.GetTile(tile).OccupiedPiece.PieceColor != currentKingColor)
                {
                    return true; // Under attack by enemy pawn
                }
            }
        }

        return false; // Not under attack
    }

    private bool IsValidTile(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    private string GetTileName(int row, int col)
    {
        char colChar = (char)('a' + col);
        char rowChar = (char)('1' + row);
        return $"{colChar}{rowChar}";
    }
}


////test
//for (int i = 0; i < 8; i++)
//{
//    if (raycastHits[i].transform)
//    {
//        objects[i] = raycastHits[i].transform.gameObject;
//    }
//    else
//    {
//        objects[i] = null;
//    }
//}

//Debug.DrawLine(origin, origin + (Vector3.forward * m_raycastDistance), Color.red, 100f);