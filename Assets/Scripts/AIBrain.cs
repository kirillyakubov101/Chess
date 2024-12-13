using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AIBrain : MonoBehaviour
{
    private string stockfishPath = @"Python/stockfish/stockfish-windows-x86-64-avx2.exe";  // Correct path to Stockfish executable
    private PieceColor AIColor;
    private string AIColorSymbol;
    public string currentFEN;

    public string BestMove;
    public string currentTile;
    public string goalTile;
    // Event to notify when the process is complete
    public event Action<string> OnBestMoveCalculated;

    // Start is called before the first frame update
    void Start()
    {
        OnBestMoveCalculated += BestMoveCalculatedHandler;
        AIColor = GameMode.Instance.WhoIsPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;

        if(AIColor == PieceColor.Black)
        {
            AIColorSymbol = " b ";
        }
        else
        {
            AIColorSymbol = " w ";
        }

       
    }

    private void BestMoveCalculatedHandler(string bestMove)
    {
        BestMove = bestMove;
        currentTile = bestMove.Substring(0, 2);
        goalTile = bestMove.Substring(2,2);

        var currentTileObj =  Board.Instance.GetTile(currentTile);
        var goalTileObj = Board.Instance.GetTile(goalTile);

        MovementHandler.Instance.Move(currentTileObj, goalTileObj, currentTileObj.OccupiedPiece);
    }

    public async void GetBestMoveAsync(string fen)
    {
        try
        {
            string bestMove = await Task.Run(() => GetBestMoveProcess(fen));
            OnBestMoveCalculated?.Invoke(bestMove); // Trigger the event with the result
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error calculating best move: {ex.Message}");
        }
    }

    public string GetBestMoveProcess(string fen)
    {
        // Start the Stockfish process
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = stockfishPath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = startInfo })
        {
            process.Start();

            // Send UCI commands to Stockfish
            StreamWriter inputWriter = process.StandardInput;
            StreamReader outputReader = process.StandardOutput;

            inputWriter.WriteLine("uci");
            inputWriter.Flush();
            ReadUntil(outputReader, "uciok");

            inputWriter.WriteLine($"position fen {fen}");
            inputWriter.Flush();

            inputWriter.WriteLine("go movetime 2000");
            inputWriter.Flush();

            string bestMoveLine = ReadUntil(outputReader, "bestmove");
            string bestMove = null;

            if (bestMoveLine.StartsWith("bestmove"))
            {
                bestMove = bestMoveLine.Split(' ')[1];
            }

            inputWriter.WriteLine("quit");
            inputWriter.Flush();

            process.WaitForExit();

            return bestMove ?? "No move found";
        }
    }

    private string ReadUntil(StreamReader reader, string keyword)
    {
        StringBuilder output = new StringBuilder();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            //Debug.Log($"Stockfish output: {line}");
            if (line.StartsWith(keyword))
            {
                return line;
            }
        }
        return null;
    }


    public bool testAI = false;
    private void Update()
    {
        if(testAI)
        {
            GetBestMove();
            testAI = false;

        }
    }

    // Coroutine to get the best move from Lichess API
    private void GetBestMove()
    {
        string fen = GenerateFEN();
        currentFEN = fen;

        GetBestMoveAsync(fen);

    }

    public string GenerateFEN()
    {
        StringBuilder fen = new StringBuilder();

        // Iterate over the ranks (rows) of the board, starting from rank 8
        for (int rank = 8; rank >= 1; rank--)
        {
            int emptySquares = 0;

            // Iterate over the files (columns) of the board
            for (char file = 'a'; file <= 'h'; file++)
            {
                string squareName = $"{file}{rank}";
                GameTile tile = Board.Instance.GetTile(squareName);

                if (tile.IsTileOccupied)
                {
                    // If there were empty squares, add their count to the FEN string
                    if (emptySquares > 0)
                    {
                        fen.Append(emptySquares);
                        emptySquares = 0;
                    }

                    // Add the piece's FEN symbol
                    char symbokFen = ToFENSymbol(tile.OccupiedPiece.GetPieceName(), tile.OccupiedPiece.PieceColor);
                    fen.Append(symbokFen);
                }
                else
                {
                    emptySquares++;
                }
            }

            // Add remaining empty squares at the end of the rank
            if (emptySquares > 0)
            {
                fen.Append(emptySquares);
            }

            // Separate ranks with '/'
            if (rank > 1)
            {
                fen.Append('/');
            }
        }

        // Add active color (replace 'w' or 'b' with the actual color)
        fen.Append(AIColorSymbol);

        // Add castling rights, en passant square, halfmove clock, and fullmove number
        // (Replace placeholders with actual values)
        string castleString = GameMode.Instance.GenerateCastleRight();
        fen.Append(castleString);
        fen.Append(" " + GameMode.Instance.EnPassantSquare + " ");

        fen.Append(" " + GameMode.Instance.HalfMoves.ToString() + " ");
        fen.Append(" " + GameMode.Instance.FullMoves.ToString());

        return fen.ToString();
    }

    private char ToFENSymbol(PieceName piece,PieceColor color)
    {
        if(color == PieceColor.White)
        {
            switch (piece)
            {
                case PieceName.Pawn:
                    return 'P';
                case PieceName.Knight:
                    return 'N';
                case PieceName.Bishop:
                    return 'B';
                case PieceName.Rook:
                    return 'R';
                case PieceName.Queen:
                    return 'Q';
                case PieceName.King:
                    return 'K';
                default:
                    throw new ArgumentOutOfRangeException(nameof(piece), piece, "Invalid PieceName");
            }
        }
        else
        {
            switch (piece)
            {
                case PieceName.Pawn:
                    return 'p';
                case PieceName.Knight:
                    return 'n';
                case PieceName.Bishop:
                    return 'b';
                case PieceName.Rook:
                    return 'r';
                case PieceName.Queen:
                    return 'q';
                case PieceName.King:
                    return 'k';
                default:
                    throw new ArgumentOutOfRangeException(nameof(piece), piece, "Invalid PieceName");
            }
        }
    }
}
