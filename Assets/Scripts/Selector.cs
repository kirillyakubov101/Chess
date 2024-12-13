using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField] private LayerMask TileMask = new LayerMask();
    [SerializeField] private GameTile m_currentSelectedGameTile;
    [SerializeField] private Piece m_currentSelectedPiece;

    private ISelectable CurrentHover = null; //only for those you hover
    private ISelectable CurrentSelect = null; //only for those you click
    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }


    private void Update()
    {
        if (!GameMode.Instance.IsPlayerTurn()) return;

        TraceForHover();
        if (Input.GetMouseButtonDown(0))
        {
            //if you click on Any Tile first time
            if(CurrentHover != null)
            {
                CurrentSelect = CurrentHover;

                if (m_currentSelectedGameTile != null)
                {
                    m_currentSelectedGameTile.Deselet();
                }

                m_currentSelectedGameTile = CurrentSelect as GameTile;
                
                if(IsTileWithPlayerPiece())
                {
                    m_currentSelectedPiece = m_currentSelectedGameTile.OccupiedPiece;
                }

                else if(m_currentSelectedPiece != null)
                {
                    //TODO: if all will workout, refactor this function for just the GOALTile / Piece
                    bool canMove = MovementHandler.Instance.CanMove(m_currentSelectedPiece.CurrentTile, m_currentSelectedGameTile, m_currentSelectedPiece);
                    if(canMove)
                    {
                        MovementHandler.Instance.Move(m_currentSelectedPiece.CurrentTile, m_currentSelectedGameTile, m_currentSelectedPiece);
                    }

                    m_currentSelectedPiece = null;
                    m_currentSelectedGameTile = null;
                    CurrentSelect = null;

                }

            }
            
        }
    }

    //I need to know if the tile contains a friendly piece to interact with
    private bool IsTileWithPlayerPiece()
    {
        return m_currentSelectedGameTile.OccupiedPiece && m_currentSelectedGameTile.OccupiedPiece.PieceColor == GameMode.Instance.WhoIsPlayer;
    }

    private bool IsTileWithEnemyPiece()
    {
        return m_currentSelectedGameTile.OccupiedPiece && m_currentSelectedGameTile.OccupiedPiece.PieceColor != GameMode.Instance.WhoIsPlayer;
    }

    private void TraceForHover()
    {
        Ray mouseRay = m_Camera.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(mouseRay, out RaycastHit hitInfo, Mathf.Infinity, TileMask);
        if (hit)
        {
            if(hitInfo.transform.TryGetComponent(out ISelectable selectable))
            {
                if (CurrentHover != null && selectable != CurrentHover && CurrentHover != CurrentSelect)
                {
                    CurrentHover.UnHighlight();
                }

                CurrentHover = selectable;
                CurrentHover.Highlight();
            }
        }
        else
        {
            if (CurrentHover != null && CurrentHover != CurrentSelect)
            {
                CurrentHover.UnHighlight();
                CurrentHover = null;
            }
        }
    }

    private void InteractWithTile(GameTile gameTile)
    {
        if (m_currentSelectedGameTile == null && gameTile.OccupiedPiece != null && gameTile.OccupiedPiece.PieceColor == GameMode.Instance.WhoIsPlayer)
        {
            //SHOW all avaialable moves for this piece
            m_currentSelectedGameTile = gameTile;
        }
        //for none occupied tiles
        else if (m_currentSelectedGameTile != null && m_currentSelectedGameTile != gameTile && !gameTile.IsTileOccupied)
        {
            bool canMove = MovementHandler.Instance.CanMove(m_currentSelectedGameTile, gameTile, m_currentSelectedGameTile.OccupiedPiece);
            if (canMove)
            {
                MovementHandler.Instance.Move(m_currentSelectedGameTile, gameTile, m_currentSelectedGameTile.OccupiedPiece);
                GameMode.Instance.CompleteTurn();
            }

            m_currentSelectedGameTile = null;
        }
    }
}
