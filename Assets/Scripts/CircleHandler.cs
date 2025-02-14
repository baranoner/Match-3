using UnityEngine;

public class CircleHandler : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public int Row { get; set; }
    public int Col { get; set; }
    public int Type { get; set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>(); 
    }
    public void Initialize(int row, int col, int type, Color[] typesOfColors)
    {
        UpdatePosition(row, col);
        UpdateType(type, typesOfColors);
    }

    public void UpdatePosition(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public void UpdateType(int newType , Color[] typesOfColors)
    {
        
        if (newType < 0 || newType > typesOfColors.Length)
            return;
        
        Type = newType;
        _spriteRenderer.color = typesOfColors[newType];

    }
}



