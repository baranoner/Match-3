using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class GameBoard : MonoBehaviour
{
    private Vector2[,] _gameBoardPositions;
    private GameObject[,] _gameBoardCircles;
    private GameObject[] _circlePrefabs;
    private Color[] _typesOfColors;
    private IObjectPool<GameObject> _circlePool;
    private int _rows;
    private int _cols;
    private int _randomType;
    private Vector2 _finalPosition;
    private Vector2 _startPosition;

    public void Initialize(int rows, int cols, float spacing, GameObject[] circlePrefabs)
    {
        this._rows = rows;
        this._cols = cols;
        this._circlePrefabs = circlePrefabs;
        
        _gameBoardPositions = new Vector2[rows, cols];
        _gameBoardCircles = new GameObject[rows, cols];
        _circlePool = new ObjectPool<GameObject>(CreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, false, 50, 200);
        _typesOfColors = new Color[_circlePrefabs.Length];
        
        SavePrefabColors();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                _gameBoardPositions[i, j] = new Vector2(i * spacing, j * spacing);
            }
        }
    }

    private void SavePrefabColors()
    {
        for (int i = 0; i < _circlePrefabs.Length; i++)
        {
            Color color = _circlePrefabs[i].GetComponent<SpriteRenderer>().color;
            _typesOfColors[i] = color;
        }
    }

    private void OnDestroyPooledObject(GameObject obj)
    {
        Destroy(obj);
    }

    private void OnReleaseToPool(GameObject obj)
    {
        obj.transform.localScale = Vector3.one;
        obj.SetActive(false);
    }

    private void OnGetFromPool(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.position = _startPosition;
    }

    private GameObject CreatePooledObject()
    {
        GameObject prefab = _circlePrefabs[_randomType];
        GameObject obj = Instantiate(prefab, transform, true);
        obj.SetActive(false);
        return obj;
    } 
    public void PopulateGameBoard(GameObject[] circlePrefabs, float dropDuration, float staggerDelay)
    {
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _cols; j++)
            {
                CreateCircle(i, j, dropDuration, staggerDelay, true);
            }
        }
    }
    private void CreateCircle(int row, int col, float dropDuration, float staggerDelay, bool isAtStart)
    {
         _finalPosition = _gameBoardPositions[row, col];
         _startPosition = _finalPosition + Vector2.up * 15;
        
        if (isAtStart)
        {
            staggerDelay *= (row * _cols + col);
            bool isValid;
            do
            {
                _randomType = Random.Range(0, _typesOfColors.Length);
                isValid = IsTypeValid(row, col, _randomType);
            } while (!isValid);
        }
        else
        {
            _randomType = Random.Range(0, _typesOfColors.Length);
        }
        
        GameObject circle = _circlePool.Get();
        circle.transform.SetParent(transform);

        CircleHandler handler = circle.GetComponent<CircleHandler>();
        handler.Initialize(row, col, _randomType, _typesOfColors);

        _gameBoardCircles[row, col] = circle;

        circle.transform.DOMove(_finalPosition, dropDuration)
            .SetEase(Ease.OutBounce)
            .SetDelay(staggerDelay);
    }
    private bool IsTypeValid(int row, int col, int type)
    {
        
        if (col >= 2) 
        {
            CircleHandler left1 = _gameBoardCircles[row, col - 1]?.GetComponent<CircleHandler>();
            CircleHandler left2 = _gameBoardCircles[row, col - 2]?.GetComponent<CircleHandler>();

            if (left1 != null && left2 != null && left1.Type == type && left2.Type == type)
            {
                return false; 
            }
        }

        
        if (row >= 2) 
        {
            CircleHandler top1 = _gameBoardCircles[row - 1, col]?.GetComponent<CircleHandler>();
            CircleHandler top2 = _gameBoardCircles[row - 2, col]?.GetComponent<CircleHandler>();

            if (top1 != null && top2 != null && top1.Type == type && top2.Type == type)
            {
                return false; 
            }
        }

        return true; 
    }   
    public void SwapCirclesBoardLogic(int rowA, int colA, int rowB, int colB)
        {
            (_gameBoardCircles[rowA, colA], _gameBoardCircles[rowB, colB]) = (_gameBoardCircles[rowB, colB], _gameBoardCircles[rowA, colA]);
        }
    public bool ArePositionsAdjacent(int rowA, int colA, int rowB, int colB)
        {
            return (Mathf.Abs(rowA - rowB) == 1 && colA == colB) || 
                   (Mathf.Abs(colA - colB) == 1 && rowA == rowB);
        }
    private List<CircleHandler> GetMatches()
    {
        List<CircleHandler> matches = new List<CircleHandler>();

        
        for (int row = 0; row < _rows; row++)
        {
            List<CircleHandler> rowMatches = CheckLine(row, 0, 0, 1); 
            matches.AddRange(rowMatches);
        }
        
        for (int col = 0; col < _cols; col++)
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

        for (int i = 0; i < Mathf.Max(_rows, _cols); i++)
        {
            int row = startRow + rowDelta * i;
            int col = startCol + colDelta * i;

            
            if (row >= _rows || col >= _cols || row < 0 || col < 0)
            {
                if (matchCount >= 3)
                    return lineMatches;

                matchType = -1;
                matchCount = 0;
                lineMatches.Clear();
                continue;
            }

            CircleHandler handler = _gameBoardCircles[row, col]?.GetComponent<CircleHandler>();
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

                GameEvents.OnScoreDecreaseByType?.Invoke(match.Type);
            }
        }
        
        yield return new WaitForSeconds(delay);
        
        foreach (CircleHandler match in matches)
        {
            if (match != null && match.gameObject != null)
            {
                match.transform.DOKill();
                _gameBoardCircles[match.Row, match.Col] = null;
                _circlePool.Release(match.gameObject);
            }
        }

        DropCircles();
        yield return new WaitForSeconds(delay);
        CheckForMatches();
    }
    private void DropCircles(float dropDuration = 0.5f)
    {
        for (int row = 0; row < _rows; row++) 
        {
            for (int col = 0; col < _cols; col++) 
            {
                if (_gameBoardCircles[row, col] == null) 
                {
                    
                    for (int aboveRow = col + 1; aboveRow < _cols; aboveRow++)
                    {
                        if (_gameBoardCircles[row, aboveRow] != null) 
                        {
                            CircleHandler handler = _gameBoardCircles[row, aboveRow].GetComponent<CircleHandler>();
                            Vector2 targetPosition = _gameBoardPositions[row, col];

                            
                            _gameBoardCircles[row, aboveRow].transform.DOMove(targetPosition, dropDuration);

                            
                            _gameBoardCircles[row, col] = _gameBoardCircles[row, aboveRow];
                            _gameBoardCircles[row, aboveRow] = null;

                            
                            handler.UpdatePosition(row, col);

                            break; 
                        }
                    }

                    if(_gameBoardCircles[row, col] == null)
                    {
                       CreateCircle(row, col, 0.7f, 0.05f, false); 
                    }

                }
            }
        }

    }
}






