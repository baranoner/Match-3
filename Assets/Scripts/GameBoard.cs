using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GameBoard : MonoBehaviour
{
    private Vector2[,] gameBoardPositions;
    private GameObject[,] gameBoardCircles;

    private int rows;
    private int cols;
    private float spacing;
    GameObject[] circlePrefabs;

public void Initialize(int rows, int cols, float spacing, GameObject[] circlePrefabs)
    {
        this.rows = rows;
        this.cols = cols;
        this.spacing = spacing;
        this.circlePrefabs = circlePrefabs;

        gameBoardPositions = new Vector2[rows, cols];
        gameBoardCircles = new GameObject[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                gameBoardPositions[i, j] = new Vector2(i * spacing, j * spacing);
            }
        }
    }
public void PopulateGameBoard(GameObject[] circlePrefabs, float dropDuration, float staggerDelay)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                CreateCirclesAtStart(circlePrefabs, i, j, dropDuration, staggerDelay);
            }
        }
    }
private void CreateCirclesAtStart(GameObject[] circlePrefabs, int row, int col, float dropDuration, float staggerDelay)
{
    Vector2 finalPosition = gameBoardPositions[row, col];
    Vector2 startPosition = finalPosition + Vector2.up * 15;

    int randomType;
    GameObject prefab;
    bool isValid;

    do
    {
        
        randomType = Random.Range(0, circlePrefabs.Length);
        prefab = circlePrefabs[randomType];

        
        isValid = IsTypeValid(row, col, randomType);

    } while (!isValid);

    
    GameObject circle = Instantiate(prefab, startPosition, Quaternion.identity);
    circle.transform.parent = this.transform;

    
    CircleHandler handler = circle.GetComponent<CircleHandler>();
    handler.Initialize(row, col, randomType);

    
    gameBoardCircles[row, col] = circle;

    
    circle.transform.DOMove(finalPosition, dropDuration)
        .SetEase(Ease.OutBounce)
        .SetDelay(staggerDelay * (row * cols + col));
}
public void CreateCircle(GameObject[] circlePrefabs, int row, int col, float dropDuration, float staggerDelay)
{
    Vector2 finalPosition = gameBoardPositions[row, col];
    Vector2 startPosition = finalPosition + Vector2.up * 15;

    int randomType;
    GameObject prefab;

    randomType = Random.Range(0, circlePrefabs.Length);
    prefab = circlePrefabs[randomType];
    

    
    GameObject circle = Instantiate(prefab, startPosition, Quaternion.identity);
    circle.transform.parent = this.transform;

    
    CircleHandler handler = circle.GetComponent<CircleHandler>();
    handler.Initialize(row, col, randomType);

    
    gameBoardCircles[row, col] = circle;

    
    circle.transform.DOMove(finalPosition, dropDuration)
        .SetEase(Ease.OutBounce)
        .SetDelay(staggerDelay);
}
private bool IsTypeValid(int row, int col, int type)
{
    
    if (col >= 2) 
    {
        CircleHandler left1 = gameBoardCircles[row, col - 1]?.GetComponent<CircleHandler>();
        CircleHandler left2 = gameBoardCircles[row, col - 2]?.GetComponent<CircleHandler>();

        if (left1 != null && left2 != null && left1.Type == type && left2.Type == type)
        {
            return false; 
        }
    }

    
    if (row >= 2) 
    {
        CircleHandler top1 = gameBoardCircles[row - 1, col]?.GetComponent<CircleHandler>();
        CircleHandler top2 = gameBoardCircles[row - 2, col]?.GetComponent<CircleHandler>();

        if (top1 != null && top2 != null && top1.Type == type && top2.Type == type)
        {
            return false; 
        }
    }

    return true; 
}   
public void SwapCirclesBoardLogic(int rowA, int colA, int rowB, int colB)
    {
        GameObject temp = gameBoardCircles[rowA, colA];
        gameBoardCircles[rowA, colA] = gameBoardCircles[rowB, colB];
        gameBoardCircles[rowB, colB] = temp;
    }
public bool ArePositionsAdjacent(int rowA, int colA, int rowB, int colB)
    {
        return (Mathf.Abs(rowA - rowB) == 1 && colA == colB) || 
               (Mathf.Abs(colA - colB) == 1 && rowA == rowB);
    }
private List<CircleHandler> GetMatches()
{
    List<CircleHandler> matches = new List<CircleHandler>();

    
    for (int row = 0; row < rows; row++)
    {
        List<CircleHandler> rowMatches = CheckLine(row, 0, 0, 1); 
        matches.AddRange(rowMatches);
    }

    // Check columns for matches
    for (int col = 0; col < cols; col++)
    {
        List<CircleHandler> colMatches = CheckLine(0, col, 1, 0); 
        matches.AddRange(colMatches);
    }

    return matches;
}
private List<CircleHandler> CheckLine(int startRow, int startCol, int rowDelta, int colDelta)
{
    List<CircleHandler> lineMatches = new List<CircleHandler>();
    int matchType = -1;
    int matchCount = 0;

    for (int i = 0; i < Mathf.Max(rows, cols); i++)
    {
        int row = startRow + rowDelta * i;
        int col = startCol + colDelta * i;

        
        if (row >= rows || col >= cols || row < 0 || col < 0)
        {
            if (matchCount >= 3)
                return lineMatches;

            matchType = -1;
            matchCount = 0;
            lineMatches.Clear();
            continue;
        }

        CircleHandler handler = gameBoardCircles[row, col]?.GetComponent<CircleHandler>();
        if (handler == null)
            break;

        if (handler.Type == matchType)
        {
            matchCount++;
            lineMatches.Add(handler);
        }
        else
        {
            if (matchCount >= 3)
                return lineMatches;

            matchType = handler.Type;
            matchCount = 1;
            lineMatches.Clear();
            lineMatches.Add(handler);
        }
    }

    if (matchCount >= 3)
        return lineMatches;

    return new List<CircleHandler>();
}
public bool CheckForMatches()
    {
        var matches = GetMatches();
        if (matches.Count > 0)
        {
            RemoveMatches(matches);
            return true;
            
        }
        else{
            return false;
        }
    }
private void RemoveMatches(List<CircleHandler> matches, float delay = 0.5f)
    {
        StartCoroutine(RemoveMatchesWithDelay(matches, delay));
    }
private IEnumerator RemoveMatchesWithDelay(List<CircleHandler> matches, float delay)
{
    

    foreach (CircleHandler match in matches)
    {
        if (match != null)
        {
            
            match.transform.DOScale(Vector3.one * 1.5f, 0.3f).SetLoops(2, LoopType.Yoyo);

            GameEvents.OnScoreAddedByType?.Invoke(1, match.Type);
        }
    }

    
    yield return new WaitForSeconds(delay);

    
    foreach (CircleHandler match in matches)
    {
        if (match != null && match.gameObject != null)
        {
            match.transform.DOKill();
            gameBoardCircles[match.Row, match.Col] = null;
            Destroy(match.gameObject);
        }
    }

    DropCircles();
    yield return new WaitForSeconds(delay);
    CheckForMatches();
}
private void DropCircles(float dropDuration = 0.5f)
{
    for (int row = 0; row < rows; row++) 
    {
        for (int col = 0; col < cols; col++) 
        {
            if (gameBoardCircles[row, col] == null) 
            {
                
                for (int aboveRow = col + 1; aboveRow < cols; aboveRow++)
                {
                    if (gameBoardCircles[row, aboveRow] != null) 
                    {
                        CircleHandler handler = gameBoardCircles[row, aboveRow].GetComponent<CircleHandler>();
                        Vector2 targetPosition = gameBoardPositions[row, col];

                        
                        gameBoardCircles[row, aboveRow].transform.DOMove(targetPosition, dropDuration);

                        
                        gameBoardCircles[row, col] = gameBoardCircles[row, aboveRow];
                        gameBoardCircles[row, aboveRow] = null;

                        
                        handler.UpdatePosition(row, col);

                        break; 
                    }
                }

                if(gameBoardCircles[row, col] == null)
                {
                   CreateCircle(circlePrefabs, row, col, 0.7f, 0.05f); 
                }

            }
        }
    }

}




















}






