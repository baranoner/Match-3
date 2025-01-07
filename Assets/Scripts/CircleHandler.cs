using DG.Tweening;
using UnityEngine;

public class CircleHandler : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int Type { get; private set; } 

    public void Initialize(int row, int col, int type)
    {
        UpdatePosition(row, col);
        UpdateType(type);
    }

    public void UpdatePosition(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public void UpdateType(int newType)
    {
        Type = newType;
        
    }
     private void OnDestroy()
    {
        
        transform.DOKill();
    }
}



