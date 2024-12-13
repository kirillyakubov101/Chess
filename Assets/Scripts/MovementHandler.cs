using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    private static MovementHandler instance = null;
    public static MovementHandler Instance { get => instance; }

    [field: SerializeField] public bool MovementInProgress { get; private set; } = false;
    [SerializeField] private float m_moveSpeed = 3f;
    [SerializeField] private float m_movementMaxTime = 0.5f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public bool CanMove(GameTile Current, GameTile Goal, Piece piece)
    {
        bool canMove = false;
        bool isPathClear = true;
        bool moveWillCauseCheck = false;

        int deltaX, deltaY, currentX, currentY, goalX, goalY;
        InitDelta(Current, Goal, out deltaX, out deltaY, out currentX, out currentY, out goalX, out goalY);

        canMove = piece.ListOfMoves
               .Any(move => move.x == deltaX && move.y == deltaY);

        if(piece is Pawn pawn)
        {
            canMove = Goal.IsTileOccupied && ((Pawn)piece).SpecialCaptureMoves
                .Any(move => move.x == deltaX && move.y == deltaY) || piece.ListOfMoves
                .Any(move => move.x == deltaX && move.y == deltaY) && !Goal.IsTileOccupied;
        }
        else
        {
            canMove = piece.ListOfMoves
               .Any(move => move.x == deltaX && move.y == deltaY);
        }

        //Knight does not care about blocked path
        if (piece.PieceInfo.pieceName != PieceName.Knight)
        {
            isPathClear = !IsPathBlocked(currentX, currentY, goalX, goalY, deltaX, deltaY);
        }

        //Check if move will cause check Or will keep the player in check after move
        if(piece.GetPieceName() != PieceName.King)
        {
            GameMode.Instance.Raycaster.RayCastFromKing(out moveWillCauseCheck, Goal, piece);
        }
        //if piece is KING
        else
        {
            GameMode.Instance.Raycaster.RayCastForKingNextTile(out moveWillCauseCheck,Goal);
            if(isPathClear)
            {
                CanCastle(Goal, piece);
            }
            
        }

        return canMove && isPathClear && !moveWillCauseCheck;
    }

    public void Move(GameTile Current, GameTile Goal, Piece piece)
    {
        StartCoroutine(MovementProcess(Current, Goal, piece));
    }

    private void InitDelta(GameTile Current, GameTile Goal, out int deltaX, out int deltaY, out int currentXPos, out int currentYPos, out int goalXPos, out int goalYPos)
    {
        string currentTileName = Current.GetTileName();
        string goalTileName = Goal.GetTileName();

        currentXPos = (int)currentTileName[0]; //letters
        currentYPos = int.Parse(currentTileName[1].ToString()); //numbers

        goalXPos = (int)goalTileName[0]; //letters
        goalYPos = int.Parse(goalTileName[1].ToString()); //numbers

        deltaX = goalXPos - currentXPos;
        deltaY = goalYPos - currentYPos;

    }

    private bool IsPathBlocked(int currentX, int currentY, int goalX, int goalY, int deltaX, int deltaY)
    {
        // Determine the direction of movement
        int stepX = deltaX == 0 ? 0 : deltaX / Mathf.Abs(deltaX);
        int stepY = deltaY == 0 ? 0 : deltaY / Mathf.Abs(deltaY);

        // Step through the path, excluding the goal tile
        while (currentX != goalX || currentY != goalY)
        {
            currentX += stepX;
            currentY += stepY;

            // Check if we've reached the goal
            if (currentX == goalX && currentY == goalY)
                break;

            // Check if a piece is blocking this tile
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append((char)currentX);
            stringBuilder.Append(currentY.ToString());
            GameTile tile = Board.Instance.GetTile(stringBuilder.ToString());
            if (tile.IsTileOccupied)
            {
                return true;
            }
        }
        return false; // No blocking pieces found
    }

    private IEnumerator MovementProcess(GameTile Current, GameTile Goal, Piece piece)
    {
        if (Goal.IsTileOccupied)
        {
            yield return CapturePiece(Goal);
        }
        else if(piece is Pawn)
        {
            GameMode.Instance.ResetHalfMove();
        }
        else
        {
            GameMode.Instance.IncrementHalfMove();
        }
        yield return MoveToNewTile(piece, Goal);
        yield return UpdateTileAfterMove(Current, Goal, piece);

        if(piece.PieceColor == PieceColor.Black)
        {
            GameMode.Instance.IncrementFullMove();
        }
    }

    private IEnumerator CapturePiece(GameTile Goal)
    {
        Board.Instance.RemovePiece(Goal.OccupiedPiece);
        Goal.FreeTile();
        GameMode.Instance.ResetHalfMove();

        yield return null;
    }

    private IEnumerator MoveToNewTile(Piece piece, GameTile newTile)
    {
        Vector3 endPos = new Vector3(newTile.transform.position.x, piece.transform.position.y, newTile.transform.position.z);
        float timer = 0;

        while (piece.transform.position != endPos && timer < m_movementMaxTime)
        {
            piece.transform.position = Vector3.Lerp(piece.transform.position, endPos, Time.deltaTime * m_moveSpeed);
            timer += Time.deltaTime;
            yield return null;
        }

        //notify the game mode that the rook made its move castle-wise flag
        if(piece is Rook && (piece as Rook).FirstMove)
        {
            (piece as Rook).MakeFirstMove();
        }
        //notify the game mode that the king  made its move castle-wise flag
        else if (piece is King && (piece as King).FirstMove)
        {
            (piece as King).MakeFirstMove();
        }

        //if the pawn moved, it cannot make the 2 tile move no more
        else if(piece is Pawn && !(piece as Pawn).DidFirstMove)
        {
            (piece as Pawn).PawnMoveFirstTime();

            int currentYPos = int.Parse(piece.CurrentTile.GetTileName()[1].ToString()); //numbers
            int goalYPos = int.Parse(newTile.GetTileName()[1].ToString()); //numbers
            int deltaY = goalYPos - currentYPos;

            if(Mathf.Abs(currentYPos - goalYPos) == 2)
            {
                // Calculate the en passant square
                int enPassantYPos = (currentYPos + goalYPos) / 2; // The rank the pawn passed over
                string enPassantSquare = $"{newTile.GetTileName()[0]}{enPassantYPos}"; // File + passed rank
                GameMode.Instance.EnPassantSquare = enPassantSquare;
            }

        }
        else
        {
            GameMode.Instance.EnPassantSquare = "-";
        }

        piece.transform.position = endPos;
    }

    private IEnumerator UpdateTileAfterMove(GameTile Current, GameTile Goal, Piece piece)
    {
        Current.FreeTile();
        Goal.OccupyTile(piece);
        piece.SetTile(Goal);
        yield return null;
    }

    private bool CanCastle(GameTile Goal, Piece piece)
    {
        if (Goal.IsTileOccupied) { return false; }
        PieceColor kingColor = piece.PieceColor;
        bool MoveCannotBeDone = false;
        bool didCastle = false;
        //for WHITE
        if (kingColor == PieceColor.White)
        {
            if (GameMode.Instance.whiteKingMoved) { return false; }
            Vector2Int WhiteKingStartPos = piece.CurrentTile.GetTilePos();
            string[] allowedKingTiles = { "g1", "c1" };

            bool isKingMoveLegitToCastle = allowedKingTiles.Contains(Goal.GetTileName());
            if(isKingMoveLegitToCastle)
            {
                switch (Goal.GetTileName())
                {
                    case "c1":
                        if (GameMode.Instance.whiteRookQueensideMoved) { return false; }
                        GameTile d1_tile = Board.Instance.GetTile("d1");
                        GameTile c1_tile = Board.Instance.GetTile("c1");
                        GameTile b1_tile = Board.Instance.GetTile("b1");

                        if (d1_tile.IsTileOccupied || c1_tile.IsTileOccupied || b1_tile.IsTileOccupied) { return false; }

                        if (MoveCannotBeDone) { return false; }
                        GameMode.Instance.Raycaster.RayCastForKingNextTile(out MoveCannotBeDone, d1_tile);
                        if (MoveCannotBeDone) { return false; }
                        GameMode.Instance.Raycaster.RayCastForKingNextTile(out MoveCannotBeDone, c1_tile);
                        if (MoveCannotBeDone) { return false; }

                        Piece QueenSideRook = Board.Instance.GetTile("a1").OccupiedPiece;

                        didCastle = Castle(piece, QueenSideRook, c1_tile, d1_tile);
                        if (didCastle) { return true; }

                        break;
                    case "g1":
                        if (GameMode.Instance.whiteRookKingsideMoved) { return false; }
                        GameTile g1_tile = Board.Instance.GetTile("g1");
                        GameTile f1_tile = Board.Instance.GetTile("f1");

                        GameMode.Instance.Raycaster.RayCastForKingNextTile(out MoveCannotBeDone, g1_tile);
                        if (MoveCannotBeDone) { return false; }
                        GameMode.Instance.Raycaster.RayCastForKingNextTile(out MoveCannotBeDone, f1_tile);
                        if (MoveCannotBeDone) { return false; }

                        Piece KingsideRook = Board.Instance.GetTile("h1").OccupiedPiece;

                        didCastle =  Castle(piece, KingsideRook, g1_tile, f1_tile);
                        if (didCastle) { return true; }

                        break;
                }
                 
            }
        }
        ///for BLACK
        else
        {

        }
        return false;
    }

    private bool Castle(Piece KingPiece, Piece RookPiece, GameTile KingGoalTile, GameTile RookGoalTile)
    {
        if (RookPiece == null || KingPiece == null)
        {
            print("Castle: piece is null???");
            return false;
        }

        Move(RookPiece.CurrentTile, RookGoalTile, RookPiece);
        Move(KingPiece.CurrentTile, KingGoalTile, KingPiece);

        return true;
    }

    private bool IsPathBlocked(GameTile Current, GameTile Goal)
    {
        InitDelta(Current, Goal,out int deltaX, out int deltaY, out int currentPosX, out int currentPosY, out int goalXpos, out int goalYpos);
        return IsPathBlocked(currentPosX, currentPosY, goalXpos, goalYpos, deltaX, deltaY);
    }


}
