using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class Board : MonoBehaviour
{
    [Header("SpawnContainers")]
    [SerializeField] private Transform Pieces;
    [SerializeField] private Transform Tiles;
    [Header("SpawnParams")]
    [SerializeField] private GameTile m_TilePrefab;
    [SerializeField] private float m_offset = 0f;
    [Header("Pieces-spawnParams")]
    [SerializeField] private Piece[] m_WhitePieces; //0 - pawn | 1- King
    [SerializeField] private Piece[] m_BlackPieces; //0 - pawn | 1- King
    [Header("Active Pieces")]
    [SerializeField] private List<Piece> m_AllWhitePieces = new List<Piece>();
    [SerializeField] private List<Piece> m_AllBlackPieces = new List<Piece>();


    private readonly char[] Files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
    private Dictionary<string, GameTile> AllGameTiles = new Dictionary<string, GameTile>();
    private Vector3 m_spawnVector = Vector3.zero;
    private static Board instance = null;

    public static Board Instance { get => instance; }

    public GameTile GetTile(string name)
    {
        if(AllGameTiles.ContainsKey(name))
        {
            return AllGameTiles[name];
        }

        print("no tile found from board GetTile()" + name);
        return null;
       
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Start()
    //{
    //    InitBoard();
    //}

    public void RemovePiece(Piece piece)
    {
        if(piece.PieceColor == PieceColor.White)
        {
            m_AllWhitePieces.Remove(piece);
        }
        else
        {
            m_AllBlackPieces.Remove(piece);
        }

        Destroy(piece.gameObject);
    }

    public IEnumerator Initialize()
    {
        PopulateGameTilesDictionary();
        yield return PopulateWhiteSide();
        yield return PopulateBlackSide();

        yield return null;
    }


    private IEnumerator PopulateWhiteSide()
    {
        yield return SpawnAllPawns(m_WhitePieces, true); //pawns
        yield return SpawnKing(m_WhitePieces[1], true);  //king
        yield return SpawnPiece(m_WhitePieces[5], AllGameTiles["a1"]); //rook
        yield return SpawnPiece(m_WhitePieces[6], AllGameTiles["h1"]); //rook
        yield return SpawnPiece(m_WhitePieces[3], AllGameTiles["b1"]); //kight
        yield return SpawnPiece(m_WhitePieces[3], AllGameTiles["g1"]); //knight
        yield return SpawnPiece(m_WhitePieces[2], AllGameTiles["c1"]); //bishop
        yield return SpawnPiece(m_WhitePieces[2], AllGameTiles["f1"]); //bishop
        yield return SpawnPiece(m_WhitePieces[4], AllGameTiles["d1"]); //queen
    }

    private IEnumerator PopulateBlackSide()
    {
        yield return SpawnAllPawns(m_BlackPieces, false);
        yield return SpawnKing(m_BlackPieces[1], false);
        yield return SpawnPiece(m_BlackPieces[5], AllGameTiles["a8"]); //rook
        yield return SpawnPiece(m_BlackPieces[6], AllGameTiles["h8"]); //rook
        yield return SpawnPiece(m_BlackPieces[3], AllGameTiles["b8"]); //kight
        yield return SpawnPiece(m_BlackPieces[3], AllGameTiles["g8"]); //knight
        yield return SpawnPiece(m_BlackPieces[2], AllGameTiles["c8"]); //bishop
        yield return SpawnPiece(m_BlackPieces[2], AllGameTiles["f8"]); //bishop
        yield return SpawnPiece(m_BlackPieces[4], AllGameTiles["d8"]); //queen



    }

    private IEnumerator SpawnAllPawns(Piece[] collection, bool isWhite)
    {
        //Spawn Pawns
        int currentChar = 'a';
        int index = isWhite ? 2 : 7;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 8; i++)
        {
            sb.Append((char)currentChar);
            sb.Append(index);
            Piece newPawn = SpawnPiece(collection[0], AllGameTiles[sb.ToString()]);
            if (isWhite)
            {
                m_AllWhitePieces.Add(newPawn);
            }
            else
            {
                m_AllBlackPieces.Add(newPawn);
            }
            sb.Clear();
            currentChar++;
        }

        yield return null;
    }

    private IEnumerator SpawnKing(Piece piece, bool isWhite)
    {
        StringBuilder sb = new StringBuilder();
        int currentChar = 'e';
        int index = isWhite ? 1 : 8;
        sb.Append((char)currentChar);
        sb.Append(index);
        Piece King = SpawnPiece(piece, AllGameTiles[sb.ToString()]);
        GameMode.Instance.AssignKing(King, isWhite);

        if(isWhite)
        {
            m_AllWhitePieces.Add(King);
        }
        else
        {
            m_AllBlackPieces.Add(King);
        }
        yield return null;
    }



    private void PopulateGameTilesDictionary()
    {
        GameTile[] Tiles = GetComponentsInChildren<GameTile>();

        foreach (GameTile tile in Tiles)
        {
            AllGameTiles.Add(tile.GetTileName(), tile); 
        }
    }

    private Vector2Int ConvertPosition(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    private Piece SpawnPiece(Piece piece, GameTile tile)
    {
        Piece newPiece = Instantiate(piece, Pieces);
        newPiece.InitPiece(tile);
        return newPiece;
    }

    //I created it on editor execute once, no need for it again
    private void InitBoard()
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameTile NewInstance = Instantiate(m_TilePrefab, Tiles);
                NewInstance.transform.localPosition = m_spawnVector;

                stringBuilder.Clear();
                stringBuilder.Append(Files[j]);
                stringBuilder.Append(i + 1);

                Vector2Int TilePosLocal = ConvertPosition(NewInstance.transform.position);
                NewInstance.InitializeTile(stringBuilder.ToString(), TilePosLocal);
                m_spawnVector.x += m_offset;
            }
            m_spawnVector.x = 0f;
            m_spawnVector.z += m_offset;
        }
    }
}
