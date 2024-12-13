using System.Collections;
using System.Text;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public bool bTesting;
    [SerializeField] private PieceColor m_WhoIsPlayer;
    [SerializeField] private bool m_isGameOver = false;
    [SerializeField] private PieceColor m_CurrentTurn = PieceColor.White;
    [SerializeField] private Piece m_whiteKing;
    [SerializeField] private Piece m_blackKing;

    [Header("Castle")]

    [field: SerializeField] public bool whiteKingMoved = false;
    [field: SerializeField] public bool whiteRookKingsideMoved = false;
    [field: SerializeField] public bool whiteRookQueensideMoved = false;

    [field: SerializeField] public bool blackKingMoved = false;
    [field: SerializeField] public bool blackRookKingsideMoved = false;
    [field: SerializeField] public bool blackRookQueensideMoved = false;
    [Header("EN Peasent")]
    public string EnPassantSquare = "-";
    [Header("Full Moves/ Half Moves")]
    public int FullMoves = 1;
    public int HalfMoves = 0;
   


    private static GameMode instance = null;

    private PieceRaycaster m_Raycaster;
    public static GameMode Instance { get => instance;}
    public PieceColor CurrentTurn { get => m_CurrentTurn; }
    public PieceColor WhoIsPlayer { get => m_WhoIsPlayer; }
    public PieceRaycaster Raycaster { get => m_Raycaster; set => m_Raycaster = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_Raycaster = GetComponent<PieceRaycaster>();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            m_CurrentTurn = m_CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
            m_WhoIsPlayer = m_WhoIsPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    }

    public void CompleteTurn()
    {
        if (bTesting) { return; }
        m_CurrentTurn = m_CurrentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    public void IncrementHalfMove()
    {
        HalfMoves++;
    }

    //pawn moved or there was a capture
    public void ResetHalfMove()
    {
        HalfMoves = 0;
    }

    public void IncrementFullMove()
    {
        FullMoves++;
    }

    public bool IsPlayerTurn()
    {
        return m_CurrentTurn == WhoIsPlayer;
    }

    public void AssignKing(Piece king, bool isWhite)
    {
        if(isWhite)
        {
            m_whiteKing = king;
        }
        else
        {
            m_blackKing = king;
        }
    }

    public Piece GetCurrentTurnKing()
    {
        return m_CurrentTurn == PieceColor.White ? m_whiteKing : m_blackKing;
    }

    private IEnumerator Start()
    {
        yield return Board.Instance.Initialize();
        yield return BeginGame();
    }

    private IEnumerator BeginGame()
    {
        while (!m_isGameOver)
        {
            yield return null;
        }
    }

    public string GenerateCastleRight()
    {
        StringBuilder castlingRights = new StringBuilder();

        if (!whiteKingMoved && !whiteRookKingsideMoved)
            castlingRights.Append('K');
        if (!whiteKingMoved && !whiteRookQueensideMoved)
            castlingRights.Append('Q');
        if (!blackKingMoved && !blackRookKingsideMoved)
            castlingRights.Append('k');
        if (!blackKingMoved && !blackRookQueensideMoved)
            castlingRights.Append('q');

        // If no castling rights remain, append '-'
        if (castlingRights.Length == 0)
            castlingRights.Append('-');

        return castlingRights.ToString();
    }

}
