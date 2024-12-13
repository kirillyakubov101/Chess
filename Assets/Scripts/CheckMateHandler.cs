using UnityEngine;

public enum DangerDirections
{
    Forward,
    Backward,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Left,
    Right
}


public static class CheckMateHandler
{
    public static bool CheckForCheck(DangerDirections direction, int maxIndex, RaycastHit[] arr, PieceColor friendlyColor,Piece possiblePieceToCapture = null)
    {
        switch (direction)
        {
            case DangerDirections.Right:
            case DangerDirections.Backward:
            case DangerDirections.Left:
            case DangerDirections.Forward:
                for (int i = maxIndex; i >= 0; i--)
                {
                    if (arr[i].transform.CompareTag("Ghost"))
                    {
                        //Debug.Log("You hit a friendly ghost!");
                        return false;
                    }
                    else if (arr[i].transform.TryGetComponent(out Piece piece))
                    {
                        if (piece.PieceColor == friendlyColor)
                        {
                            //Debug.Log($"{piece.GetPieceName()} Friend");
                            return false;
                        }
                        else
                        {
                            if(piece == possiblePieceToCapture) { continue; }
                            else if (piece.GetPieceName() == PieceName.Queen || piece.GetPieceName() == PieceName.Rook)
                            {
                                //Debug.Log($"{piece.GetPieceName()} is checking you!");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("CheckForCheck : Super Weird object found! ");
                    }

                }
                break;

            case DangerDirections.TopRight:
            case DangerDirections.BottomLeft:
            case DangerDirections.BottomRight:
            case DangerDirections.TopLeft:
                for (int i = maxIndex; i >= 0; i--)
                {
                    if (arr[i].transform.CompareTag("Ghost"))
                    {
                        return false;
                    }
                    else if (arr[i].transform.TryGetComponent(out Piece piece))
                    {
                        if (piece.PieceColor == friendlyColor)
                        {
                            //Debug.Log($"{piece.GetPieceName()} Friend");
                            return false;
                        }
                        else
                        {
                            if (piece == possiblePieceToCapture) { continue; }
                            else if (piece.GetPieceName() == PieceName.Queen || piece.GetPieceName() == PieceName.Bishop)
                            {
                                //Debug.Log($"{piece.GetPieceName()} is checking you!");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("CheckForCheck : Super Weird object found! ");
                    }

                }
                break;
        }


        return false;
        
    }
}
