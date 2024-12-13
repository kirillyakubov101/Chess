using UnityEngine;

public class GameTile : MonoBehaviour, ISelectable
{
    [SerializeField] private string GameTileName;
    [SerializeField] private Vector2Int TilePos;
    [SerializeField] private bool m_isTileOccupied = false;
    [SerializeField] private Piece m_occupiedPiece;
    [SerializeField] private MeshRenderer m_HoverMat;

    public bool IsTileOccupied { get => m_isTileOccupied; }
    public Piece OccupiedPiece { get => m_occupiedPiece; }

    public void InitializeTile(string tileName,Vector2Int pos)
    {
        GameTileName = tileName;
        gameObject.name = GameTileName;
        TilePos = pos;
    }

    public void OccupyTile(Piece piece)
    {
        if(!m_isTileOccupied)
        {
            m_isTileOccupied = true;
            m_occupiedPiece = piece;
        }
    }

    public void FreeTile()
    {
        m_isTileOccupied = false;
        m_occupiedPiece = null;
    }
      
    public string GetTileName()
    {
        return GameTileName;
    }

    public Vector2Int GetTilePos()
    {
        return TilePos;
    }

    public void Select()
    {
        if (m_HoverMat.material.HasProperty("_Color")) // Ensure the material has a "_Color" property
        {
            Color color = m_HoverMat.material.color;
            color.a = 0.3f; // Set the alpha value
            m_HoverMat.material.color = color;
        }
    }

    public void Deselet()
    {
        if (m_HoverMat.material.HasProperty("_Color")) // Ensure the material has a "_Color" property
        {
            Color color = m_HoverMat.material.color;
            color.a = 0f; // Set the alpha value
            m_HoverMat.material.color = color;
        }
    }

    public void Highlight()
    {
        if (m_HoverMat.material.HasProperty("_Color")) // Ensure the material has a "_Color" property
        {
            Color color = m_HoverMat.material.color;
            color.a = 0.3f; // Set the alpha value
            m_HoverMat.material.color = color;
        }
    }

    public void UnHighlight()
    {
        if (m_HoverMat.material.HasProperty("_Color")) // Ensure the material has a "_Color" property
        {
            Color color = m_HoverMat.material.color;
            color.a = 0f; // Set the alpha value
            m_HoverMat.material.color = color;
        }
    }
}
